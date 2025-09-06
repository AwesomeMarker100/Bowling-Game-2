using UnityEngine;

using Control = ControllerInput.Control;

public class HandCalibrator : MonoBehaviour
{
    [SerializeField] HandTracker hand;

    [Tooltip("Enter Calibration Mode")]
    
    [SerializeField] ControllerInput startCalibration;

    //Movement Controls
    #region
    [Tooltip("Translate Hand Controls")]
    //Translation
    [SerializeField] ControllerInput translateHandUp = new ControllerInput(new Control[]{Control.RightJoystickUp});
    [SerializeField] ControllerInput translateHandDown = new ControllerInput(new Control[] { Control.RightJoystickDown});

    [SerializeField] ControllerInput translateHandLeft = new ControllerInput(new Control[] { Control.RightJoystickLeft});
    [SerializeField] ControllerInput translateHandRight = new ControllerInput(new Control[] { Control.RightJoystickRight});
    [SerializeField] ControllerInput translateHandForward = new ControllerInput(new Control[] { Control.RightJoystickPress, Control.RightJoystickUp});
    [SerializeField] ControllerInput translateHandBackward = new ControllerInput(new Control[]{ Control.RightJoystickPress, Control.RightJoystickDown});

    //Rotation
    [Tooltip("Rotate Hand Controls")]
    [SerializeField] ControllerInput yawLeft = new ControllerInput(new Control[] { Control.RightTriggerHold, Control.RightJoystickLeft});
    [SerializeField] ControllerInput yawRight = new ControllerInput(new Control[] { Control.RightTriggerHold, Control.RightJoystickRight});
    [SerializeField] ControllerInput pitchUp = new ControllerInput(new Control[] { Control.RightTriggerHold, Control.RightJoystickUp});
    [SerializeField] ControllerInput pitchDown = new ControllerInput(new Control[] { Control.RightTriggerHold, Control.RightJoystickDown });
    [SerializeField] ControllerInput rollLeft = new ControllerInput(new Control[] { Control.RightGripHold, Control.RightJoystickLeft });
    [SerializeField] ControllerInput rollRight = new ControllerInput(new Control[] { Control.RightGripHold, Control.RightJoystickRight });

    #endregion

    [SerializeField] bool inCalibrationMode = false;

    private OculusInputManager oculusInputManager;
    private ControllerInputEvent startEvent;
    private ControllerInputEvent calibrationInputs;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        oculusInputManager = FindFirstObjectByType<OculusInputManager>();
        startEvent = OculusInputManager.GetInputEvent(startCalibration);

        startEvent.AddListener(StartCalibration);
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    private void StartCalibration(ControllerInputInfo info)
    {

        if (inCalibrationMode) return;

        if(!hand)
        {
            print("Hand not assigned!");
            return;
        }

        print("here");
        hand.isActive = false;
        inCalibrationMode = true;
    }

    #region
    private void TranslateHandLeft(ControllerInputInfo info)
    {
           
    }

    private void TranslateHandRight(ControllerInputInfo info)
    {

    }

    private void TranslateHandUp(ControllerInputInfo info)
    {

    }

    private void TranslateHandDown(ControllerInputInfo info)
    {

    }
    #endregion
    private void 

}
