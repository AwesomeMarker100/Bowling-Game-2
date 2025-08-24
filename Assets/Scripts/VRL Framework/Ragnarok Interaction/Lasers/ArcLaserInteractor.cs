using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcLaserInteractor : LaserInteractor
{
    [SerializeField] private float initialAngle = Mathf.PI / 2;
    [SerializeField] private float arcRadius = 1f;
    [SerializeField] private float thetaScale = 0.03f;

    [SerializeField] private Vector3 rotationOffsets;

    private void Awake()
    {
        this.laserType = LaserType.Arc;
    }

    public override void CreateLaser()
    {
        laser.positionCount = (int)(1 / thetaScale) + 1;
        laser.endColor = Color.red;

        laser.startColor = Color.red;
        laser.endColor = Color.red;

        Vector3 origin = transform.position;

        float theta = initialAngle;
        int i = 0;

        while (i < laser.positionCount)
        {
            Vector3 point = Vector3.zero;

            point.x = arcRadius * Mathf.Cos(theta); //x is your cos(times the radius)
            point.y = arcRadius * Mathf.Sin(theta); //y is your sin(times the radius)

            Vector3 truePoint = Quaternion.AngleAxis(rotationOffsets.y, Vector3.up) * Quaternion.AngleAxis(rotationOffsets.z, Vector3.forward) * Quaternion.AngleAxis(rotationOffsets.x, Vector3.left) * (point + origin + Vector3.down * arcRadius + Vector3.right);

            laser.SetPosition(i, truePoint);
            //if (DidCollide(truePoint, i)) break;

            theta -= initialAngle * thetaScale;
            i++;
        }

        laser.positionCount = i;
    }
}
