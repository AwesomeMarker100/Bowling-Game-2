using UnityEngine;

[RequireComponent(typeof(ValkyrieRigidbody2))]
public class CircularMotionTest : MonoBehaviour
{
    [SerializeField] bool startMotion = false;

    [SerializeField] Transform center;
    [SerializeField][Min(0f)] float speed = 2f;
    [SerializeField][Min(0f)] float radius = 1f;

    private bool initSetupDone = false;
    private ValkyrieRigidbody2 vrb;
    private float mass;

    private float squaredSpeed;

    private void Start()
    {
        vrb = GetComponent<ValkyrieRigidbody2>();
        mass = vrb.GetMass();

        squaredSpeed = Mathf.Pow(speed, 2);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(startMotion)
        {
            if (!center)
            {
                print("Center transform not selected!");
                return;
            }

            if(speed == 0f)
            {
                print("Speed is set to 0, please change this!");
                return;
            }
            
            if(radius == 0f)
            {
                print("Radius is set to 0, please change this!");
                return;
            }

            if (!initSetupDone)
            {
                transform.position = center.position + Vector3.right * radius;
                vrb.SetVelocity(speed * Vector3.forward);
                initSetupDone = true;
                return; //wait for a frame to let the velocity update for the VRB and then apply centripetal acceleration
            }

            Vector3 toCenter = (center.position - transform.position).normalized;
            Vector3 acceleration = squaredSpeed / radius * toCenter;

            vrb.ApplyForce(mass * acceleration);

        }  else
        {
            initSetupDone = false;
        }
    }
}
