using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BinaryAspect : Aspect
{

    public ControllerInput input;


    public BinaryAspect()
    {

    }

    public void ChangeValue(ControllerInputInfo inputInfo)
    {

        SetValue(!GetValue());

    }

    public bool GetValue()
    {

        return (bool)trackedField.GetValue(trackedComponent);

    }

    public void SetValue(bool val)
    {

        trackedField.SetValue(trackedComponent, val);

    }
}
