using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerInputInfo 
{
    public HandTracker handTracker;
    private string inputName = "";

    public ControllerInputHoldTime holdTime;

    //useful if grip or trigger
    private float amountOfPress;

    //constructor
    public ControllerInputInfo(string inputName, HandTracker handTracker, ControllerInputHoldTime holdtime)
    {
        this.inputName = inputName;
        this.handTracker = handTracker;
        this.holdTime = holdtime;
        this.amountOfPress = 1;
    }

    public ControllerInputInfo(string inputName, HandTracker handTracker, ControllerInputHoldTime holdTime, float amountOfPress)
    {

        this.inputName=inputName;
        this.handTracker = handTracker;
        this.holdTime = holdTime;
        this.amountOfPress = amountOfPress;
    }

    
    public float GetAmountOfPress()
    {
        return amountOfPress;
    }


}
