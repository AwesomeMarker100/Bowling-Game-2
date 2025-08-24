using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GrabInteractor))]
public class GrabInteractorEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GrabInteractor gi = (GrabInteractor)target;


        if (GUILayout.Button("Set Location Details"))
        {

            gi.SetLocation();

        }

        if(GUILayout.Button("Reset Location of Editor Item"))
        {

            gi.ResetEditorItemPosition();

        }


    }

}
