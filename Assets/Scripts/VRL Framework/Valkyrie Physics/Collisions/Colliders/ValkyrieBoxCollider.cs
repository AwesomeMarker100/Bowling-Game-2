using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Plane = VPhys.Plane;

[ExecuteInEditMode]
public class ValkyrieBoxCollider : ValkyrieCollider
{

    [Header("Dimensions")]
    public Vector3 size;

    [HideInInspector] public float xMin;
    [HideInInspector] public float xMax;
    [HideInInspector] public float yMin;
    [HideInInspector] public float yMax;
    [HideInInspector] public float zMin;
    [HideInInspector] public float zMax;


    [HideInInspector] public Vector3 topLeftFront;
    [HideInInspector] public Vector3 bottomLeftFront;
    [HideInInspector] public Vector3 topRightFront;
    [HideInInspector] public Vector3 bottomRightFront;
    [HideInInspector] public Vector3 topLeftBack;
    [HideInInspector] public Vector3 topRightBack;
    [HideInInspector] public Vector3 bottomLeftBack;
    [HideInInspector] public Vector3 bottomRightBack;

    public Plane topPlane;
    public Plane bottomPlane;
    public Plane rightPlane;
    public Plane leftPlane;
    public Plane backPlane;
    public Plane frontPlane;


    private void Start()
    {
        this.type = ColliderType.BoxCollider;
        globalCenter = transform.TransformPoint(localCenter);


        topPlane = new Plane();
        bottomPlane = new Plane();
        leftPlane = new Plane();
        backPlane = new Plane();
        frontPlane = new Plane();
        rightPlane = new Plane();
    }


    public override void SetBounds()
    {
        //update mins and maxes(boundary conditions) IN WORLD SPACE


        xMin = globalCenter.x - (size.x / 2);
        xMax = globalCenter.x + (size.x / 2);
        yMin = globalCenter.y - (size.y / 2);
        yMax = globalCenter.y + (size.y / 2);
        zMin = globalCenter.z - (size.z / 2);
        zMax = globalCenter.z + (size.z / 2);

        topLeftFront = transform.rotation * (new Vector3(xMin, yMax, zMin) - globalCenter);
        topLeftFront += globalCenter;

        topRightFront = transform.rotation * (new Vector3(xMax, yMax, zMin) - globalCenter);
        topRightFront += globalCenter;

        bottomLeftFront = transform.rotation * (new Vector3(xMin, yMin, zMin) - globalCenter);
        bottomLeftFront += globalCenter;

        bottomRightFront = transform.rotation * (new Vector3(xMax, yMin, zMin) - globalCenter);
        bottomRightFront += globalCenter;

        topLeftBack = transform.rotation * (new Vector3(xMin, yMax, zMax) - globalCenter);
        topLeftBack += globalCenter;

        topRightBack = transform.rotation * (new Vector3(xMax, yMax, zMax) - globalCenter);
        topRightBack += globalCenter;


        bottomLeftBack = transform.rotation * (new Vector3(xMin, yMin, zMax) - globalCenter);
        bottomLeftBack += globalCenter;


        bottomRightBack = transform.rotation * (new Vector3(xMax, yMin, zMax) - globalCenter);
        bottomRightBack += globalCenter;

        //TOP PLANE
        topPlane.normal = Vector3.Cross(topRightBack - topLeftBack, topLeftFront - topLeftBack).normalized;
        (Vector3, Vector3) minMaxBounds = GetMinMaxes(new Vector3[] { topLeftBack, topLeftFront, topRightBack, topRightFront });
        topPlane.minBounds = minMaxBounds.Item1;
        topPlane.maxBounds = minMaxBounds.Item2;
        topPlane.pt = topLeftFront;
        if (Vector3.Dot(topLeftBack - globalCenter, topPlane.normal) < 0) topPlane.normal *= -1;

        //BOTTOM PLANE
        bottomPlane.normal = Vector3.Cross(bottomRightBack - bottomLeftBack, bottomLeftFront - bottomLeftBack).normalized;
        (Vector3, Vector3) minMaxBounds2 = GetMinMaxes(new Vector3[] { bottomRightBack, bottomLeftBack, bottomLeftFront, bottomRightFront });
        bottomPlane.minBounds = minMaxBounds2.Item1;
        bottomPlane.maxBounds = minMaxBounds2.Item2;
        bottomPlane.pt = bottomRightBack;
        if (Vector3.Dot(bottomLeftBack - globalCenter, bottomPlane.normal) < 0) bottomPlane.normal *= -1;

        //RIGHT PLANE
        rightPlane.normal = Vector3.Cross(topRightBack - topRightFront, bottomRightFront - topRightFront).normalized;
        (Vector3, Vector3) minMaxBounds3 = GetMinMaxes(new Vector3[] { topRightBack, bottomRightBack, topRightFront, bottomRightFront});
        rightPlane.minBounds = minMaxBounds3.Item1;
        rightPlane.maxBounds = minMaxBounds3.Item2;
        rightPlane.pt = topRightFront;
        if (Vector3.Dot(topRightFront - globalCenter, rightPlane.normal) < 0) rightPlane.normal *= -1;
        
        leftPlane.normal = Vector3.Cross(topLeftBack - topLeftFront, bottomLeftFront - topLeftFront).normalized;
        (Vector3, Vector3) minMaxBounds4 = GetMinMaxes(new Vector3[] { topLeftBack, topLeftFront, bottomLeftBack, bottomLeftFront });
        leftPlane.minBounds = minMaxBounds4.Item1;
        leftPlane.maxBounds = minMaxBounds4.Item2;
        leftPlane.pt = topLeftFront;
        if (Vector3.Dot(topLeftFront - globalCenter, leftPlane.normal) < 0) leftPlane.normal *= -1;

        frontPlane.normal = Vector3.Cross(topRightFront - topLeftFront, bottomLeftFront - topLeftFront).normalized;
        (Vector3, Vector3) minMaxBounds5 = GetMinMaxes(new Vector3[] { topRightFront, topLeftFront, bottomLeftFront, bottomRightFront});
        frontPlane.minBounds = minMaxBounds5.Item1;
        frontPlane.maxBounds = minMaxBounds5.Item2;
        frontPlane.pt = topLeftFront;
        if(Vector3.Dot(topRightFront - globalCenter, frontPlane.normal) < 0) frontPlane.normal *= -1;

        backPlane.normal = Vector3.Cross(topRightBack - topLeftBack, bottomLeftBack - topLeftBack).normalized;
        (Vector3, Vector3) minMaxBounds6 = GetMinMaxes(new Vector3[] { topRightBack, bottomRightBack, topLeftBack, bottomLeftBack});
        backPlane.minBounds = minMaxBounds6.Item1;
        backPlane.maxBounds = minMaxBounds6.Item2;
        backPlane.pt = topRightBack;
        if(Vector3.Dot(topRightBack - globalCenter, backPlane.normal) < 0) backPlane.normal *= -1;
    }

    public (Vector3, Vector3) GetMinMaxes(Vector3[] pts)
    {
        Vector3 mins = Vector3.positiveInfinity;
        Vector3 maxes = Vector3.negativeInfinity;

        foreach(Vector3 pt in pts)
        {
            if (pt.x < mins.x) mins.x = pt.x;
            if(pt.y < mins.y) mins.y = pt.y;
            if(pt.z < mins.z) mins.z = pt.z;
            if(pt.x > maxes.x) maxes.x = pt.x;
            if(pt.y > maxes.y) maxes.y = pt.y;
            if(pt.z > maxes.z) maxes.z = pt.z;
        }

        return (mins, maxes);
    }



    //CONFIRMED WORKS
    public override void DrawGizmos()
    {
        base.DrawGizmos();

        SetBounds();

        

        Gizmos.DrawLine(topLeftFront, topRightFront);
        Gizmos.DrawLine(topLeftFront, topLeftBack);
        Gizmos.DrawLine(topLeftFront, bottomLeftFront);
        Gizmos.DrawLine(bottomLeftFront, bottomRightFront);
        Gizmos.DrawLine(topRightFront, bottomRightFront);
        Gizmos.DrawLine(bottomLeftFront, bottomLeftBack);
        Gizmos.DrawLine(bottomLeftBack, bottomRightBack);
        Gizmos.DrawLine(bottomRightFront, bottomRightBack);
        Gizmos.DrawLine(topRightFront, topRightBack);
        Gizmos.DrawLine(topRightBack, topLeftBack);
        Gizmos.DrawLine(topRightBack, bottomRightBack);
        Gizmos.DrawLine(topLeftBack, bottomLeftBack);

    }



    public override Vector3 GetClosestPointOnBounds(Vector3 point)
    {


        //if x or y or z is in bounds, dont change it, if they aren't, then clamp them to either the min or max depending on if they're higher than min or max

        return new Vector3(Mathf.Clamp(point.x, xMin, xMax), Mathf.Clamp(point.y, yMin, yMax), Mathf.Clamp(point.z, zMin, zMax)); //just clamp each value

    }



    public override bool PointInBounds(Vector3 point)
    {
        return (point.x <= xMax && point.x >= xMin) && (point.y <= yMax && point.y >= yMin) && (point.z <= zMax && point.z >= zMin);
    }

    //CHECK BELOW 
    public override Vector3 GetFurthestPoint(Vector3 dir) //the furthest point HAS to be one of the corners
    {
        //that's not true -- my bad G

        Vector3[] verts =
        {

            topLeftFront,
            topRightFront,
            bottomLeftFront,
            bottomRightFront,
            topLeftBack,
            topRightBack,
            bottomLeftBack,
            bottomRightBack,


        };


        Vector3 furthestPoint = topLeftFront;

        for(int i = 1; verts.Length > i; i++)
        {
            //if the furthest point isn't as aligned as well to the given direction as another point then change the furthest point
            if (Vector3.Dot(furthestPoint, dir) < Vector3.Dot(verts[i], dir)) furthestPoint = verts[i];

        }

        return furthestPoint;

    }

}
