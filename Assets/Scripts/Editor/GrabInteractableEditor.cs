using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GrabInteractable))]
public class GrabInteractableEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GrabInteractable gi = target as GrabInteractable;

        if (GUILayout.Button("Set Loc, Rot, Scale"))
        {

            gi.SetLocalPosition(gi.transform.localPosition);
            gi.SetLocalRotation(gi.transform.localRotation);
            gi.SetLocalScale(gi.transform.localScale);

            EditorUtility.SetDirty(gi);

        }
    }

}
