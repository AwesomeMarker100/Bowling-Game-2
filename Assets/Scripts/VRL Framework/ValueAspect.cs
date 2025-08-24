using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ValueAspect : Aspect
{

    public ControllerInput positiveInput;
    public ControllerInput negativeInput;

    public float inputIncrement = 3f;

    public float max = 10;
    public float min = 0.01f;

    /*public ValueAspect()
    {
        trackedField = GetField();

        if (trackedField != null)
        {

            if (trackedField.FieldType != null && trackedField.FieldType != typeof(float) || trackedField.FieldType != typeof(int) || trackedField.FieldType != typeof(double))
            {

                Debug.LogError("Value Aspect field type is not of an appropriate form!");

            }
            
        }

    }*/

    public ValueAspect(MonoBehaviour trackedComponent, string trackedFieldName, ControllerInput positiveInput, ControllerInput negativeInput, float inputIncrement)
    {
        this.trackedComponent = trackedComponent;
        this.trackedFieldName = trackedFieldName;
        this.positiveInput = positiveInput;
        this.negativeInput = negativeInput;
        this.inputIncrement = inputIncrement;


        trackedField = GetField();

    }

    public void Positive(ControllerInputInfo inputInfo)
    {
        if ((float)GetValue() + inputIncrement > max) return;

        SetValue((float)GetValue() + (float)inputIncrement);
    }

    public void Negative(ControllerInputInfo inputInfo)
    {

        if ((float)GetValue() - inputIncrement <= min) return;
        SetValue((float)GetValue() - (float)inputIncrement);

    }


    public ValueType GetValue()
    {

        return (ValueType)trackedField.GetValue(trackedComponent);

    }

    public void SetValue(ValueType value)
    {

        trackedField.SetValue(trackedComponent, value);

    }

}
