using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ValueAspect))]
public class ValueAspectPropertyDrawerUIE : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {

        return 500;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {


        SerializedProperty trackedComponent = property.FindPropertyRelative("trackedComponent");
        SerializedProperty trackedField = property.FindPropertyRelative("trackedFieldName");

        SerializedProperty posInput = property.FindPropertyRelative("positiveInput");
        SerializedProperty negInput = property.FindPropertyRelative("negativeInput");

        
        SerializedProperty inputIncrement = property.FindPropertyRelative("inputIncrement");

        EditorGUI.PropertyField(new Rect(position.x, position.y, 300, 30), trackedComponent);
        EditorGUI.PropertyField(new Rect(position.x, position.y + 30, 300, 30), trackedField);
        EditorGUI.PropertyField(new Rect(position.x, position.y + 60, 300, 40), posInput);
        EditorGUI.PropertyField(new Rect(position.x, position.y + 180, 300, 40), negInput);
        EditorGUI.PropertyField(new Rect(position.x, position.y + 220, 300, 40), inputIncrement);

    }
}
