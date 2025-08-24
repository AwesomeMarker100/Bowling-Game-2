using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(ValkyrieRigidbody2))]
public class HarmonicOscillatorTest : MonoBehaviour
{
    [SerializeField] Vector3 initialVelocity;
    [SerializeField] Vector3 initialDisplacement;

    [Tooltip("In Ns/m")]
    [SerializeField] float dampingConstant = 3f;

    [Tooltip("In N/m")]
    [SerializeField] float springConstant = 2f;
    
    [SerializeField] bool applyForce = false;
    [SerializeField] float timeStep = 1;

    private bool applyInit = false;
    private ValkyrieRigidbody2 vrb;
    private Vector3 originalPosition;
    private Vector3 vel = Vector3.zero;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        vrb = GetComponent<ValkyrieRigidbody2>();
        originalPosition = transform.position;
        vel = initialVelocity;
    }

    void FixedUpdate()
    {

        if (applyForce)
        {
            
            if(!applyInit)
            {
                transform.position = originalPosition + initialDisplacement;
                vel = initialVelocity;
                applyInit = true;
            } else
            {
                vel = vrb.GetVelocity();
            }

            vrb.ApplyForce(-springConstant * (transform.position - originalPosition) - dampingConstant * vel);
        } else
        {
            applyInit = false;
        }
    }

}


