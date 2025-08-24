using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationController : MonoBehaviour
{
    public MovementEngine movementEngine;

    public virtual int GetPriority()
    {
        return 0;
    }

}
