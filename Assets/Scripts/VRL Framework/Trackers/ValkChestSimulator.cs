using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValkChestSimulator : MonoBehaviour
{

    private enum TransformType
    {

        Up, Forward, Right

    }

    private enum TrackingType
    {

        MedianController, PerpendicularController, Head

    }

    
    //SETTINGS

    [SerializeField] private TrackingType trackingType;
    [SerializeField] private TransformType forwardAxis = TransformType.Up;
    [SerializeField] private bool flipAxis = true;

    [Range(0, 1)][SerializeField] private float minControllerFaceAmount = 0.7f;


    [Header("Basic Tracking Settings")]
    [SerializeField] private float chestHeightOffset = -0.1f;
    [SerializeField] bool calculateForward = true;

    [Header("Perpendicular Tracking Settings")]
    [SerializeField] private bool conductDistanceCheck = true;
    [SerializeField] private bool conductAxesCheck = true;

    [SerializeField] private float maxControllerDistance = 2f;


    //TRACKED DEVICES

    private RadixTracker radixTracker;
    private HandTracker leftHand;
    private HandTracker rightHand;


    //COMPONENT INFO
    [HideInInspector] public Vector3 position;
    [HideInInspector] public Vector3 forward;

    private float flipFactor = 1f;




    // Start is called before the first frame update
    private void Start()
    {
        radixTracker = VRLGameObjectManager.instance.TryGetRadixTracker();
        leftHand = VRLGameObjectManager.instance.TryGetLeftHand();
        rightHand = VRLGameObjectManager.instance.TryGetRightHand();


        if (flipAxis) flipFactor *= -1f;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {

        SetPosition();
        if (calculateForward) SetForward();


    }


    //SETTERS
    #region

    private void SetPosition()
    {

        //find the position of the head

        position = radixTracker.headPosition;
        position.y += chestHeightOffset;

        transform.localPosition = position;

    }

    private void SetForward()
    {


        switch (trackingType)
        {
            
            case TrackingType.MedianController:


                UseMedianControllers();
                break;

        }

        //if we actually set the forward
        if(!forward.Equals(Vector3.zero)) transform.forward = forward;

    }

    #endregion

    //SPECIFIC SET FORWARD FUNCTIONS

    #region
    private void UseMedianControllers()
    {

        forward = Vector3.zero;

        if (!leftHand.isActive && !rightHand.isActive) return;

        if (!leftHand.isActive) //left hand not active, use rightHand
        {
            forward = flipFactor * rightHand.transform.forward;

        }
        else if (!rightHand.isActive) //right hand not active
        {

            forward = flipFactor * leftHand.transform.forward;

        }
        else //both hands are active
        {
            //get leftHandForward
            Vector3 leftHandForward = leftHand.transform.forward;
            leftHandForward.y = 0;


            //getRightHandForward
            Vector3 rightHandForward = rightHand.transform.forward;
            rightHandForward.y = 0;


            //check if left and right hand are facing the same way, and if they are then take the averages of both and set the forward to that
            if (IsFacingSameWay(leftHand.transform, rightHand.transform, minControllerFaceAmount)) forward = flipFactor * (leftHandForward + rightHandForward);

        }

        //chest shouldn't have any y
        forward.y = 0;

    }

    private void UseHead()
    {


    }

    private void UsePerpendicularControllers()
    {


    }

    #endregion


    //check if two transforms are facing the same way
    private bool IsFacingSameWay(Transform target, Transform reference, float acceptableMin)
    {

        //check both the forward and up axes 
        bool hasSimilarForward = Vector3.Dot(reference.forward, target.forward) > acceptableMin;
        bool hasSimilarUp = Vector3.Dot(reference.up, target.up) > acceptableMin;

        //compare forward and up vectors to see if two transforms are facing the same way
        return hasSimilarForward && hasSimilarUp;

    }
}









/*
 * 
 * 
 * 
 *  case TrackingType.PerpendicularController:

                if(rightHand.isActive && leftHand.isActive) //left and right hand are both active
                {

                    //distance is too great or not facing the same way - can't predict chest location
                    if (conductDistanceCheck && Vector3.Distance(leftHand.GetGameHandPosition(), rightHand.GetGameHandPosition()) > maxControllerDistance)
                    {

                        if (Vector3.Distance(leftHand.GetGameHandPosition(), rightHand.GetGameHandPosition()) > maxControllerDistance || !IsFacingSameWay(leftHand.transform, rightHand.transform, minControllerFaceAmount)) return;

                    }
                    Vector3 diffVec = rightHand.GetGameHandPosition() - leftHand.GetGameHandPosition(); //get difference vector between both controllers
                    forward = Vector3.Cross(diffVec, Vector3.up); //get perpendicular vector from difference Vector

                }

                break;
*/
