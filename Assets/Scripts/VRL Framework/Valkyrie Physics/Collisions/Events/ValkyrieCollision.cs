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
    public Vector3 pointOfContact; 

    public ValkyrieCollision(ValkyrieCollider a, ValkyrieCollider b, Vector3 penetrationNormal, float penetrationDepth, Vector3 pointOfContact)
    {
        colliders = new ValkyrieCollider[]
        {
            a, b
        };

        this.penetrationNormal = penetrationNormal;
        this.penetrationDepth = penetrationDepth;
        this.pointOfContact = pointOfContact;
    }

    public ValkyrieCollider GetOtherCollider(ValkyrieCollider col) => col == colliders[0] ? colliders[1] : colliders[0];

    public override string ToString()
    {
        return $"Penetration Depth: {penetrationDepth}, Penetration Normal: {penetrationNormal}, Point Of Contact: {pointOfContact}";
    }

}
