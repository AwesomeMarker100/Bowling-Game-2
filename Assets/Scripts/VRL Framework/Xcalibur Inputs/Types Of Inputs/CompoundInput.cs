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

    private float acceptableDelay = 0.5f;

    private OculusInputManager inputManager;

    private HandTracker handTracker;
    bool[] activeControls;

    
    public CompoundInput(OculusInputManager inputManager, ControllerInput input, float acceptableDelay)
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
        activeControls = new bool[controls.Length];

        if (sameHand) handTracker = VRLGameObjectManager.instance.GetHandTracker(firstControlHandType); //if all of our inputs come from one hand, then set this handTracker to that hand
        

    }

    public bool AllInputsActive()
    {
        for(int i = 0; activeControls.Length > i; i++)
        {
            if (!activeControls[i]) return false;
        }

        return true;
    }

    public void ResetActiveControls()
    {
        for (int i = 0; activeControls.Length > i; i++)
        {
            activeControls[i] = false;
        }
    }


    public async Awaitable CheckInputs()
    {

        float curTime = 0; 
        while (!AllInputsActive() && curTime < acceptableDelay) {

            for (int i = 0; controls.Length > i; i++)
            {
                if (activeControls[i]) continue;
                if (inputManager.IsActive(controls[i])) activeControls[i] = true;

            }

            await Awaitable.FixedUpdateAsync();
            curTime += Time.fixedDeltaTime;
        }

        if (AllInputsActive()) {
            compoundInputEvent.Invoke(new ControllerInputInfo("CompoundInput", handTracker, null));
        }

        ResetActiveControls();
        
    }

    public ControllerInputEvent GetCompoundInputEvent()
    {

        return compoundInputEvent;

    }

}
