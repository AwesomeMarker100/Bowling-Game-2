using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[ExecuteAlways]
public class HandPoser : MonoBehaviour
{
    public enum GripType
    {
        MaxMinMethod,
        HalfPhysics,
        Physics
    }


    public GripType gripType = GripType.MaxMinMethod;

    [SerializeField] private HandTracker handTracker;
    private Animator handAnimator;

    public bool isControlled = false;
    public bool itemDisruption;

    public bool movementLocked = false;

    [Range(0, 1)]public float gripAmount;
    [Range(0, 1)]public float pointAmount;

    private float minimumGripAmount = 0f;
    private float maximumGripAmount = 1f;
    
    private float minimumPointAmount = 0f;
    private float maximumPointAmount = 1f;

    public UnityEvent belowGripAmount = new UnityEvent();
    public UnityEvent aboveGripAmount = new UnityEvent();
    public UnityEvent belowPointAmount = new UnityEvent();
    public UnityEvent abovePointAmount = new UnityEvent();

    // Start is called before the first frame update
    void Start()
    {
        

        handAnimator = GetComponent<Animator>();
        itemDisruption = false;

    }
    // Update is called once per frame
    void FixedUpdate()
    {

        if (Application.isPlaying)
        {
            gripAmount = handTracker.GetGripButtonAmount();
            pointAmount = handTracker.GetTriggerButtonAmount();

            CheckGripStatus();
            CheckPointStatus();

        } 
    }

    private void Update()
    {




            

    }


    private void CheckGripStatus()
    {
        if (gripAmount >= minimumGripAmount && gripAmount <= maximumGripAmount)
        {
            handAnimator.SetFloat("Grip", gripAmount);

        }
        else if (gripAmount < minimumGripAmount)
        {
            belowGripAmount.Invoke();

        }


    }

    private void CheckPointStatus()
    {
        if (!isControlled)
        {

            if (pointAmount >= minimumPointAmount && pointAmount <= maximumPointAmount)
            {

                handAnimator.SetFloat("Point", pointAmount);

            }
            else if (pointAmount < minimumPointAmount)
            {
                belowPointAmount.Invoke();

            }

        }

    }



    //SETTING MINIMUMS AND MAXIMUMS FOR GRIP AND POINT -- USED TO PREVENT CLIPPING

    #region
    public void SetMinimumGrip(float val)
    {

        minimumGripAmount = val;

    }

    public void SetMaximumGrip(float val)
    {

        maximumGripAmount = val;

    }

    public void SetMinimumPoint(float val)
    {

        minimumPointAmount = val;

    }

    public void SetMaximumPoint(float val)
    {

        maximumPointAmount = val;

    }

    public void SetControl(bool control)
    {

        this.isControlled = control;

    }

    #endregion

}
