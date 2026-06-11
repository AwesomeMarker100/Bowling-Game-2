using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PhysSim))]
public class PhysSimEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PhysSim physSim = (PhysSim)target;
        Record resetRec = physSim.resetPoint;


        if(GUILayout.Button("Save Reset Point"))
        {
            Transform trackPt = resetRec.trackedObjParentTransform;

            if(trackPt != null)
            {
                int numChildren = trackPt.childCount;
                if (numChildren == 0) return;

                resetRec.trackedObjInfo = new RecordedObject[numChildren];

                for(int i = 0; i < numChildren; i++)
                {
                    Transform child = trackPt.GetChild(i);
                    resetRec.trackedObjInfo[i] = new RecordedObject(child, child.position, child.rotation);
                }

                Debug.Log("Successfully saved tracked info!");
            }
            
        }
    }
}
