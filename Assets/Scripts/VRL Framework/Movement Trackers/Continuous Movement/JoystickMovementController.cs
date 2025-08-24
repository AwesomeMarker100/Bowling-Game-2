using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class JoystickMovementController : MovementController
{


    [Header("Walk Bounce Settings")]
    [SerializeField] private bool addBounce = true;
    [SerializeField] private float bounceAmount = 0.1f; //amplitude 
    [SerializeField] private float period = 0.5f; //period which is used to obtain b-val
    private const float tau = 2 * Mathf.PI; //useful
    
    private Vector3 movementVec;
    
    private Camera m_cam;
    private float origY; //clamps Y down, also used as vertical shift

    private void Start()
    {
        m_cam = VRLGameObjectManager.GetPlayerHeadCamera();

    }

    public override void Move(ControllerInputInfo inputInfo)
    {
        base.Move(inputInfo);
        Vector2 joystickVel = inputInfo.handTracker.GetJoystickVelocity();

        movementVec = joystickVel.x * m_cam.transform.right;
        movementVec += joystickVel.y * m_cam.transform.forward;

        if (addBounce) AddBounce();
        movementEngine.OverrideMovement(movementVec, speedMultiplier);
    }

    private void AddBounce()
    {
        //sine wave oscillation bounce-  from Booster game

        float newY = bounceAmount * Mathf.Sin(tau / period * Time.time);
        movementVec.y = newY;

    }


}
