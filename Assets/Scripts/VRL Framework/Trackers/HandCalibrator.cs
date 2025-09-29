using UnityEngine;

using Control = ControllerInput.Control;

public class HandCalibrator : MonoBehaviour
{
    [SerializeField] HandTracker hand;

    [Tooltip("Enter Calibration Mode")]
    
    [SerializeField] ControllerInput startCalibration;
    [SerializeField] float translationSpeed;
    [SerializeField] float rotationSpeed;

    private Transform handTransform;

    //Movement Controls
    #region
    [Tooltip("Translate Hand Controls")]
    //Translation
    [SerializeField] ControllerInput translateUp = new ControllerInput(new Control[]{Control.RightJoystickUp});
    [SerializeField] ControllerInput translateDown = new ControllerInput(new Control[] { Control.RightJoystickDown});

    [SerializeField] ControllerInput translateLeft = new ControllerInput(new Control[] { Control.RightJoystickLeft});
    [SerializeField] ControllerInput translateRight = new ControllerInput(new Control[] { Control.RightJoystickRight});
    [SerializeField] ControllerInput translateForward = new ControllerInput(new Control[] { Control.RightJoystickPress, Control.RightJoystickUp});
    [SerializeField] ControllerInput translateBackward = new ControllerInput(new Control[]{ Control.RightJoystickPress, Control.RightJoystickDown});

    //Rotation
    [Tooltip("Rotate Hand Controls")]
    [SerializeField] ControllerInput yawLeft = new ControllerInput(new Control[] { Control.RightTriggerHold, Control.RightJoystickLeft});
    [SerializeField] ControllerInput yawRight = new ControllerInput(new Control[] { Control.RightTriggerHold, Control.RightJoystickRight});
    [SerializeField] ControllerInput pitchUp = new ControllerInput(new Control[] { Control.RightTriggerHold, Control.RightJoystickUp});
    [SerializeField] ControllerInput pitchDown = new ControllerInput(new Control[] { Control.RightTriggerHold, Control.RightJoystickDown });
    [SerializeField] ControllerInput rollLeft = new ControllerInput(new Control[] { Control.RightGripHold, Control.RightJoystickLeft });
    [SerializeField] ControllerInput rollRight = new ControllerInput(new Control[] { Control.RightGripHold, Control.RightJoystickRight });

    #endregion


    //Events
    #region
    private ControllerInputEvent startEvent;

    //Translation Events
    #region
    private ControllerInputEvent translateLeftEvt;
    private ControllerInputEvent translateRightEvt;
    private ControllerInputEvent translateUpEvt;
    private ControllerInputEvent translateDownEvt;
    private ControllerInputEvent translateForwardEvt;
    private ControllerInputEvent translateBackEvt;
    #endregion
    //Rotation Events (Pitch, Yaw, Roll)
    #region
    private ControllerInputEvent pitchUpEvt;
    private ControllerInputEvent pitchDownEvt;
    private ControllerInputEvent rollLeftEvt;
    private ControllerInputEvent rollRightEvt;
    private ControllerInputEvent yawLeftEvt;
    private ControllerInputEvent yawRightEvt;
    #endregion
    #endregion

    [SerializeField] bool inCalibrationMode = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        handTransform = hand ? hand.transform : null;

        InstantiateEvents();
        SetInputEvents();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Input Event Setup
    #region
    private void InstantiateEvents()
    {
        startEvent = OculusInputManager.GetInputEvent(startCalibration);
        yawLeftEvt = OculusInputManager.GetInputEvent(yawLeft);
        yawRightEvt = OculusInputManager.GetInputEvent(yawRight);
        pitchUpEvt = OculusInputManager.GetInputEvent(pitchUp);
        pitchDownEvt = OculusInputManager.GetInputEvent(pitchDown);
        rollLeftEvt = OculusInputManager.GetInputEvent(rollLeft);
        rollRightEvt = OculusInputManager.GetInputEvent(rollRight);

        translateLeftEvt = OculusInputManager.GetInputEvent(translateLeft);
        translateRightEvt = OculusInputManager.GetInputEvent(translateRight);
        translateUpEvt = OculusInputManager.GetInputEvent(translateUp);


        translateDownEvt = OculusInputManager.GetInputEvent(translateDown);
        translateForwardEvt = OculusInputManager.GetInputEvent(translateForward);
        translateBackEvt = OculusInputManager.GetInputEvent(translateBackward);
    }

    private void SetInputEvents()
    {
        startEvent.AddListener(StartCalibration);
        
        yawLeftEvt.AddListener(YawLeft);
        yawRightEvt.AddListener(YawRight);
        pitchUpEvt.AddListener(PitchUp);
        pitchDownEvt.AddListener(PitchDown);
        rollLeftEvt.AddListener(RollLeft);
        rollRightEvt.AddListener(RollRight);

        translateForwardEvt.AddListener(TranslateForward);
        translateDownEvt.AddListener(TranslateDown);
        translateBackEvt.AddListener(TranslateBackward);
        translateUpEvt.AddListener(TranslateUp);
        translateRightEvt.AddListener(TranslateRight);
        translateLeftEvt.AddListener(TranslateLeft);
        
    }

    #endregion

    private void StartCalibration(ControllerInputInfo info)
    {

        if (inCalibrationMode)
        {
            inCalibrationMode = false;
            return;
        }

        if(!hand)
        {
            print("Hand not assigned!");
            return;
        }

        print("here");
        hand.isActive = false;
        inCalibrationMode = true;
    }


    //Translation Events
    #region
    private void TranslateLeft(ControllerInputInfo info)
    {
        if (!inCalibrationMode) return;
        handTransform.transform.position += Vector3.left * info.GetAmountOfPress() * translationSpeed;
    }

    private void TranslateRight(ControllerInputInfo info)
    {
        if (!inCalibrationMode) return;
        handTransform.transform.position += Vector3.right * info.GetAmountOfPress() * translationSpeed;

    }

    private void TranslateUp(ControllerInputInfo info)
    {
        if (!inCalibrationMode) return;
        handTransform.transform.position += Vector3.up * info.GetAmountOfPress() * translationSpeed;

    }

    private void TranslateDown(ControllerInputInfo info)
    {
        if (!inCalibrationMode) return;
        handTransform.transform.position += Vector3.down * info.GetAmountOfPress() * translationSpeed;

    }

    private void TranslateForward(ControllerInputInfo info)
    {
        if (!inCalibrationMode) return;
        handTransform.transform.position += Vector3.forward * info.GetAmountOfPress() * translationSpeed;


    }

    private void TranslateBackward(ControllerInputInfo info)
    {
        if (!inCalibrationMode) return;
        handTransform.transform.position += Vector3.back * info.GetAmountOfPress() * translationSpeed;
    }

    #endregion

    //Rotation Events
    #region
    private void YawLeft(ControllerInputInfo info)
    {
        if (!inCalibrationMode) return;

        handTransform.rotation *= Quaternion.AngleAxis(info.GetAmountOfPress() * rotationSpeed, Vector3.up);

    }

    private void YawRight(ControllerInputInfo info)
    {
        if (!inCalibrationMode) return;

        handTransform.rotation *= Quaternion.AngleAxis(info.GetAmountOfPress() * rotationSpeed, Vector3.down);

    }

    private void PitchUp(ControllerInputInfo info)
    {
        if (!inCalibrationMode) return;

        handTransform.rotation *= Quaternion.AngleAxis(info.GetAmountOfPress() * rotationSpeed, Vector3.left);

    }

 

    private void PitchDown(ControllerInputInfo info)
    {
        if (!inCalibrationMode) return;

        handTransform.rotation *= Quaternion.AngleAxis(info.GetAmountOfPress() * rotationSpeed, Vector3.right);

    }

    private void RollLeft(ControllerInputInfo info)
    {
        if (!inCalibrationMode) return;

        handTransform.rotation *= Quaternion.AngleAxis(info.GetAmountOfPress() * rotationSpeed, Vector3.forward);

    }

    private void RollRight(ControllerInputInfo info)
    {
        if (!inCalibrationMode) return;

        handTransform.rotation *= Quaternion.AngleAxis(info.GetAmountOfPress() * rotationSpeed, Vector3.back);

    }

    #endregion

}
