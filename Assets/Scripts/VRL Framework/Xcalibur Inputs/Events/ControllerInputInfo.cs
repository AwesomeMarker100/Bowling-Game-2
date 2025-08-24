using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerInputInfo 
{
    public HandTracker handTracker;
    public string inputName = "";

    public ControllerInputHoldTime holdTime;

    //useful if grip or trigger
    public float amountOfPress;

    //constructor
    public ControllerInputInfo(string inputName, HandTracker handTracker, ControllerInputHoldTime ciht)
    {
        this.inputName = inputName;
        this.handTracker = handTracker;
        holdTime = ciht;

    }

}
