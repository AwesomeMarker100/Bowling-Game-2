using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using static ControllerInput;
using static HandTracker;

public class CompoundInput
{

    public Control[] controls;
    private ControllerInputEvent compoundInputEvent = new ControllerInputEvent();
    private ControllerInputInfo controllerInputInfo;

    private OculusInputManager inputManager;

    private HandTracker handTracker;

    
    public CompoundInput(OculusInputManager inputManager, ControllerInput input)
    {
        this.inputManager = inputManager;
        SetInputs(input);
    }

    public void SetInputs(ControllerInput input) //while the class is singular, can hold multiple controls
    {
        //get controls, iterate through them and see if they all come from the same hand
        Control[] controls = input.controls;
        HandType firstControlHandType = GetHandType(controls[0]);
        bool sameHand = true;

        //we're just checking if all the inputs come from one hand or both
        for (int i = 1; controls.Length > i; i++)
        {
            if (!GetHandType(controls[i]).Equals(firstControlHandType)) {  sameHand = false; }

        }

        this.controls = input.controls;

        if (sameHand) handTracker = VRLGameObjectManager.instance.GetHandTracker(firstControlHandType); //if all of our inputs come from one hand, then set this handTracker to that hand
        

    }


    public void CheckInputs()
    {

        foreach (Control control in controls)
        {
            //Every control in this compound input must be active for us to call the event
            if (!inputManager.IsActive(control)) return;
            

        }
        compoundInputEvent.Invoke(new ControllerInputInfo("CompoundInput", handTracker, null));
        
    }

    public ControllerInputEvent GetCompoundInputEvent()
    {

        return compoundInputEvent;

    }

}
