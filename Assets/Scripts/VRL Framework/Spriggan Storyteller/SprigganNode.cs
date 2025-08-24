using System;
using UnityEngine;

[Serializable]
public class SprigganNode
{

    private bool isActive = false;
    public void SetActive()
    {
        isActive = true;
    }

    public void SetInactive()
    {
        isActive = false;
    }

    public bool IsActive()
    {
        return isActive;
    }

    public virtual async Awaitable Execute()
    {

    }

}
