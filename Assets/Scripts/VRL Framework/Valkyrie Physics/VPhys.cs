using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//EQUIVALENT OF THE PHYSICS LIBRARY IN UNITY
public class VPhys : MonoBehaviour
{

    [SerializeField] Vector3 dir;

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
            if (Vector3.Dot(basisVector1, basisVector2) != 0) throw new ArgumentException("b - a and c - a must be orthogonal!");

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





    //General Raycast Method
    public bool Raycast(Vector3 start, Vector3 dir, float dist)
    {
        //should change to use KDTree or OctTree nodes at some point 
        ValkyrieCollider[] colliders = FindObjectsByType<ValkyrieCollider>(FindObjectsSortMode.None);

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

    public void Update()
    {
        Raycast(transform.position, dir, 1);
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position, dir);
    }

    //Helper Methods

    public bool DidIntersectBox(VRay ray, ValkyrieBoxCollider vbc)
    {
        bool topPlane = DidIntersectBoundedRect(ray, vbc.topPlane);
        bool bottomPlane = DidIntersectBoundedRect(ray, vbc.bottomPlane);
        bool leftPlane = DidIntersectBoundedRect(ray, vbc.leftPlane);
        bool rightPlane = DidIntersectBoundedRect(ray, vbc.rightPlane);
        bool frontPlane = DidIntersectBoundedRect(ray, vbc.frontPlane);
        bool backPlane = DidIntersectBoundedRect(ray, vbc.backPlane);

        return topPlane || bottomPlane || leftPlane || rightPlane || frontPlane || backPlane;
    }

    //check VRLands notes for derivation of this. it's relatively simple calc 3 
    public bool DidIntersectSphere(VRay ray, Vector3 center, float radius)
    {
        float a = Vector3.SqrMagnitude(ray.dir);
        float b = 2 * ray.start.x * ray.dir.x - 2 * ray.dir.x * center.x + 2 * ray.start.y * ray.dir.y - 2 * ray.dir.y * center.y + 2 * ray.start.z * ray.dir.z - 2 * ray.dir.z * center.z;
        float c = Vector3.SqrMagnitude(ray.start - center);
        float d = c - Mathf.Pow(radius, 2);

        float discriminant = Mathf.Pow(b, 2) - 4 * a * d;

        if (discriminant < 0) return false;
        return true;
    
    }   

    public bool DidIntersectBoundedRect(VRay ray, BoundedRect plane)
    {
        if (Vector3.Dot(plane.normal, ray.dir) == 0) return false;
        Vector3 pointOfIntersection = ray.start + (Vector3.Dot(plane.normal, plane.origin - ray.start) / Vector3.Dot(plane.normal, ray.dir)) * ray.dir;

        return plane.IsOnPlane(pointOfIntersection);
    }


    public bool DidIntersectBox()
    {
        return false;
    }


    public void Raycast(Vector3 start, Vector3 end)
    {

    }

    public void SphereCast()
    {

    }

}
