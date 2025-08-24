using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValkyrieRaycastHit
{

    public Vector3 hitPoint;
    public ValkyrieCollider collider;

    public HittableObject uiObject;

    public ValkyrieRaycastHit(Vector3 hitPoint, ValkyrieCollider collider)
    {
        this.hitPoint = hitPoint;
        this.collider = collider;

    }

    public ValkyrieRaycastHit(Vector3 hitPoint, HittableObject uiObject)
    {

        this.hitPoint = hitPoint;
        this.uiObject = uiObject;

    }


}
