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

    public struct Plane
    {
        public Vector3 pt;
        public Vector3 normal;

        public Vector3 minBounds;
        public Vector3 maxBounds; 

        public Plane(Vector3 pt, Vector3 normal)
        {
            this.pt = pt;
            this.normal = normal;

            this.minBounds = Vector3.negativeInfinity;
            this.maxBounds = Vector3.positiveInfinity;
        }

        public Plane(Vector3 pt, Vector3 normal, Vector3 minBounds, Vector3 maxBounds)
        {
            this.pt = pt;
            this.normal = normal;
            this.minBounds = minBounds;
            this.maxBounds = maxBounds;
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

    public bool DidIntersectPlane(VRay ray, Plane plane)
    {
        if (Vector3.Dot(plane.normal, ray.dir) == 0) return false;
        Vector3 checkVec = ray.start + (Vector3.Dot(plane.normal, plane.pt - ray.start) / Vector3.Dot(plane.normal, ray.dir)) * ray.dir;

        return WithinBounds(checkVec, plane.minBounds, plane.maxBounds);
    }

    public bool WithinBounds(Vector3 a, Vector3 minBounds, Vector3 maxBounds)
    {
        if(a.x >= minBounds.x && a.y >= minBounds.y && a.z >= minBounds.z && a.x <= maxBounds.x && a.y <= maxBounds.y && a.z <= maxBounds.z)
        {
            print("Min X: " + minBounds.x + ", " + "Max X: " + maxBounds.x + "Actual: " + a.x);
            print("Min Y: " + minBounds.y + ", " + "Max Y: " + maxBounds.y + "Actual: " + a.y);

            print("Min Z: " + minBounds.z + ", " + "Max Z: " + maxBounds.z + "Actual: " + a.z);


        }
        return a.x >= minBounds.x && a.y >= minBounds.y && a.z >= minBounds.z && a.x <= maxBounds.x && a.y <= maxBounds.y && a.z <= maxBounds.z;
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
