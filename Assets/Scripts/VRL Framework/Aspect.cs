using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

public class Aspect
{
    public MonoBehaviour trackedComponent;
    public string trackedFieldName;

    public FieldInfo trackedField;

   /* public Aspect()
    {

        trackedField = GetField();

    }*/
    
    public virtual FieldInfo GetField()
    {
        
        return trackedComponent.GetType().GetField(trackedFieldName);

    }


}
