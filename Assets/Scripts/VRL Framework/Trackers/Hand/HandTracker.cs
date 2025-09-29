using System;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;


/**
 * 
 * I'm going to touch myself
 */
public class HandTracker : MonoBehaviour
{

    public enum HandType { Left, Right }

    [Header("Basic Info")]
    [SerializeField] public HandType handType;
    [SerializeField] public bool useRigidbody = false;
    public bool isActive = true;

    [SerializeField] private RadixTracker radixTracker;

    [SerializeField] private bool followPosition = true;
    [SerializeField] private bool followRotation = true;
    [SerializeField] private bool followCollision = true;


    private InputDevice handTrackingDevice;


    [Header("Position Scale Values")]
    [SerializeField][Min(0)] private float positionXScale = 20f;
    [SerializeField][Min(0)] private float positionYScale = 15f;
    [SerializeField][Min(0)] private float positionZScale = 20f;

    [SerializeField] private Vector3 positionOffset;

    [Header("Joints")]
    [SerializeField] private Transform wristJoint;
    /// <summary>
    /// I'm here
    /// </summary>
    private Vector3 wristJointDiffVec;




    [Header("Rotation Offsets")]
    [SerializeField] private float leftRotationOffset = 180f;
    [SerializeField] private float forwardRotationOffset = -90f;
    [SerializeField] private float upRotationOffset = 180f;

    [HideInInspector] private Vector3 trueHandPosition;
    [HideInInspector] private Vector3 gameHandPosition;

    [HideInInspector] private Quaternion handRotation;
    [HideInInspector] private Vector3 velocity;
    [HideInInspector] private Vector3 handAngularVelocity;
    [HideInInspector] private bool isGripButtonPressed;
    [HideInInspector] private bool isTriggerButtonPressed;

    private new ValkyrieCollider collider;
    private new ValkyrieRigidbody rigidbody;

    private VPhys physicsHandler;

    private bool savedInBetween = false;
    private Vector3 inBetween;
    private Quaternion originalRotation;
    private Vector3 originalPosition;

    // Start is called before the first frame update
    private void Start()
    {
        handTrackingDevice = handTrackingDevice = handType == HandType.Left ? VRLDeviceManager.GetLeftHandController() : VRLDeviceManager.GetRightHandController();
        collider = GetComponent<ValkyrieCollider>();
        rigidbody = GetComponent<ValkyrieRigidbody>();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        handTrackingDevice = handTrackingDevice = handType == HandType.Left ? VRLDeviceManager.GetLeftHandController() : VRLDeviceManager.GetRightHandController();

        if (handTrackingDevice.name is not null)
        {

            if (!radixTracker.freeze)
            {
                UpdateRealPosition();
                UpdateRealVelocity();

                UpdateRealAngularVelocity();
                UpdateRealRotation();

                UpdateTriggerPressed();
                UpdateGripButtonPressed();

                if (followPosition) SetGameHandPosition();
                if (followRotation) SetGameHandRotation();

            } 

        } 

    }


    //GAME VALUES
    #region
    private void SetGameHandPosition()
    {
        //this vector gets the position difference from the headset and controller in real life to accurately display it in game
        Vector3 inBetween = trueHandPosition - radixTracker.GetPosition();
        inBetween.x *= positionXScale;
        inBetween.y *= positionYScale;
        inBetween.z *= positionZScale;

        gameHandPosition = radixTracker.headCam.transform.localPosition + inBetween;
        transform.localPosition = gameHandPosition;
        

    }

    private void SetGameHandRotation()
    {
        handTrackingDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out handRotation);
        Quaternion rotationOffset = Quaternion.AngleAxis(leftRotationOffset, Vector3.left) * Quaternion.AngleAxis(upRotationOffset, Vector3.up) * Quaternion.AngleAxis(forwardRotationOffset, Vector3.forward);

        transform.localRotation = handRotation * rotationOffset;

    }
    #endregion

    public Vector3 GetGameHandPosition()
    {

        return gameHandPosition;

    }

    public Quaternion GetGameHandRotation()
    {

        return handRotation;

    }

    //SETTERS
    #region
    private void UpdateRealPosition()
    {
        handTrackingDevice.TryGetFeatureValue(CommonUsages.devicePosition, out trueHandPosition);

    }

    private void UpdateRealRotation()
    {

        if(!handTrackingDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out handRotation))
        {
            print("Could not retrieve device rotation");
            handRotation = Quaternion.identity;
        }

    }

    private void UpdateRealVelocity()
    {

        handTrackingDevice.TryGetFeatureValue(CommonUsages.deviceVelocity, out velocity);

    }

    private void UpdateRealAngularVelocity()
    {

        handTrackingDevice.TryGetFeatureValue(CommonUsages.deviceAngularVelocity, out handAngularVelocity);

    }

    private void UpdateGripButtonPressed()
    {

        handTrackingDevice.TryGetFeatureValue(CommonUsages.gripButton, out isGripButtonPressed);

    }

    private void UpdateTriggerPressed()
    {

        handTrackingDevice.TryGetFeatureValue(CommonUsages.triggerButton, out isTriggerButtonPressed);

    }

    #endregion

    //GETTERS
    #region
    public float GetGripButtonAmount()
    {

        float amount;
        handTrackingDevice.TryGetFeatureValue(CommonUsages.grip, out amount);

        return amount;

    }


    public float GetTriggerButtonAmount()
    {

        float amount;
        handTrackingDevice.TryGetFeatureValue(CommonUsages.trigger, out amount);

        return amount;

    }

    public float GetThumbButtonAmount()
    {

        bool press;
        handTrackingDevice.TryGetFeatureValue(CommonUsages.primaryButton, out press);

        if (press) return 1.0f;

        bool touch;
        handTrackingDevice.TryGetFeatureValue(CommonUsages.primaryTouch, out touch);

        if (touch) return 0.5f;

        return 0f;

    }

    public bool GetJoystickButtonPressed()
    {
        bool isJoystickPressed;
        handTrackingDevice.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out isJoystickPressed);

        return isJoystickPressed;
    }

    public Vector2 GetJoystickVelocity()
    {
        Vector2 joystickVel;
        handTrackingDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out joystickVel);

        return joystickVel;

    }

    public bool GetSecondaryButtonPressed()
    {

        bool pressed;
        handTrackingDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out pressed);

        return pressed;

    }

    public bool GetPrimaryButtonPressed()
    {

        bool pressed;
        handTrackingDevice.TryGetFeatureValue(CommonUsages.primaryButton, out pressed);

        return pressed;

    }

    public InputDevice GetHandTrackingDevice()
    {
        return handTrackingDevice;

    }

    #endregion

    //JOYSTICK GETTERS
    #region
    public bool IsJoystickGoingUp()
    {

        return GetJoystickVelocity().y > 0;

    }

    public bool IsJoystickGoingDown()
    {

        return GetJoystickVelocity().y < 0;


    }

    public bool IsJoystickGoingLeft()
    {

        return GetJoystickVelocity().x < 0;

    }

    public bool IsJoystickGoingRight()
    {

        return GetJoystickVelocity().x > 0;

    }
    #endregion

}
