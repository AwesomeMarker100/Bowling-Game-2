using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HandTracker;

[Serializable]
public class ControllerInput 
{

    public enum Control
    {
        LeftPrimaryPress,
        RightPrimaryPress,

        LeftSecondaryPress,
        RightSecondaryPress,

        LeftTriggerPress,
        LeftTriggerHold,

        RightTriggerPress,
        RightTriggerHold,

        LeftGripPress,
        LeftGripHold,

        RightGripPress,
        RightGripHold,

        LeftJoystickPress,
        RightJoystickPress,
        
        LeftJoystickUp,
        RightJoystickUp,

        LeftJoystickDown,
        RightJoystickDown,

        LeftJoystickLeft,
        RightJoystickLeft,

        LeftJoystickRight,
        RightJoystickRight,

        None
        
    }

    public Control[] controls = new Control[1];

    public ControllerInput()
    {

    }

    public ControllerInput(Control[] controls)
    {
        this.controls = controls;
    }

    public static HandType GetHandType(Control control)
    {

        return control.ToString().Contains("Left") ? HandType.Left : HandType.Right;

    }


}
