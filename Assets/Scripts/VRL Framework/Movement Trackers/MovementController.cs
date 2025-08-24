using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

using static ControllerInput;
public class MovementController : MonoBehaviour {

    [Header("Necessary Info")]
    [HideInInspector] public MovementEngine movementEngine;
    [SerializeField] public ControllerInput moveInput;

  /*  [SerializeField] AudioSource movementAudioSource;
    [SerializeField] AudioClip movementClip; */

    [SerializeField] public bool clampToGround = true;

    public float speedMultiplier = 10;

    public bool isActive = true;

    public TextMeshProUGUI showcaseText; 

    [HideInInspector] public bool inputPressed = false;
    [HideInInspector] public bool conditionsMet = false;

    public virtual int GetPriority() { return 0; }

    public virtual void Start()
    {
        OculusInputManager.SubscribeToEvent(moveInput, CheckIfActive);

        movementEngine = GetComponentInParent<MovementEngine>();

        if (movementEngine == null) movementEngine = FindObjectOfType<MovementEngine>();
    }

    

    private void CheckIfActive(ControllerInputInfo inputInfo) //base and derived class can see but no others
    {
        if (isActive)
        {
            Move(inputInfo);
        }
        
    }

    public virtual void Move(ControllerInputInfo inputInfo)
    {
        inputPressed = true;
    }

    public virtual void StopMove(ControllerInputInfo inputInfo)
    {
        if (isActive && inputPressed)
        {
            inputPressed = false;

        }

    }
}
