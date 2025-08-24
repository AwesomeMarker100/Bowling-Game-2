using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static ControllerInput;
public class SwingMovementController : MovementController
{


    /*
     * 
     * EXPLANATION
     * 
     * The SwingWalk() function itself will call when the input is pressed for either controller
     * It will keep track of what controller called the function(left or right) and mark its conditions met if it meets the necessary stride time
     * The Update() method is what actually tracks and sends the movement message to the MovementEngine
     * If only one controller is available, it will account for that and just check if the conditions are met
     * However, if both controllers are used - it will check for all SyncConditions to be met such as if the controllers are on opposite sides of the passthrough point
     * 
     * 
     */


    public enum PassthroughPoint { Headset, Chest, None }
    public enum PassthroughAlignment {   Front, Back }

    public enum SyncCondition
    {
        None,
        OppositePositions
    }


    [Header("Hand Trackers and Head")]
    [SerializeField] private HandTracker leftHand;
    [SerializeField] private HandTracker rightHand;
    [SerializeField] private ValkChestSimulator chestSimulator;
    [SerializeField] private RadixTracker radixTracker;


    [Header("Inputs")]
    [SerializeField] private ControllerInput leftHandWalkInput = new ControllerInput();
    [SerializeField] private ControllerInput rightHandWalkInput = new ControllerInput();

    

    [Header("Walk Settings")]
    [SerializeField] private List<SyncCondition> syncConditions;
    [SerializeField] public PassthroughPoint passthroughPoint = PassthroughPoint.Chest;

    [SerializeField] private float syncDelayTime = 0.5f;
    [SerializeField] private float leftMaximumStrideTime = 3f;
    [SerializeField] private float rightMaximumStrideTime = 3f;


    private PassthroughAlignment[] alignments = new PassthroughAlignment[2];
    private float[] alignmentChangeTimes = new float[2];
    private bool[] inputsPressed = new bool[2];
    private bool[] conditionssMet = new bool[2]; //better

    private float lastSyncTime = 0;

    //PASSTHROUGH SETTINGS
    private Vector3 passthroughForward;
    private Vector3 passthroughPosition;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        chestSimulator = VRLGameObjectManager.instance.TryGetChestSimulator();



        OculusInputManager.SubscribeToEvent(leftHandWalkInput, SwingWalk);
        OculusInputManager.SubscribeToEvent(rightHandWalkInput, SwingWalk);

        
        OculusInputManager.GetCorrespondingPressEvent(leftHandWalkInput.controls[0]).AddListener(StopSwingWalk);
        OculusInputManager.GetCorrespondingPressEvent(rightHandWalkInput.controls[0]).AddListener(StopSwingWalk);


    }

    


    //ONLY CALLS WHEN WALK BUTTON IS HELD -- THIS FUNCTION IS JUST USED TO SET CONDITIONSMET -- THE ACTUAL MOVEMENT OCCURS IN THE UPDATE LOOP
    private void SwingWalk(ControllerInputInfo inputInfo)
    {
        if (!isActive) return;


        base.Move(inputInfo); //set inputPressed true
        HandTracker ht = inputInfo.handTracker;

        //LEFT HAND IS 0, RIGHT HAND IS 1

        int i = ht == leftHand ? 0 : 1; //gotta love the one-liners
        float maxStrideTime = ht == leftHand ? leftMaximumStrideTime : rightMaximumStrideTime;

        //set the inputsPressed for this controller to true
        inputsPressed[i] = true;


        if (GetPassthroughAlignment(ht) != alignments[i]) //did we switch up alignment to passthrough point and if we're not a slow ass
        {

            if (Time.time - alignmentChangeTimes[i] <= maxStrideTime) //we changed alignment regularly and have fit the right time
            {

                conditionssMet[i] = true;
            }
            else //we did but failed to meet the time requirement
            {
                conditionssMet[i] = false;
            }

            //set the alignment change time
            alignmentChangeTimes[i] = Time.time;

        }
        else
        {

            //if we haven't changed our alignment for this controller and its past the max time then call it quits

            if (Time.time - alignmentChangeTimes[i] > maxStrideTime)
            {
                conditionssMet[i] = false;

            }

        }

        alignments[i] = GetPassthroughAlignment(ht);

    }

    private void StopSwingWalk(ControllerInputInfo inputInfo)
    {
        //if we're not walking, set inputsPressed false and change our time to -1
        int i = inputInfo.handTracker == leftHand ? 0 : 1;

        inputsPressed[i] = false;
        alignmentChangeTimes[i] = -1;
        conditionssMet[i] = false;
    }


    private void Update()
    {
        SetPassthroughSettings();

        //index 0 is always left hand, index 1 is right hand

        if (isActive)
        {

            if (!leftHand.isActive && !rightHand.isActive)//no hands ;(
            {
                return;

            } else if(!leftHand.isActive)//no left hand
            {
                SwingOneHand(1);

            } else if (!rightHand.isActive)//no right hand
            {

                SwingOneHand(0);

            } else // both hands active
            {
                SwingBothHands();
                
            }

        } else
        {

            //chestSimulator.calculateForward = true;
        }

    }

    private void SetPassthroughSettings()
    {

        switch (passthroughPoint)
        {

            //if the passthrough point is the headset, set the passthroughposition to the headset and the forward of the headset
            case PassthroughPoint.Headset:
                passthroughPosition = radixTracker.headPosition;
                passthroughForward = radixTracker.GetForward();
                return;


            //if the passthrough point is the chest, use the chestSimulator position and the chestForward
            case PassthroughPoint.Chest:
                passthroughPosition = chestSimulator.position;
                passthroughForward = chestSimulator.forward;
                return;

            default:
                return;

        }

    }

    //ACTUAL MOVEMENT FUNCTIONS
    #region
    private void SwingOneHand(int handIndex)
    {
        //check input
        if (inputsPressed[handIndex])
        {
            //chestSimulator.calculateForward = false;

            if (conditionssMet[handIndex]) //check if conditions have been met
            {

                movementEngine.OverrideMovement(passthroughForward, speedMultiplier);
                 
            }


        } else
        {
            //chestSimulator.calculateForward = true;
        }

    }

    private void SwingBothHands()
    {
        //same thing as above but checking both inputs now
        if(inputsPressed[0] && inputsPressed[1])
        {
            //chestSimulator.calculateForward = false;

            if(conditionssMet[0] && conditionssMet[1])
            {

                if (IsSynced())
                {
                    
                    movementEngine.OverrideMovement(passthroughForward, speedMultiplier);


                }


            }

        } else
        {

            //chestSimulator.calculateForward = true;

        }

    }

    #endregion


    //SYNC FUNCTIONS

    #region
    private bool IsSynced()
    {
        //check all SyncConditions and check if they are met
        foreach (SyncCondition sc in syncConditions)
        {

            if (!IsSyncConditionMet(sc)) { //if the sync condition is not met, check if its still within the delay time

                if (Time.time - lastSyncTime <= syncDelayTime)
                {

                    return true;

                }

                //if sync conditions are not met and the user has failed to meet the syncDelayTime - stop the movement
                
                return false; 
            
            } 

        }
        
        lastSyncTime = Time.time;
        return true;

    }

    //takes a particular SyncCondition and cheks for it
    private bool IsSyncConditionMet(SyncCondition sc)
    {

        switch (sc)
        {
            //checks that controllers are at opposite positions of passthrough point
            case SyncCondition.OppositePositions:

                return CheckForOppositePosition();



            default:
                return false;

        }

    }

    #endregion

    private bool CheckForOppositePosition()
    {
        //make sure alignments are not similar (opposite)
        return alignments[0] != alignments[1];

    }


    private PassthroughAlignment GetPassthroughAlignment(HandTracker ht)
    {

        //gets the vector between the passthrough point and the current location of the game hand
        Vector3 diffVector = ht.GetGameHandPosition() - passthroughPosition;
        diffVector.y = 0f;

        float dotProd = Vector3.Dot(passthroughForward, diffVector.normalized);


        return dotProd > 0 ? PassthroughAlignment.Front : PassthroughAlignment.Back; //is the controller in front or back of the passthrough point

    }

}
