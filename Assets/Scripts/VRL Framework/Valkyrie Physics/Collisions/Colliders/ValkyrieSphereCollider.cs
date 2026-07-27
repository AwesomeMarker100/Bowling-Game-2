using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ValkyrieSphereCollider : ValkyrieCollider
{

    public float radius = 2f;
    private const float tau = 2 * Mathf.PI;


    private void Start()
    {
        this.type = ColliderType.SphereCollider;
        globalCenter = transform.TransformPoint(localCenter);


    }

    public override void DrawGizmos()
    {
        base.DrawGizmos();
        
        Gizmos.DrawWireSphere(globalCenter, radius);

    }

    public override Vector3 GetClosestPointOnBounds(Vector3 point)
    {
        //get vector from our global center to the point being tested
        Vector3 diffVec = point - globalCenter;
        return GetFurthestPoint(diffVec);

    }

    //Mr.G Reference?!?
    /*
    private Vector3 GetUnitCirclePoint(float unitCircleAngle, Vector3 diffVec)
    {
        //polar to normal coords --- remember to convert unitCircleAngle to radians
        //THIS IS JUST IN XY COORDS - NEED TO APPLY ROTATION ALONG CROSS VEC TO GET ACTUAL CLOSEST POINT -- NEED TO GET Z
        Vector3 initPointInXY = new Vector3(radius * Mathf.Cos(Mathf.Deg2Rad * unitCircleAngle), radius * Mathf.Sin(Mathf.Deg2Rad * unitCircleAngle), 0f) + globalCenter; 
        Vector3 closestPointDiffVec = initPointInXY - globalCenter;

        //get the cross vector between diffVec(to the point) and closestPointDiffVec
        Vector3 crossVec = Vector3.Cross(diffVec, closestPointDiffVec);
        float angle = Vector3.SignedAngle(closestPointDiffVec, diffVec, crossVec);

        Debug.DrawRay(globalCenter, crossVec);

        Quaternion angleAxis = Quaternion.AngleAxis(angle, crossVec);

        Debug.DrawRay(globalCenter, angleAxis * closestPointDiffVec, Color.blue);


        return angleAxis * closestPointDiffVec + globalCenter;
    }
    */

    //furthest point easy on sphere, just go that normalized direction * radius + our center point
    public override Vector3 GetFurthestPoint(Vector3 dir)
    {
        return dir != Vector3.zero ? globalCenter + dir.normalized * radius : globalCenter + Vector3.forward * radius;
    }

    //point is in global coordinates
    public override bool PointInBounds(Vector3 point)
    {
        return Vector3.Distance(globalCenter, point) <= radius;
    }

    public void SayHiAgain(ProgrammableNodeSignal nodeSignal)
    {
        //nodeSignal.SetTaskStarted();
        nodeSignal.SetTaskCompleted();
    }
}
