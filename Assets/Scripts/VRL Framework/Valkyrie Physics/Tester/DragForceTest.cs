using System.Collections;
using UnityEngine;


[RequireComponent(typeof(ValkyrieRigidbody2))]
public class DragForceTest : MonoBehaviour
{

    [Min(0)][SerializeField] private float dragConstant = 0.4f;
    [SerializeField] private Vector3 force = Vector3.right;

    [SerializeField] private bool applyForce = false;
    [SerializeField] private bool applyDrag = false;

    [SerializeField] private float timeToApplyForce = 1;

    private ValkyrieRigidbody2 vrb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        vrb = GetComponent<ValkyrieRigidbody2>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (applyForce)
        {
            if (!coroutineStarted)
            {
                StartCoroutine(ApplyForce(timeToApplyForce, force));
            }
            applyForce = false;
        }

        if(applyDrag)
        {
            float magnitudeSquaredVel = Mathf.Pow(1 + Vector3.Magnitude(vrb.GetVelocity()), 4);
            vrb.ApplyForce(-dragConstant * magnitudeSquaredVel * Vector3.Normalize(vrb.GetVelocity()));
        }
    }


    private bool coroutineStarted = false;
    private IEnumerator ApplyForce(float seconds, Vector3 force)
    {
        coroutineStarted = true;
        float timer = 0f;

        vrb.ApplyForce(seconds, force);

        while(timer <= seconds)
        {
            yield return new WaitForFixedUpdate();
            timer += Time.fixedDeltaTime;
        }

        coroutineStarted = false;
    }


    
}
