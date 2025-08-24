using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearLaserInteractor : LaserInteractor
{
    [Header("Line Settings")]
    public Axis forwardAxis = Axis.Z;
    [SerializeField] private bool flipDirection = false;
    [SerializeField] private int linearSegments = 100;


    private Vector3 forwardVec;

    private void Awake()
    {
        this.laserType = LaserType.Linear;

        if(forwardAxis == Axis.X)
        {

            forwardVec = Vector3.left;

        } else if(forwardAxis == Axis.Y)
        {

            forwardVec = Vector3.up;

        } else
        {

            forwardVec = Vector3.forward;

        }

        if (flipDirection) forwardVec *= -1;

    }

    public override void CreateLaser()
    {
        if (maximumDistance <= 0) return;

        laser.positionCount = linearSegments + 1;
        laser.SetPosition(0, Vector3.zero);

        float interval = maximumDistance / linearSegments;
        int index = 1;

        for (int i = 1; i <= linearSegments; i++)
        {
            Vector3 lastPosition = laser.GetPosition(i - 1);
            Vector3 rawPosition = lastPosition + (forwardVec * interval);

            laser.SetPosition(i, rawPosition);

            if (doCollisions && DidCollide(i, interval)) break;
            index++;
        }

        laser.positionCount = index;

        ballPoint.position = transform.TransformPoint(laser.GetPosition(laser.positionCount - 1));
    }
}
