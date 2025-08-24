using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static ControllerInput;

[RequireComponent(typeof(VRLGameObjectManager))]
public class OculusInputManager : MonoBehaviour
{
    private HandTracker leftHandTracker;
    private HandTracker rightHandTracker;

    [SerializeField] private float triggerPressAmount = 0.98f; 
    [SerializeField] private float gripPressAmount = 0.98f;

    //CONTROLLER INPUT EVENTS -- EACH INPUT ON THE CONTROLLER CORRESPONDS TO ITS OWN EVENT THAT CAN TRIGGER ACTIONS TO HAPPEN IN-GAME
    #region
    private static ControllerInputEvent leftPrimaryButtonPress = new ControllerInputEvent();
    private static ControllerInputEvent rightPrimaryButtonPress = new ControllerInputEvent();

    private static ControllerInputEvent leftSecondaryButtonPress = new ControllerInputEvent();
    private static ControllerInputEvent rightSecondaryButtonPress = new ControllerInputEvent();

    private static ControllerInputEvent leftTriggerButtonPress = new ControllerInputEvent();
    private static ControllerInputEvent rightTriggerButtonPress = new ControllerInputEvent();
    private static ControllerInputEvent leftTriggerButtonHold = new ControllerInputEvent();
    private static ControllerInputEvent rightTriggerButtonHold = new ControllerInputEvent();

    private static ControllerInputEvent leftGripButtonPress = new ControllerInputEvent();
    private static ControllerInputEvent rightGripButtonPress = new ControllerInputEvent();
    private static ControllerInputEvent leftGripButtonHold = new ControllerInputEvent();
    private static ControllerInputEvent rightGripButtonHold = new ControllerInputEvent();


    private static ControllerInputEvent leftJoystickButtonPress = new ControllerInputEvent();
    private static ControllerInputEvent rightJoystickButtonPress = new ControllerInputEvent();

    private static ControllerInputEvent leftJoystickUp = new ControllerInputEvent();
    private static ControllerInputEvent rightJoystickUp = new ControllerInputEvent();

    private static ControllerInputEvent leftJoystickDown = new ControllerInputEvent();
    private static ControllerInputEvent rightJoystickDown = new ControllerInputEvent();

    private static ControllerInputEvent leftJoystickLeft = new ControllerInputEvent();
    private static ControllerInputEvent rightJoystickLeft = new ControllerInputEvent();

    private static ControllerInputEvent leftJoystickRight = new ControllerInputEvent();
    private static ControllerInputEvent rightJoystickRight = new ControllerInputEvent();

    #endregion
    
    //HOLD TIMES- MUST BE REFERENCE TYPES 
    #region
    private ControllerInputHoldTime leftPrimaryButtonHoldTime = new ControllerInputHoldTime();
    private ControllerInputHoldTime rightPrimaryButtonHoldTime = new ControllerInputHoldTime();  
    private ControllerInputHoldTime leftSecondaryButtonHoldTime = new ControllerInputHoldTime();
    private ControllerInputHoldTime rightSecondaryButtonHoldTime = new ControllerInputHoldTime();
    private ControllerInputHoldTime leftTriggerButtonHoldTime = new ControllerInputHoldTime();
    private ControllerInputHoldTime rightTriggerButtonHoldTime = new ControllerInputHoldTime();
    private ControllerInputHoldTime leftGripButtonHoldTime = new ControllerInputHoldTime();
    private ControllerInputHoldTime rightGripButtonHoldTime = new ControllerInputHoldTime();
    #endregion

    //COMPOUND INPUTS ARE TWO OR MORE INPUTS THAT ARE PRESSED AT THE SAME TIME -- WE NEED TO TRACK THESE SLIGHTLY DIFFERENTLY
    private static List<CompoundInput> compoundInputs = new List<CompoundInput>();

    //MAKES THIS MORE EFFICIENT SO WE'RE NOT CHECKING CONTROLS THAT DON'T ACTUALLY DO ANYTHING
    private static HashSet<Control> activeControls = new HashSet<Control>();


    //ESESNTIALLY WE 

    private static List<ControllerInputEvent> inputEvents = new List<ControllerInputEvent>();


    //USER IMPORTANT FUNCTIONS
    #region

    private void Start()
    {
        leftHandTracker = VRLGameObjectManager.instance.TryGetLeftHand();
        rightHandTracker = VRLGameObjectManager.instance.TryGetRightHand();


    }

    public static void SubscribeToEvent(ControllerInput input, UnityAction<ControllerInputInfo> function)
    {
        //make sure there is an actual control being binded to an event
        if (input.controls.Length > 0)
        {
            ControllerInputEvent inputEvent = GetInputEvent(input);
            inputEvents.Add(inputEvent);

           
            inputEvent.AddListener(function);

            
        }
    }
    #endregion

    //GETTING EVENTS
    #region
    public static ControllerInputEvent GetInputEvent(ControllerInput controllerInput)
    {

        if (controllerInput.controls.Length == 1) //if the input has one control
        {
            Control input = controllerInput.controls[0];
            activeControls.Add(input);

            return GetCorrespondingEvent(input);

            
        } else if(controllerInput.controls.Length > 1) //if the input has many controls
        {
            //WE STILL ADD TO ACTIVECONTROLS LIST

            return GetCompoundInput(controllerInput); //returns the compound input event

        } else //zero inputs given
        {

            Debug.LogWarning("One or more GameObjects has a field with zero controls attached to an input!");
            return null;

        }
        
    }

    public static ControllerInputEvent GetCorrespondingPressEvent(Control holdInput)
    {

        switch (holdInput)
        {
            case Control.LeftTriggerHold:
                return leftTriggerButtonPress;

            case Control.RightTriggerHold:
                return rightTriggerButtonPress;

            case Control.LeftGripHold:
                return leftGripButtonPress;

            case Control.RightGripHold:
                return rightGripButtonPress;

            default:
                return null;

        }

    }

    public static ControllerInputEvent GetCorrespondingEvent(Control input)
    {


        switch (input)
        {

            case Control.LeftPrimaryPress:
                return leftPrimaryButtonPress;

            case Control.RightPrimaryPress:
                return rightPrimaryButtonPress;

            case Control.LeftSecondaryPress:
                return leftSecondaryButtonPress;

            case Control.RightSecondaryPress:
                return rightSecondaryButtonPress;

            case Control.LeftTriggerPress:
                return leftTriggerButtonPress;

            case Control.LeftTriggerHold:
                return leftTriggerButtonHold;

            case Control.RightTriggerPress:
                return rightTriggerButtonPress;

            case Control.RightTriggerHold:
                return rightTriggerButtonHold;

            case Control.LeftGripPress:
                return leftGripButtonPress;

            case Control.LeftGripHold:
                return leftGripButtonHold;

            case Control.RightGripPress:
                return rightGripButtonPress;

            case Control.RightGripHold:
                return rightGripButtonHold;

            case Control.LeftJoystickPress:
                return leftJoystickButtonPress;

            case Control.RightJoystickPress:
                return rightJoystickButtonPress;

            case Control.LeftJoystickUp:
                return leftJoystickUp;

            case Control.RightJoystickUp:
                return rightJoystickUp;

            case Control.LeftJoystickDown:
                return leftJoystickDown;

            case Control.RightJoystickDown:
                return rightJoystickDown;

            case Control.LeftJoystickLeft:
                return leftJoystickLeft;

            case Control.RightJoystickLeft:
                return rightJoystickLeft;

            case Control.LeftJoystickRight:
                return leftJoystickRight;

            case Control.RightJoystickRight:
                return rightJoystickRight;


        }

        return null;

    }

    private static ControllerInputEvent GetCompoundInput(ControllerInput input)
    {
        //create a compound input instance, add it to our list
        CompoundInput compoundInput = new CompoundInput(FindObjectOfType<OculusInputManager>(), input);
        compoundInputs.Add(compoundInput);


        foreach (Control control in input.controls)
        {
            //add each control in the given input to the activeControls hashset so we can efficiently search for input presses that are being used
            activeControls.Add(control);

        }

        return compoundInput.GetCompoundInputEvent();

    }

    #endregion
    


    // Update is called once per frame
    private void FixedUpdate()
    {


        CheckLeftButtons();
        CheckRightButtons();

        CheckJoystickUpDown();
        CheckJoystickLeftRight();

        CheckTriggers();
        CheckGrips();

        CheckCompoundInputs();



    }






    //CHECK LEFT AND RIGHT BUTTONS
    #region
    private void CheckLeftButtons()
    {

        //if left hand connected
        if (leftHandTracker != null) 
        {
            //
            if (activeControls.Contains(Control.LeftPrimaryPress))
            {
                //If the user pressed it down and DID NOT let it up yet
                if (leftHandTracker.GetPrimaryButtonPressed())
                {

                    leftPrimaryButtonHoldTime.holdTime += Time.fixedDeltaTime;

                }
                else //either the user didn't press it at all or just released it
                {
                    //if the user just let the button go back up
                    if (leftPrimaryButtonHoldTime.holdTime != 0)
                    {
                        leftPrimaryButtonPress.Invoke(new ControllerInputInfo("LeftPrimaryPress", leftHandTracker, leftPrimaryButtonHoldTime));
                        leftPrimaryButtonHoldTime.holdTime = 0;
                    }

                }
            }


            if (activeControls.Contains(Control.LeftSecondaryPress))
            {
                if (leftHandTracker.GetSecondaryButtonPressed())
                {
                    leftSecondaryButtonHoldTime.holdTime += Time.fixedDeltaTime;

                }
                else
                {

                    if (leftSecondaryButtonHoldTime.holdTime != 0)
                    {
                        leftSecondaryButtonPress.Invoke(new ControllerInputInfo("LeftSecondaryPress", leftHandTracker, leftSecondaryButtonHoldTime));
                        leftSecondaryButtonHoldTime.holdTime = 0;

                    }

                }
            }

            if (activeControls.Contains(Control.LeftJoystickPress))
            {
                if (IsActive(Control.LeftJoystickPress))
                {
                    leftJoystickButtonPress.Invoke(new ControllerInputInfo("LeftJoystickPress", leftHandTracker, new ControllerInputHoldTime()));
                }
            }
        }

        
    }

    private void CheckRightButtons()
    {
        if (rightHandTracker != null)
        {
            if (activeControls.Contains(Control.RightPrimaryPress))
            {
                //LOOK AT CheckLeftButtons() for comment notes
                if (rightHandTracker.GetPrimaryButtonPressed())
                {
                    rightPrimaryButtonHoldTime.holdTime += Time.fixedDeltaTime;

                }
                else
                {
                    if (rightPrimaryButtonHoldTime.holdTime != 0)
                    {

                        rightPrimaryButtonPress.Invoke(new ControllerInputInfo("RightPrimaryPress", rightHandTracker, rightPrimaryButtonHoldTime));
                        rightPrimaryButtonHoldTime.holdTime = 0f;

                    }
                }
            }

            if (activeControls.Contains(Control.RightSecondaryPress))
            {

                if (rightHandTracker.GetSecondaryButtonPressed())
                {
                    rightSecondaryButtonHoldTime.holdTime += Time.fixedDeltaTime;

                }
                else
                {
                    if (rightSecondaryButtonHoldTime.holdTime != 0)
                    {
                        rightSecondaryButtonPress.Invoke(new ControllerInputInfo("RightSecondaryPress", rightHandTracker, rightSecondaryButtonHoldTime));
                        rightSecondaryButtonHoldTime.holdTime = 0f;

                    }

                }
            }

            if (activeControls.Contains(Control.RightJoystickPress))
            {

                if (rightHandTracker.GetJoystickButtonPressed()) rightJoystickButtonPress.Invoke(new ControllerInputInfo("RightJoystickPress", rightHandTracker, new ControllerInputHoldTime()));

            }
        }

    }
    #endregion

    //CHECK JOYSTICKS
    #region
    private void CheckJoystickUpDown()
    {


        if (leftHandTracker != null)
        {


            if (activeControls.Contains(Control.LeftJoystickUp))
            {

                if (IsActive(Control.LeftJoystickUp)) leftJoystickUp.Invoke(new ControllerInputInfo("LeftJoystickUp", leftHandTracker, new ControllerInputHoldTime()));

            }

            if (activeControls.Contains(Control.LeftJoystickDown))
            {

                if (IsActive(Control.LeftJoystickDown)) leftJoystickDown.Invoke(new ControllerInputInfo("LeftJoystickDown", leftHandTracker, new ControllerInputHoldTime()));


            }
        }

        if (rightHandTracker != null)
        {

            if (activeControls.Contains(Control.RightJoystickUp))
            {

                if (IsActive(Control.RightJoystickUp)) rightJoystickUp.Invoke(new ControllerInputInfo("RightJoystickUp", rightHandTracker, new ControllerInputHoldTime()));


            }

            if (activeControls.Contains(Control.RightJoystickDown))
            {

                if (IsActive(Control.RightJoystickDown)) rightJoystickDown.Invoke(new ControllerInputInfo("RightJoystickDown", rightHandTracker, new ControllerInputHoldTime()));

            }

        }

    }

    private void CheckJoystickLeftRight()
    {
        if (leftHandTracker != null)
        {
            if (activeControls.Contains(Control.LeftJoystickLeft)){

                if (IsActive(Control.LeftJoystickLeft)) leftJoystickLeft.Invoke(new ControllerInputInfo("LeftJoystickLeft", leftHandTracker, new ControllerInputHoldTime()));


            }

            if (activeControls.Contains(Control.LeftJoystickRight)) {

                if (IsActive(Control.LeftJoystickRight)) leftJoystickRight.Invoke(new ControllerInputInfo("LeftJoystickRight", leftHandTracker, new ControllerInputHoldTime()));

            }

        }

        if (rightHandTracker != null)
        {
            if (activeControls.Contains(Control.RightJoystickLeft))
            {

                if (IsActive(Control.RightJoystickLeft)) rightJoystickLeft.Invoke(new ControllerInputInfo("RightJoystickLeft", rightHandTracker, new ControllerInputHoldTime()));


            }

            if (activeControls.Contains(Control.RightJoystickRight))
            {

                
                if (IsActive(Control.RightJoystickRight)) rightJoystickRight.Invoke(new ControllerInputInfo("RightJoystickRight", rightHandTracker, new ControllerInputHoldTime()));


            }

        }

    }

    #endregion

    //CHECK TRIGGERS AND GRIPS
    #region
    private void CheckTriggers()
    {
        if (leftHandTracker != null)
        {
            //If we're tracking a press or hold
            if (activeControls.Contains(Control.LeftTriggerPress) || activeControls.Contains(Control.LeftTriggerHold))
            {
                    
                if (IsActive(Control.LeftTriggerHold))
                {
                    leftTriggerButtonHold.Invoke(new ControllerInputInfo("LeftTriggerHold", leftHandTracker, leftTriggerButtonHoldTime));
                    leftTriggerButtonHoldTime.holdTime += Time.fixedDeltaTime;


                } //IsActive() can differentiate between a press and hold
                else if(IsActive(Control.LeftTriggerPress))
                {

                    leftTriggerButtonPress.Invoke(new ControllerInputInfo("LeftTriggerPress", leftHandTracker, leftTriggerButtonHoldTime));
                    leftTriggerButtonHoldTime.holdTime = 0;

                }

            }

            
        }

        if(rightHandTracker != null)
        {
            if (activeControls.Contains(Control.RightTriggerPress) || activeControls.Contains(Control.RightTriggerHold))
            {
                if (IsActive(Control.RightTriggerHold))
                {
                    

                    rightTriggerButtonHold.Invoke(new ControllerInputInfo("RightTriggerHold", rightHandTracker, rightTriggerButtonHoldTime));
                    rightTriggerButtonHoldTime.holdTime += Time.fixedDeltaTime;

                }
                else if(IsActive(Control.RightTriggerPress))
                {

                    rightTriggerButtonPress.Invoke(new ControllerInputInfo("RightTriggerPress", rightHandTracker, rightTriggerButtonHoldTime));
                    rightTriggerButtonHoldTime.holdTime = 0f;
                }
            }

        }

    }

    private void CheckGrips()
    {

        if (leftHandTracker != null)
        {

            if (activeControls.Contains(Control.LeftGripPress) || activeControls.Contains(Control.LeftGripHold))
            {

                if (IsActive(Control.LeftGripHold))
                {

                    leftGripButtonHold.Invoke(new ControllerInputInfo("LeftGripHold", leftHandTracker, leftGripButtonHoldTime));
                    leftGripButtonHoldTime.holdTime += Time.fixedDeltaTime;


                }
                else if(IsActive(Control.LeftGripPress))
                {
                    leftGripButtonPress.Invoke(new ControllerInputInfo("LeftGripPress", leftHandTracker, leftGripButtonHoldTime));
                    leftGripButtonHoldTime.holdTime = 0f;


                }
            } 

        }

        if (rightHandTracker != null)
        {
            if (activeControls.Contains(Control.RightGripPress) || activeControls.Contains(Control.RightGripHold))
            {
                if (IsActive(Control.RightGripHold))
                {
                    rightGripButtonHold.Invoke(new ControllerInputInfo("RightGripHold", rightHandTracker, rightGripButtonHoldTime));
                    rightGripButtonHoldTime.holdTime += Time.fixedDeltaTime;

                }
                else if (IsActive(Control.RightGripPress))
                {
                    rightGripButtonPress.Invoke(new ControllerInputInfo("RightGripPress", rightHandTracker, rightGripButtonHoldTime));
                    rightGripButtonHoldTime.holdTime = 0f;
                }
            }
        
        }
    }

    #endregion

    //GIVEN A CONTROL - THIS FUNCTION WILL RETURN IF THAT CONTROL IS CURRENTLY ACTIVE(if a button is pressed or trigger held)
    #region
    public bool IsActive(Control input)
    {
        switch (input)
        {

            case Control.LeftPrimaryPress:
                return leftHandTracker.GetPrimaryButtonPressed();

            case Control.RightPrimaryPress:
                return rightHandTracker.GetPrimaryButtonPressed();

            case Control.LeftSecondaryPress:
                return leftHandTracker.GetSecondaryButtonPressed();

            case Control.RightSecondaryPress:
                return rightHandTracker.GetSecondaryButtonPressed();

            case Control.LeftTriggerPress:
                return leftHandTracker.GetTriggerButtonAmount() < triggerPressAmount && leftTriggerButtonHoldTime.holdTime != 0;

            case Control.LeftTriggerHold:
                return leftHandTracker.GetTriggerButtonAmount() >= triggerPressAmount;

            case Control.RightTriggerPress:
                return rightHandTracker.GetTriggerButtonAmount() < triggerPressAmount && rightTriggerButtonHoldTime.holdTime != 0;

            case Control.RightTriggerHold:
                return rightHandTracker.GetTriggerButtonAmount() >= triggerPressAmount;

            case Control.LeftGripPress:
                return leftHandTracker.GetGripButtonAmount() < gripPressAmount && leftGripButtonHoldTime.holdTime != 0;

            case Control.LeftGripHold:
                return leftHandTracker.GetGripButtonAmount() >= gripPressAmount;

            case Control.RightGripPress:
                return rightHandTracker.GetGripButtonAmount() < gripPressAmount && rightGripButtonHoldTime.holdTime != 0;

            case Control.RightGripHold:
                return rightHandTracker.GetGripButtonAmount() >= gripPressAmount;

            case Control.LeftJoystickPress:
                return leftHandTracker.GetJoystickButtonPressed();

            case Control.RightJoystickPress:
                return rightHandTracker.GetJoystickButtonPressed();

            case Control.LeftJoystickUp:
                return leftHandTracker.IsJoystickGoingUp();

            case Control.RightJoystickUp:
                return rightHandTracker.IsJoystickGoingUp();

            case Control.LeftJoystickDown:
                return leftHandTracker.IsJoystickGoingDown();

            case Control.RightJoystickDown:
                return rightHandTracker.IsJoystickGoingDown();

            case Control.LeftJoystickLeft:
                return leftHandTracker.IsJoystickGoingLeft();

            case Control.RightJoystickLeft:
                return rightHandTracker.IsJoystickGoingLeft();

            case Control.LeftJoystickRight:
                return leftHandTracker.IsJoystickGoingRight();

            case Control.RightJoystickRight:
                return rightHandTracker.IsJoystickGoingRight();

            default:
                return false;

        }
        

    }
    #endregion

    //CHECK COMPOUND INPUTS(MORE THAN ONE CONTROL)
    #region
    private void CheckCompoundInputs()
    {

        //iterate over 
        foreach (CompoundInput compoundInput in compoundInputs)
        {
            compoundInput.CheckInputs();

        }

    }
    #endregion
}
