using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValkyrieCollision 
{

    public ValkyrieCollider[] colliders;

    public Vector3 penetrationNormal;
    public float penetrationDepth;


    public Vector3 relativeVelocity;
    public float impulseMagnitude;

    public Vector3[] contactManifold;

    public float relVelDotNorm;


    public ValkyrieCollision(ValkyrieCollider a, ValkyrieCollider b, Vector3 penetrationNormal, float penetrationDepth)
    {
        colliders = new ValkyrieCollider[]
        {
            a, b
        };

        this.penetrationNormal = penetrationNormal;
        this.penetrationDepth = penetrationDepth;
    }

    public ValkyrieCollider GetOtherCollider(ValkyrieCollider col)
    {
        if(col == colliders[0]) return colliders[1];
        else if(col == colliders[1]) return colliders[0];
        
        return null;
    }


}
