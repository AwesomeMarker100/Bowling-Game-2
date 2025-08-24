using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Unity.XR;

public class OculusController : MonoBehaviour
{

    [SerializeField] private HandTracker handTracker;
    [SerializeField] private HandPoser handPoser;
    [SerializeField] private float blinkTime = 5f;

    private InputDevice handTrackingDevice;

    private bool primaryTouch;
    private bool joystickTouch;
    private bool secondaryTouch;

    private InputFeatureUsage thumb;
    private Animator animator;

    

    // Start is called before the first frame update
    void Start()
    {
        handTrackingDevice = handTracker.GetHandTrackingDevice();
        animator = GetComponent<Animator>();
        
    }

    // Update is called once per frame
    void Update()
    {
        SetJoystickTouch();
        SetPrimaryButtonTouch();
        SetSecondaryButtonTouch();

        //MOVE MIDDLE
        animator.SetFloat("Grip", handTracker.GetGripButtonAmount());


        //MOVE INDEX

        

        animator.SetFloat("Trigger", handTracker.GetTriggerButtonAmount());

        if(handTracker.GetTriggerButtonAmount() > 0.01f)
        {
            transform.Find("b_trigger_front").Find("Indicator").gameObject.GetComponent<Light>().color = Color.red;

            transform.Find("b_trigger_front").Find("Indicator").gameObject.SetActive(true);

        } else
        {
            transform.Find("b_trigger_front").Find("Indicator").gameObject.SetActive(false);


        }

        if(handTracker.GetGripButtonAmount() > 0.01f)
        {
            transform.Find("b_trigger_grip").Find("Indicator").gameObject.GetComponent<Light>().color = Color.red;

            transform.Find("b_trigger_grip").Find("Indicator").gameObject.SetActive(true);


        } else
        {
            transform.Find("b_trigger_grip").Find("Indicator").gameObject.SetActive(false);


        }


        Vector2 joystickVel = handTracker.GetJoystickVelocity();

        animator.SetBool("PrimaryPress", handTracker.GetPrimaryButtonPressed());



        animator.SetBool("SecondaryPress", handTracker.GetSecondaryButtonPressed());
        animator.SetBool("JoystickPress", handTracker.GetJoystickButtonPressed());

        animator.SetFloat("JoystickUp", joystickVel.y);
        animator.SetFloat("JoystickRight", joystickVel.x);

        if (handTracker.GetPrimaryButtonPressed())
        {
            transform.Find("b_button_a").Find("Indicator").gameObject.GetComponent<Light>().color = Color.red;

            transform.Find("b_button_a").Find("Indicator").gameObject.SetActive(true);

        } else
        {
            transform.Find("b_button_a").Find("Indicator").gameObject.SetActive(false);

        }

        if (handTracker.GetSecondaryButtonPressed())
        {
            transform.Find("b_button_b").Find("Indicator").gameObject.GetComponent<Light>().color = Color.red;

            transform.Find("b_button_b").Find("Indicator").gameObject.SetActive(true);


        } else
        {

            transform.Find("b_button_b").Find("Indicator").gameObject.SetActive(false);

        }

        if (joystickVel.magnitude > 0 || handTracker.GetJoystickButtonPressed())
        {
            transform.Find("b_thumbstick").Find("Indicator").gameObject.GetComponent<Light>().color = Color.red;
            transform.Find("b_thumbstick").Find("Indicator").gameObject.SetActive(true);


        } else
        {
            transform.Find("b_thumbstick").Find("Indicator").gameObject.SetActive(false);

        }
    }

   

    public void SetJoystickTouch()
    {

        handTrackingDevice.TryGetFeatureValue(CommonUsages.primary2DAxisTouch, out joystickTouch);

    }

    public void SetPrimaryButtonTouch()
    {
        
        handTrackingDevice.TryGetFeatureValue(CommonUsages.primaryTouch, out primaryTouch);

    }
    
    public void SetSecondaryButtonTouch()
    {


        handTrackingDevice.TryGetFeatureValue(CommonUsages.secondaryTouch, out secondaryTouch);

    }

    public void Toggle()
    {

        this.gameObject.SetActive(!this.gameObject.activeInHierarchy);

    }

    public IEnumerator Blink(Light indicator)
    {


        indicator.color = Color.green;
        indicator.gameObject.SetActive(true);

        yield return new WaitForSeconds(5f);

        indicator.color = Color.red;
        indicator.gameObject.SetActive(false);

    }


}
