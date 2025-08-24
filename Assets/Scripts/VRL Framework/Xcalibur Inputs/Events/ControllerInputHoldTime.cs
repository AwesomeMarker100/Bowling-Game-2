using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerInputHoldTime
{
    public float holdTime = 0f;

    public ControllerInputHoldTime()
    {


    }
    public ControllerInputHoldTime(float holdTime)
    {

        this.holdTime = holdTime;

    }

    public void SetHoldTime(float holdTime)
    {
        this.holdTime = holdTime;
    }

    public void IncreaseHoldTime(float increment)
    {

        this.holdTime += increment;

    }

    public void DecreaseHoldTime(float decrement)
    {

        this.holdTime -= decrement;

    }
}
