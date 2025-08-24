using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystickRotateController : RotationController
{

    

    [SerializeField] private HandTracker controller;
    [SerializeField] private float yawSpeed = 40f;
    [SerializeField] private float pitchSpeed = 40f;
    [SerializeField] private float rollSpeed = 40f;

    [SerializeField] private bool calculateRoll = true;

    public bool takesPriority = true;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 joystickVelocity = controller.GetJoystickVelocity();

        float roll = 0;
        float pitch = joystickVelocity.y * pitchSpeed * Time.fixedDeltaTime;
        float yaw = 0;

        if (controller.GetJoystickButtonPressed() && calculateRoll)
        {

            roll = joystickVelocity.x * rollSpeed * Time.fixedDeltaTime;

        }
        else
        {

            yaw = joystickVelocity.x * yawSpeed * Time.fixedDeltaTime;

        }

        //Roll on Z-axis, Pitch on X-axis, and Yaw on y axis

        Quaternion rotation = Quaternion.AngleAxis(-roll, Vector3.forward) * Quaternion.AngleAxis(-pitch, Vector3.right) * Quaternion.AngleAxis(yaw, Vector3.up);
        movementEngine.BlendRotation(rotation);
    }

    public override int GetPriority()
    {

        return 1;

    }


    private void OnDisable()
    {
    }
}
