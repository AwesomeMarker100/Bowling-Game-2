using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationTask : Task
{

    [SerializeField] Transform location;
    [SerializeField] float safeZoneRadius = 3f;



    public override bool IsTaskDone()
    {
        return Vector3.Distance(location.position, player.transform.position) <= safeZoneRadius;

    }
}
