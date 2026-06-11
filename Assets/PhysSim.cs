using System;
using UnityEditor.Recorder;
using UnityEditor.Recorder.Encoder;
using UnityEditor.Recorder.Input;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Recorder.MovieRecorderSettings;

public struct RecordedObject
{
    Transform objTransform;
    Vector3 position;
    Quaternion rotation;

    public RecordedObject(Transform objTransform, Vector3 position, Quaternion rotation)
    {
        this.objTransform = objTransform;
        this.position = position;
        this.rotation = rotation;
    }
}

[Serializable]
public struct Record
{
    public Transform trackedObjParentTransform;
    public RecordedObject[] trackedObjInfo;

    public Record(Transform trackedObjParentTransform)
    {
        this.trackedObjParentTransform = trackedObjParentTransform;
        trackedObjInfo = null;
    }
}


public class PhysSim : MonoBehaviour
{
    
    public Record resetPoint;
    

    [SerializeField] Camera[] cameras;
    private RecorderController controller;
    [SerializeField] Key recordKey;


    private bool isRecording = false;

    private void Start()
    {
        RecorderControllerSettings rcs = ScriptableObject.CreateInstance<RecorderControllerSettings>();

        MovieRecorderSettings mcs = ScriptableObject.CreateInstance<MovieRecorderSettings>();
        
        mcs.OutputFile = $"{Application.dataPath}/Recordings/PhysSim/SimTake_<Take>_1";

        mcs.ImageInputSettings = new CameraInputSettings()
        {
            Source = ImageSource.MainCamera,
            OutputWidth = 1920, 
            OutputHeight = 1080
        };

        mcs.EncoderSettings = new CoreEncoderSettings()
        {
            Codec = CoreEncoderSettings.OutputCodec.WEBM,
            EncodingProfile = CoreEncoderSettings.H264EncodingProfile.Main,
            EncodingQuality = CoreEncoderSettings.VideoEncodingQuality.Medium
        };

        mcs.Enabled = true;

        rcs.AddRecorderSettings(mcs);

        controller = new RecorderController(rcs);

    }

    private void Update()
    {
        if (Keyboard.current[recordKey].wasPressedThisFrame)
        {
            if(isRecording)
            {
                isRecording = false;
                StopRecording();

            } else
            {
                isRecording = true;
                if (controller == null) print("NULL BITSCH");
                StartRecording();
            }
        }
    }

    private void StartRecording()
    {
        print("Started recording!");
        controller.PrepareRecording();  // ← ADD THIS
        controller.StartRecording();
    }

    private void StopRecording()
    {
        print("Stop recording!");
        controller.StopRecording();
        
    }

    private void ResetBack()
    {

    }

}
