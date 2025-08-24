using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SerVector3
{

    public float x;
    public float y;
    public float z;

    public SerVector3(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;

    }

    public void Set(Vector3 vector3)
    {

        this.x = vector3.x;
        this.y = vector3.y;
        this.z = vector3.z;

    }

    public void Set(float x, float y, float z)
    {

        this.x = x;
        this.y = y;
        this.z = z;

    }

    public Vector3 ToVector3()
    {

        return new Vector3(x,y,z);

    }

}
