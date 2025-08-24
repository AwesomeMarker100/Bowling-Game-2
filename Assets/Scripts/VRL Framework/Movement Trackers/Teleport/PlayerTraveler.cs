using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTraveler : MovementController
{

    [SerializeField] private TeleportPoint[] teleportPoints;
    private int currentIdx = 0;

    public override void Move(ControllerInputInfo inputInfo)
    {
        base.Move(inputInfo);   

        if (currentIdx < teleportPoints.Length - 1)
        {

            currentIdx++;
            movementEngine.SetParent(null);

            Teleport();

        }
        else
        {

            currentIdx = 0;
            movementEngine.SetParent(null);

            Teleport();

        }

    }

    private void Teleport()
    {

        if (teleportPoints.Length != 0 && teleportPoints[currentIdx].openToTeleport) movementEngine.SetPosition(teleportPoints[currentIdx].transform.position);//moveTransform.position = teleportPoints[currentIdx].transform.position;
        if (teleportPoints[currentIdx].parentPlayer) movementEngine.SetParent(teleportPoints[currentIdx].transform);

    }

    
}
