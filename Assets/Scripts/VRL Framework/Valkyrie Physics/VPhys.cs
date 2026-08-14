using Assets.Scripts.VRL_Framework.Valkyrie_Physics.Collisions.Colliders.IShapes;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.UI;

using Simplex = GJK.Simplex;
using Polyhedron = GJK.Polyhedron;
using static GJK;


//EQUIVALENT OF THE PHYSICS LIBRARY IN UNITY

[ExecuteInEditMode]
public class VPhys : MonoBehaviour
{

    // [SerializeField] Vector3 dir;

    [SerializeField] int maxDataPerOctNode = 5;
    [SerializeField] bool showTreeGizmo = false;

    public static VPhys Instance { get; private set; }

    //Spatial Structures
    private OctTree4 colTree;

    //Structs / Enums
    #region
    public struct VRay
    {
        public Vector3 start;
        public Vector3 dir;

        public VRay(Vector3 start, Vector3 dir)
        {
            this.start = start;
            this.dir = dir;
        }
    }

    public enum RaycastMode
    {
        StopAtFirstHit, NoStop
    }

    public struct Edge
    {
        public Vector3 pt1;
        public Vector3 pt2;

        public Edge(Vector3 pt1, Vector3 pt2)
        {
            this.pt1 = pt1;
            this.pt2 = pt2;
        }
    }

    public struct BoundedRect
    {
        public Vector3 origin;
        public Vector3 normal;

        private Vector3 basisVector1;
        private Vector3 basisVector2;

        private float bv1Length; //basis vector 1 length
        private float bv2Length; //basis vector 2 length

        //b - a, and c - a must be orthogonal vectors 
        public BoundedRect(Vector3 a, Vector3 b, Vector3 c) //also used to establish bounds, obviously
        {
            origin = a;
            basisVector1 = b - a;
            basisVector2 = c - a;

            if (MinoMath.VApproximately(basisVector1, Vector3.zero)) throw new ArgumentException("Not given 3 unique points!");
            if (MinoMath.VApproximately(basisVector2, Vector3.zero)) throw new ArgumentException("Not given 3 unique points!");
            if (Mathf.Abs(Vector3.Dot(basisVector1, basisVector2)) > 1e-2f) throw new ArgumentException($"BV1: {basisVector1}, BV2: {basisVector2}");
            
            

            bv1Length = basisVector1.magnitude;
            bv2Length = basisVector2.magnitude;

            normal = Vector3.Cross(b - a, c - a).normalized;
        }



        /***
         * Checks if p is inside bounded plane rectangle
         * 
         */
        public bool IsOnPlane(Vector3 p)
        {
            //this first checks if p is on the infinite plane spanned by the basis vectors 
            if (!MinoMath.FApproximately(Vector3.Dot(normal, p - origin), 0, 0.0001f)) return false;

            //next part is checking if p is within the given bounds 
            //essentially mapping p in terms of basis vectors of plane
            Vector3 rebasedP = p - origin;

            //check dots with basis vectors
            float compBV1Mag = Vector3.Dot(basisVector1, rebasedP) / bv1Length;
            float compBV2Mag = Vector3.Dot(basisVector2, rebasedP) / bv2Length;
            return MinoMath.Within(compBV1Mag, 0, bv1Length) && MinoMath.Within(compBV2Mag, 0, bv2Length);
        }

    }
    #endregion

    //Awake / OnValidate / OnDrawGizmos
    #region

    private void OnValidate()
    {
        Awake();
    }

    public void Awake()
    {

        //Singleton pattern
        if(Instance != null && Instance != this)
        {
            enabled = false;
            return;
        }

        Instance = this;
    }

    public void OnDrawGizmos()
    {
        if (!showTreeGizmo) return;

        BuildColliderTree();

        Gizmos.color = Color.violetRed;
        foreach (OctNode node in colTree.nodes)
        {
            Gizmos.DrawWireCube((node.bounds.bottomLeft + node.bounds.topRight) / 2, node.bounds.topRight - node.bounds.bottomLeft);
        }
       
    }

    #endregion

    //General Raycast Method
    #region
    public static bool Raycast(Vector3 start, Vector3 dir, float dist)
    {
        //should change to use KDTree or OctTree nodes at some point 
        ValkyrieCollider[] colliders = FindObjectsByType<ValkyrieCollider>();

        VRay ray = new VRay(start, dir.normalized);
        bool hitSomething = false;

        foreach (ValkyrieCollider collider in colliders)
        {
            switch (collider)
            {
                case ValkyrieBoxCollider vbc:
                    if (DidIntersectBox(ray, vbc)) hitSomething = true;
                    break;

                
                case ValkyrieCapsuleCollider vcc:
                    break;

                case ValkyrieSphereCollider vsc:
                    if (DidIntersectSphere(ray, vsc.globalCenter, vsc.radius)) hitSomething = true;
                    break;

                case Valkyrie2DRectCollider v2Drc:
                    if (DidIntersectBoundedRect(ray, v2Drc.plane)) hitSomething = true;
                    break;
            }
        }

        return hitSomething;
    }
    #endregion


    //Helper Methods
    #region
    public static bool DidIntersectBox(VRay ray, ValkyrieBoxCollider vbc)
    {
        bool topPlaneIntersection = DidIntersectBoundedRect(ray, vbc.topPlane);
        bool bottomPlaneIntersection = DidIntersectBoundedRect(ray, vbc.bottomPlane);
        bool leftPlane = DidIntersectBoundedRect(ray, vbc.leftPlane);
        bool rightPlane = DidIntersectBoundedRect(ray, vbc.rightPlane);
        bool frontPlane = DidIntersectBoundedRect(ray, vbc.frontPlane);
        bool backPlane = DidIntersectBoundedRect(ray, vbc.backPlane);

        return topPlaneIntersection || bottomPlaneIntersection || leftPlane || rightPlane || frontPlane || backPlane;
    }

    //check VRLands notes for derivation of this. it's relatively simple calc 3 
    public static bool DidIntersectSphere(VRay ray, Vector3 center, float radius)
    {
        float a = Vector3.SqrMagnitude(ray.dir);
        float b = 2 * ray.start.x * ray.dir.x - 2 * ray.dir.x * center.x + 2 * ray.start.y * ray.dir.y - 2 * ray.dir.y * center.y + 2 * ray.start.z * ray.dir.z - 2 * ray.dir.z * center.z;
        float c = Vector3.SqrMagnitude(ray.start - center);
        float d = c - Mathf.Pow(radius, 2);

        float discriminant = Mathf.Pow(b, 2) - 4 * a * d;

        if (discriminant < 0) return false;
        return true;
    
    }   

    public static bool DidIntersectBoundedRect(VRay ray, BoundedRect plane)
    {
        if (Vector3.Dot(plane.normal, ray.dir) == 0) return false;
        Vector3 pointOfIntersection = ray.start + (Vector3.Dot(plane.normal, plane.origin - ray.start) / Vector3.Dot(plane.normal, ray.dir)) * ray.dir;

        return plane.IsOnPlane(pointOfIntersection);
    }


    public static bool Boxcast(Vector3 center, Vector3 halfLengths,  Quaternion rotation)
    {
        throw new NotImplementedException();
    }

    public static bool SphereCast(Vector3 center, float radius)
    {
        //make regional search asap 

        foreach(ValkyrieCollider col in FindObjectsByType<ValkyrieCollider>())
        {
            if(Vector3.Distance(col.globalCenter, center) < radius)
            {
                return true;
            }
        }

        return false;
    }


    #endregion

    //Collider Tree Setup
    #region

    public BoxBounds GetBoxBounds(ValkyrieCollider[] colliders)
    {
        Vector3 min = Vector3.positiveInfinity;
        Vector3 max = Vector3.negativeInfinity;

        foreach(ValkyrieCollider col in colliders)
        {
            Vector3 colPos = col.transform.position;
            if (colPos.x < min.x) min.x = colPos.x;
            else if (colPos.x > max.x) max.x = colPos.x;

            if (colPos.y < min.y) min.y = colPos.y;
            else if (colPos.y > max.y) max.y = colPos.y;

            if (colPos.z < min.z) min.z = colPos.z;
            else if (colPos.z > max.z) max.z = colPos.z;
        }

        return new BoxBounds(max, min);
    }

    public void BuildColliderTree()
    {
        ValkyrieCollider[] colliders = FindObjectsByType<ValkyrieCollider>();

        BoxBounds boxBounds = GetBoxBounds(colliders);
        colTree = new OctTree4(boxBounds, maxDataPerOctNode);

        foreach(ValkyrieCollider col in colliders)
        {
            colTree.InsertData(col.GetHashCode(), col.transform.position);
        }
    }


    #endregion


    //Shape Cast Methods (mostly used for tunneling prevention - e.g. CCD or Continuous Collision Detection) 
    #region
    
    //relVel = v_2 - v_1
    public static bool ShapeCast(IColliderShape shape1, IColliderShape shape2, Vector3 relVel)
    {
        
        if(GJK.CheckIfCollided(shape1, shape2) is var basicColDat && !basicColDat.Item1)
        {

            Simplex termSimp = basicColDat.Item2;
            /*Vector3 trackerPt = Vector3.zero; //t = 0 -> curPt = Vector3.zero, t = 1 -> curPt = relVel

            Vector3 closestPt = termSimp.GetClosestPoint(trackerPt);

            Vector3 normal = trackerPt - closestPt;

            float tNewDenom = Vector3.Dot(relVel, normal);
            if (tNewDenom == 0) throw new DivideByZeroException("Bitch ass looking ass");

            float tNew = Vector3.Dot(closestPt, normal) / tNewDenom;
            if (tNew > 1) return false;

            Vector3 newSprtPt = shape1.GetFurthestPoint(normal) - shape2.GetFurthestPoint(-normal); //it's just the support function but generalized for IColliderShape
            float d = Vector3.Dot(newSprtPt - closestPt, normal) / normal.magnitude;
            if (d <= 0.0001f) return true;

            termSimp.AddPoint(new SimplexPt(newSprtPt, normal));*/

            float d = Mathf.Infinity;
            float tCur = 0;
            float tNew;

            Vector3 trackerPt;
            Vector3 closestPt;
            Vector3 normal;

            float tNewDenom;

            Vector3 newSprtPt;

            do
            {
                trackerPt = tCur * relVel;
                closestPt = termSimp.GetClosestPointAndTrim(trackerPt);
                normal = trackerPt - closestPt;

                tNewDenom = Vector3.Dot(relVel, normal);
                if (tNewDenom == 0) throw new DivideByZeroException("Bitch ass looking ass");

                tNew = Vector3.Dot(closestPt, normal) / tNewDenom;
                if (tNew > 1) return false;


                newSprtPt = shape1.GetFurthestPoint(normal) - shape2.GetFurthestPoint(-normal); //generalized support func for IColliderShape


                d = Vector3.Dot(newSprtPt - closestPt, normal);
                termSimp.AddPoint(new SimplexPt(newSprtPt, normal));
                tCur = tNew;


            } while (d > 0.0001f);

            return true;
        }

        return false;
    }


    #endregion
}
//hkjhka