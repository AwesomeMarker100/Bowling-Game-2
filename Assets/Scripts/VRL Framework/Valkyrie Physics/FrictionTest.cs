using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(ValkyrieRigidbody2))]
public class FrictionTest : MonoBehaviour
{

    [SerializeField] bool staticFriction = false;
    [SerializeField] bool kineticFriction = false;

    [SerializeField][Range(0.1f, 1)] float kineticFrictionCoefficient = 0.2f;
    [SerializeField][Min(0f)] float gravityAccelerationMag = 9.8f;

    private ValkyrieRigidbody2 vrb;
    private float normalMagnitude = 1;

    private Vector3 forceOfFriction;
    [SerializeField] UnityEvent evt;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        vrb = GetComponent<ValkyrieRigidbody2>();
        normalMagnitude = vrb.GetMass() * Mathf.Cos(Mathf.Deg2Rad * 34.43f) * gravityAccelerationMag;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        


        //STATIC FRICTION
        if (MinoMath.VApproximately(vrb.GetVelocity(), Vector3.zero, 0.002f))
        {
            //forceOfFriction = vrb.Ge
        } else //KINETIC FRICTION
        {
            forceOfFriction = -kineticFrictionCoefficient * normalMagnitude * vrb.GetVelocity().normalized;
            vrb.ApplyForce(forceOfFriction);
        }


    }

    
}
