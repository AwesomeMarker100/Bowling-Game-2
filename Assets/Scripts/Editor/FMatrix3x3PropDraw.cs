using UnityEngine;
using UnityEditor;
using Codice.Client.Common.GameUI;

[CustomPropertyDrawer(typeof(FMatrix3x3))]
public class FMatrix3x3PropDraw : PropertyDrawer
{
/*
    private float m00;
    private float m01;
    private float m02;

    private float m10;
    private float m11;
    private float m12;

    private float m20;
    private float m21;
    private float m22; 
    


    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 500;
    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty matArr = property.FindPropertyRelative("matArr");
        EditorGUI.PropertyField(new Rect(position.x, position.y + 30, 300, 30), matArr);
        
    }*/
}
