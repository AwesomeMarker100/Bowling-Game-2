using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class RadixTracker : MonoBehaviour
{

    [HideInInspector] public bool isMoving = false;
    [HideInInspector] public bool isRotating = false;


    [Header("Editor")]
    [SerializeField] private float selectionSize = 60;

    [Header("Basic Settings")]
    [SerializeField] private bool trackMovement = true;
    [SerializeField] private bool trackRotation = true;
    [SerializeField] private float rotationSpeed = 3;

    [Header("Head Cam Settings")]
    [SerializeField] public Camera headCam;
    [Tooltip("In meters")][SerializeField] public float height = 1.8f;
    [Tooltip("How far is the head from the headset")] [SerializeField] private float headBackOffsetFactor = 0.05f;
    [SerializeField] public Vector3 rotationOffset = new Vector3();

    //IN-GAME
    [HideInInspector] public Vector3 headPosition;
    [HideInInspector] public Quaternion headRotation;

    private InputDevice headset;

    private void Start()
    {

        headset = VRLDeviceManager.GetHeadDevice();

    }

    private void FixedUpdate()
    {
        if (Application.isPlaying)
        {
            headset = VRLDeviceManager.GetHeadDevice();

            //TRACK HEAD ROTATION
            if (trackRotation) SetHeadRotation();

            //TRACK HEAD POSITION
            if (trackMovement) SetHeadPosition();
        }
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(headCam.transform.position, selectionSize);

    }

    //SET POSITION & ROTATION
    #region
    private void SetHeadPosition()
    {

        headCam.transform.localPosition = new Vector3(GetPosition().x, height, GetPosition().z);
        headPosition = headCam.transform.localPosition;

    }

    

    private void SetHeadRotation()
    {
        //need to apply a rotation offset or else it ends up all wonky


        headRotation = GetRotation();

        headCam.transform.localRotation = Quaternion.Slerp(headCam.transform.localRotation, Quaternion.Euler(rotationOffset) * headRotation, Time.fixedDeltaTime * rotationSpeed);

    }
    #endregion

    //GETTERS

    //THESE FUNCTIONS GET VALUES DIRECTLY FROM THE DEVICE

    #region
    public Vector3 GetForward()
    {
        //the forward for the radix tracker - what direction is the user looking
        return headCam.transform.forward;



    }
    
    public Vector3 GetPosition()
    {
        Vector3 pos;
        headset.TryGetFeatureValue(CommonUsages.devicePosition, out pos);

        return pos;


    }
    private Quaternion GetRotation()
    {
        Quaternion rot;
        headset.TryGetFeatureValue(CommonUsages.deviceRotation, out rot);

        return rot;

    }



    #endregion

    public void ResetPosition()
    {



    }
}
