using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SerQuaternion
{
    public float x;
    public float y;
    public float z;
    public float w;

    public SerQuaternion(float x, float y, float z, float w)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.w = w;

    }

    public void Set(Quaternion quaternion)
    {

        this.x = quaternion.x;
        this.y = quaternion.y; 
        this.z = quaternion.z;
        this.w = quaternion.w;

    }

    public void Set(float x, float y, float z, float w)
    {

        this.x = x;
        this.y = y;
        this.z = z;
        this.w = w;

    }

    public Quaternion ToQuaternion()
    {

        return new Quaternion(x, y, z, w);

    }

}
