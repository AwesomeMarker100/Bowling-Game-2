using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTeleportController : MovementController
{

    [SerializeField] private LaserInteractor laserPointer;


    public override void Move(ControllerInputInfo inputInfo)
    {

        base.Move(inputInfo);

        if (laserPointer.GetEndpointPosition() != Vector3.zero)
        {
            movementEngine.SetPosition(laserPointer.GetEndpointPosition());
        }
    }

}
