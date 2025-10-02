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

    public struct BoundedPlane
    {
        public Vector3 origin;
        public Vector3 normal;

        private Vector3 basisVector1;
        private Vector3 basisVector2;

        private float bv1Length; //basic vector 1 length
        private float bv2Length; //basis vector 2 length

        public BoundedPlane(Vector3 a, Vector3 b, Vector3 c) //also used to establish bounds, obviously
        {
            origin = a;
            basisVector1 = b - a;
            basisVector2 = c - a;

            bv1Length = basisVector1.magnitude;
            bv2Length = basisVector2.magnitude;

            normal = Vector3.Cross(b - a, c - a);

        }
        /***
         * Checks if p is inside bounded plane rectangle
         * 
         */
        public bool IsOnPlane(Vector3 p)
        {
            if (!MinoMath.FApproximately(Vector3.Dot(normal, p - origin), 0, 0.0001f)) return false;

            Vector3 rebasedP = p - origin;
            float compBV1Mag = Vector3.Dot(basisVector1, rebasedP) / bv1Length;
            float compBV2Mag = Vector3.Dot(basisVector2, rebasedP) / bv2Length;
            //print("here: " + basisVector1 + " " + basisVector2 + " " + compBV1Mag + " " + compBV2Mag);
            return MinoMath.Within(compBV1Mag, 0, bv1Length) && MinoMath.Within(compBV2Mag, 0, bv2Length);
        }

    }
    #endregion

    public bool Raycast(Vector3 start, Vector3 dir, float dist)
    {
        ValkyrieCollider[] colliders = FindObjectsByType<ValkyrieCollider>(FindObjectsSortMode.None);

        VRay ray = new VRay(start, dir.normalized);
        bool hitSomething = true;

        foreach (ValkyrieCollider collider in colliders)
        {
            switch (collider)
            {
                case ValkyrieBoxCollider:
                    ValkyrieBoxCollider vbc = collider as ValkyrieBoxCollider;

                    bool topPlane = DidIntersectPlane(ray, vbc.topPlane);
                    bool bottomPlane = DidIntersectPlane(ray, vbc.bottomPlane);
                    bool leftPlane = DidIntersectPlane(ray, vbc.leftPlane);
                    bool rightPlane = DidIntersectPlane(ray, vbc.rightPlane);
                    bool frontPlane = DidIntersectPlane(ray, vbc.frontPlane);
                    bool backPlane = DidIntersectPlane(ray, vbc.backPlane);


                    bool hitBox = topPlane || bottomPlane || leftPlane || rightPlane || frontPlane || backPlane;
                    if (hitBox)
                    {
                        print("hit box collider");
                        hitSomething = true;
                    }
                    break;

                
                case ValkyrieCapsuleCollider:
                    ValkyrieCapsuleCollider vcc = collider as ValkyrieCapsuleCollider;
                    break;

                case ValkyrieSphereCollider:
                    ValkyrieSphereCollider vsc = collider as ValkyrieSphereCollider;
                    if (DidIntersectSphere(ray, vsc.globalCenter, vsc.radius))
                    {
                        print("hit sphere collider " + vsc.name);
                        hitSomething = true;
                    }
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

    public bool DidIntersectPlane(VRay ray, BoundedPlane plane)
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
