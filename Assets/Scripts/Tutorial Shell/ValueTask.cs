using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class ValueTask : Task
{
    [SerializeField] MonoBehaviour trackedComponent;
    [Tooltip("Must be a numeric value type!")][SerializeField] string trackedFieldName = "";

    [Tooltip("Set the same if only one value is wanted")][SerializeField] float wantedValueMin;
    [SerializeField] float wantedValueMax;

    private FieldInfo fieldInfo;

    public override void Start()
    {
        base.Start();

        fieldInfo = trackedComponent.GetType().GetField(trackedFieldName);

    }

    public override bool IsTaskDone()
    {

        float value = (float)fieldInfo.GetValue(trackedComponent);

        return value >= wantedValueMin && value <= wantedValueMax;


    }
}
