using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputTask : Task
{

    [SerializeField] ControllerInput wantedInput;



    // Start is called before the first frame update
    public override void Start()
    {

    }

    public override bool IsTaskDone()
    {
        return base.IsTaskDone();
    }

}
