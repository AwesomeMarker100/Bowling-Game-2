using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomButton : HittableObject
{
    public ControllerInput pressInput;
    public UnityEvent onPress;

   


    //CORNERS
    public override void Start()
    {

        base.Start();
        OculusInputManager.GetInputEvent(pressInput).AddListener(OnPress);


    }

    public override void ActivateHover()
    {
        base.ActivateHover();


    }

    public override void DeactivateHover()
    {
        base.DeactivateHover();

    }


    public void OnPress(ControllerInputInfo inputInfo)
    {
        if (isBeingHovered)
        {
            onPress.Invoke(); 
        }
    }
}
