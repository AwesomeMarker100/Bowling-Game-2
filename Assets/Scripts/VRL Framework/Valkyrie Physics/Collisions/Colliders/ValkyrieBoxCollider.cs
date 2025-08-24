using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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


    [HideInInspector] public Vector3 topLeft;
    [HideInInspector] public Vector3 bottomLeft;
    [HideInInspector] public Vector3 topRight;
    [HideInInspector] public Vector3 bottomRight;
    [HideInInspector] public Vector3 topLeftBack;
    [HideInInspector] public Vector3 topRightBack;
    [HideInInspector] public Vector3 bottomLeftBack;
    [HideInInspector] public Vector3 bottomRightBack;


    private void Start()
    {
        this.type = ColliderType.BoxCollider;
        globalCenter = transform.TransformPoint(localCenter);

    }


    public override void SetBounds()
    {
        //update mins and maxes(boundary coordinates) IN WORLD SPACE


        xMin = globalCenter.x - (size.x / 2);
        xMax = globalCenter.x + (size.x / 2);
        yMin = globalCenter.y - (size.y / 2);
        yMax = globalCenter.y + (size.y / 2);
        zMin = globalCenter.z - (size.z / 2);
        zMax = globalCenter.z + (size.z / 2);

        topLeft = transform.rotation * (new Vector3(xMin, yMax, zMin) - globalCenter);
        topLeft += globalCenter;

        topRight = transform.rotation * (new Vector3(xMax, yMax, zMin) - globalCenter);
        topRight += globalCenter;

        bottomLeft = transform.rotation * (new Vector3(xMin, yMin, zMin) - globalCenter);
        bottomLeft += globalCenter;

        bottomRight = transform.rotation * (new Vector3(xMax, yMin, zMin) - globalCenter);
        bottomRight += globalCenter;

        topLeftBack = transform.rotation * (new Vector3(xMin, yMax, zMax) - globalCenter);
        topLeftBack += globalCenter;

        topRightBack = transform.rotation * (new Vector3(xMax, yMax, zMax) - globalCenter);
        topRightBack += globalCenter;


        bottomLeftBack = transform.rotation * (new Vector3(xMin, yMin, zMax) - globalCenter);
        bottomLeftBack += globalCenter;


        bottomRightBack = transform.rotation * (new Vector3(xMax, yMin, zMax) - globalCenter);
        bottomRightBack += globalCenter;
    }



    //CONFIRMED WORKS
    public override void DrawGizmos()
    {
        base.DrawGizmos();

        SetBounds();

        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topLeft, topLeftBack);
        Gizmos.DrawLine(topLeft, bottomLeft);
        Gizmos.DrawLine(bottomLeft, bottomRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomLeft, bottomLeftBack);
        Gizmos.DrawLine(bottomLeftBack, bottomRightBack);
        Gizmos.DrawLine(bottomRight, bottomRightBack);
        Gizmos.DrawLine(topRight, topRightBack);
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

            topLeft,
            topRight,
            bottomLeft,
            bottomRight,
            topLeftBack,
            topRightBack,
            bottomLeftBack,
            bottomRightBack,


        };


        Vector3 furthestPoint = topLeft;

        for(int i = 1; verts.Length > i; i++)
        {
            //if the furthest point isn't as aligned as well to the given direction as another point then change the furthest point
            if (Vector3.Dot(furthestPoint, dir) < Vector3.Dot(verts[i], dir)) furthestPoint = verts[i];

        }

        return furthestPoint;

    }

}
