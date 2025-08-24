using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyMovementController : MovementController
{
    [Tooltip("Sets fly direction to forward vector of given transform.")][SerializeField] private Transform flyDirection;

    [SerializeField] private ControllerInput changeSpeedInput;

    [Header("Speed Settings")]
    [SerializeField] private float speedChangeInterval = 0.1f;
    [SerializeField] private float minSpeed = 1f;
    [SerializeField] private float maxSpeed = 15f;
    [SerializeField] private float m_Speed = 5f;


    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        OculusInputManager.SubscribeToEvent(changeSpeedInput, ChangeSpeed);

        m_Speed = Mathf.Clamp(m_Speed, minSpeed, maxSpeed);
    }

    public override void Move(ControllerInputInfo inputInfo)
    {
        base.Move(inputInfo);

        movementEngine.OverrideMovement(flyDirection.forward, m_Speed);
        
    }

    private void ChangeSpeed(ControllerInputInfo inputInfo)
    {
        if (isActive)
        {
            if (inputInfo.handTracker.GetJoystickVelocity().y > 0 && m_Speed < maxSpeed) //if joystick pressed down and velocity.y > 0, then let's go faster
            {
                m_Speed = Mathf.Clamp(m_Speed + speedChangeInterval, minSpeed, maxSpeed); //clamps current speed between min and max

            }
            else if (inputInfo.handTracker.GetJoystickVelocity().y < 0 && m_Speed > minSpeed) //if joystick pressed down and velocity.y < 0, then let's go slower
            {

                m_Speed = Mathf.Clamp(m_Speed - speedChangeInterval, minSpeed, maxSpeed);

            }
        }
    }
}
