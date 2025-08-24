using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AspectController : MonoBehaviour
{
    
    [Header("For Value Aspects")]

    public MonoBehaviour[] behaviors;
    public string[] trackedFieldNames;

    public float[] maxes;
    public float[] mins;

    public float[] incrementValues;

    public ControllerInput[] positiveInputs;
    public ControllerInput[] negativeInputs;

    private ValueAspect[] valueAspects;




    private void Start()
    {

        valueAspects = new ValueAspect[behaviors.Length];

        for (int i = 0; behaviors.Length > i; i++)
        {
            ValueAspect valueAspect = new ValueAspect(behaviors[i], trackedFieldNames[i], positiveInputs[i], negativeInputs[i], incrementValues[i]);

            valueAspect.trackedFieldName = trackedFieldNames[i];
            valueAspect.positiveInput = positiveInputs[i];
            valueAspect.negativeInput = negativeInputs[i];
            valueAspect.max = maxes[i];
            valueAspect.min = mins[i];

            valueAspects[i] = valueAspect;
            
            OculusInputManager.SubscribeToEvent(valueAspect.positiveInput, valueAspect.Positive);
            OculusInputManager.SubscribeToEvent(valueAspect.negativeInput, valueAspect.Negative);

        }


    }
}
