using UnityEngine;

using Min = UnityEngine.MinAttribute;

[RequireComponent(typeof(ValkyrieRigidbody2))]
public class GravityTest : MonoBehaviour
{
    [SerializeField][@Min(0)] float gravity = 3;
    public bool applyForce = false;

    private Vector3 gravityAcceleration;
    private float mass;
    private ValkyrieRigidbody2 vrb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        vrb = GetComponent<ValkyrieRigidbody2>();
        mass = vrb.GetMass();

        gravityAcceleration = new Vector3(0, -gravity, 0);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (applyForce)
        {
            vrb.ApplyForce(mass * gravityAcceleration);

        }
    }
    
}
