using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(BinaryAspect))]
public class BinaryAspectPropertyDrawerUIE : PropertyDrawer
{

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 500;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty trackedComponent = property.FindPropertyRelative("trackedComponent");
        SerializedProperty trackedField = property.FindPropertyRelative("trackedFieldName");
        SerializedProperty input = property.FindPropertyRelative("input");

        SerializedProperty inputIncrement = property.FindPropertyRelative("inputIncrement");

        EditorGUI.PropertyField(new Rect(position.x, position.y, 300, 15), trackedComponent);
        EditorGUI.PropertyField(new Rect(position.x, position.y + 20, 300, 15), trackedField);
        EditorGUI.PropertyField(new Rect(position.x, position.y + 40, 300, 15), input);
    }

}
