using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ControllerInput))]
public class ControllerInputPropertyDrawerUIE : PropertyDrawer
{

    private float height;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SerializedProperty controls = property.FindPropertyRelative("controls");

        if (controls.isExpanded)
        {

            float height = 60; //dont start at 0 and add 60 because that'll look really weird at high numbers, experimenting- if you start at 60(title + first element) and then add
                                //20 for each element, it works out better

            float arraySize = controls.arraySize;

            //for each element, add another 30

            for (int i = 0; i < arraySize; i++)
            {

                height += 30;

            }


            this.height = height;
            return height;

        }

        return 40;
    }


    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
       

        //inputs array

        EditorGUI.BeginProperty(position, label, property);
        SerializedProperty controls = property.FindPropertyRelative("controls");

        Rect numOfInputsRect = new Rect(position.x, position.y, 300, 20);
        Rect propertyeRect = new Rect(position.x, position.y + 20, 300, height); //set to the height we got from each element


        GUIContent content = new GUIContent();
        content.text = "Inputs";

        //title
        EditorGUI.indentLevel = 0;
        EditorGUI.PropertyField(numOfInputsRect, property, label);

        //array
        EditorGUI.indentLevel = 1;
        EditorGUI.PropertyField(propertyeRect, controls, content);


        EditorGUI.EndProperty();

    }

        

}
#endif


