using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(RadixTracker))]
public class RadixTrackerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        RadixTracker rt = (RadixTracker)target;

        if (GUILayout.Button("Reset Position"))
        {
            GameObject cam = GameObject.Find("Head Cam");

            //HandTracker leftHand = VRLGameObjectManager.instance.TryGetLeftHand();
            //HandTracker rightHand = VRLGameObjectManager.instance.TryGetRightHand();

            cam.transform.localPosition = new Vector3(0, rt.height, 0);


        }
    }
}
