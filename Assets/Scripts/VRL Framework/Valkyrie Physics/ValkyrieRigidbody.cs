using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[ExecuteAlways]
public class ValkyrieRigidbody : MonoBehaviour
{
    [Header("Basic Properties")]
    public float mass = 1f;

    public bool applyGravity = false;
    private bool canTraverseGravity = true;

    [SerializeField] private float gravity = -2f;

    [Header("Interaction")]
    public bool isKinematic = false;

    [Header("Movement Information")]

    public bool printImpulseForce = false;
    [HideInInspector] public Vector3 velocity;
    [HideInInspector] public Vector3 acceleration;
    [HideInInspector] public Vector3 angularVelocity;
    [HideInInspector] public Vector3 centerOfMass;
    [HideInInspector] public Vector3 linearMomentum;
    [HideInInspector] public Vector3 angularMomentum;
    [HideInInspector] public float kineticEnergy;


    [Range(0, 1)][SerializeField] private float coefficientOfRestitution = 0.3f;

    public Vector3 inertiaTensor = Vector3.one; //how much is the object rejecting torque on each axis

    public Vector3 resistanceForce;
    public Vector3 angularResistanceForce;

    private bool noResistanceForce = false;

    [Header("Collision Detection")]
    private new ValkyrieCollider collider;
    
    private Vector3 lastPosition;
    private Vector3 lastVelocity;

    
    //CONSTANTS
    
    private const float pi = Mathf.PI;
    private const float tau = 2 * Mathf.PI;

    [HideInInspector] public bool colliding = false;

    private float impulseTime = 0f;

    private void Start()
    {

        if (this.isActiveAndEnabled)
        {
            lastVelocity = Vector3.zero;
            lastPosition = transform.position;
            velocity = Vector3.zero;
            centerOfMass = transform.position;

            collider = this.GetComponent<ValkyrieCollider>();

            if (Application.isPlaying)
            {
                if (collider != null)
                {
                  /*  collider.onCollisionAwake.AddListener(OnCollisionStart);
                    collider.onCollisionPersistent.AddListener(OnCollisionPersistent);
                    collider.onCollisionDead.AddListener(OnCollisionEnd);*/
                }
            }

            if (resistanceForce == Vector3.zero) noResistanceForce = true;
        }
        

    }

    private void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (this.isActiveAndEnabled)
        {
            centerOfMass = transform.position;

            linearMomentum = mass * velocity;
            angularMomentum = Vector3.Cross(transform.position - Vector3.zero, linearMomentum);
            kineticEnergy = mass * velocity.sqrMagnitude / 2;

            if (applyGravity && canTraverseGravity)
            {
                Accelerate(new Vector3(0f, gravity, 0f));
            }


            SetAcceleration();


            
            TravelVelocity();

            TravelAngularVelocity();
        }
    }

    

    private void TravelVelocity()
    {

        //units of velocity are meters / second

        transform.position = transform.position + velocity * Time.fixedDeltaTime;

        if (noResistanceForce)
        {

            resistanceForce = (-this.velocity * mass);

        }

        ApplyForce(resistanceForce * Time.fixedDeltaTime); //COULD BE DRAG, FRICTION, ETC


    }

    private void TravelAngularVelocity()
    {
        //Quaternion angularVelocity = Quaternion.Euler(this.angularVelocity * Time.fixedDeltaTime);
        //transform.rotation *= angularVelocity;


        transform.eulerAngles = transform.eulerAngles + angularVelocity * Time.fixedDeltaTime;
        this.angularVelocity -= this.angularVelocity * Time.fixedDeltaTime; 

    }


    private void ApplyGravity()
    {
        
        Accelerate(new Vector3(0f, gravity * Time.fixedDeltaTime, 0f));
    }


    #region

    private void SetAcceleration()
    {

        acceleration = (velocity - lastVelocity) / Time.fixedDeltaTime;
        lastVelocity = velocity;

    }

    public void ApplyAngularVelocity(Vector3 angularVelocity)
    {

        this.angularVelocity += angularVelocity;

    }

    public void SetAngularVelocity(Vector3 angularVelocity)
    {

        this.angularVelocity = angularVelocity;

    }

    public void SetVelocity(Vector3 velocity)
    {

        this.velocity = velocity;

    }

    public void SetPosition(Vector3 newPos)
    {
        //this.velocity = (newPos - transform.position) / Time.fixedDeltaTime;
        this.velocity = (newPos - transform.position) / Time.fixedDeltaTime;

    }

    public void SetRotation(Quaternion rotation, bool localSpace)
    {

        if (localSpace)
        {

            SetAngularVelocity((rotation.eulerAngles - transform.eulerAngles) / Time.fixedDeltaTime);


        } 

    }

    public void Accelerate(Vector3 velocity)
    {
        this.velocity += velocity;

    }


    public void ApplyForce(Vector3 force)
    {
        //velocity already gets multiplied by fixedDeltaTime in TravelVelocity() so don't add it here
        velocity += force / mass;

    }

    public void SetForce(Vector3 force)
    {

        SetVelocity(force * Time.deltaTime / mass);

    }

    public void SetTorque(Vector3 torque)
    {

        this.angularVelocity = (torque * Time.fixedDeltaTime / mass);

    }

    public void ApplyTorque(Vector3 torque)
    {
        //f = ma
        //a = deltav / deltat
        //f = (mdeltav) / deltat
        //f * deltat / mass = deltav

        this.angularVelocity += (torque / mass);

    }

    //GETTERS

    #region
    private Vector3 GetRelativeVelocity(Vector3 a, Vector3 b)
    {
        return velocity + Vector3.Cross(angularVelocity, a - b);

    }


    private Vector3 GetAngularMomentum(ValkyrieRigidbody aRigidbody, Vector3 a, Vector3 b)
    {
        float r = Vector3.Distance(a, b);
        float angle = Vector3.Angle(a, b);
        Vector3 linearMomentum = aRigidbody.mass * aRigidbody.velocity;

        return r * angle * linearMomentum;
    }


    public Vector3 GetVelocityAtPoint(ValkyrieRigidbody vr, Vector3 point)
    {
        Vector3 betweenVector = point - vr.centerOfMass;
        return velocity + Vector3.Cross(angularVelocity, betweenVector);

    }


    private Vector3 GetLinearImpulse(Vector3 otherMomentum)
    {
        Vector3 impulse = (otherMomentum - linearMomentum) / impulseTime;
        return impulse;
    }

    private Vector3 GetAngularImpulse(Vector3 otherAngularMomentum)
    {
        Vector3 impulse = (otherAngularMomentum - angularMomentum) / impulseTime;
        return impulse;

    }

    #endregion


    /*private bool BlockedInDirection(Vector3 direction, float raycastAmount)
    {
        RaycastHit hit;
        return Physics.Raycast(transform.position, direction, out hit, raycastAmount,  ~ignoreLayers) && hit.transform.GetComponent<ValkyrieRigidbody>().isImmovable;

    }*/


    #endregion


    /*


        https://www.scss.tcd.ie/~manzkem/CS7057/cs7057-1516-09-CollisionResponse-mm.pdf

        The development of this elastic collision function took quite a long time to figure out as I switched back and forth from different methods. Lots of sources were visited but 
        the one above was the one that got the function to the finish line. Some math was written down in a notebook as well(mostly conservation of linear momentum which was a previous
        method I tried to use) and the derivation of velocity and angular velocity as well.


        It was finally figured out that impulse force was what I needed but I struggled to understand how to derive impulse force without knowing final velocities for either object

        This is important because Impulse Force is commonly defined as the change of momentum during a collision which means I need the change in velocity which I cannot get without final velocities

        However there is another formula that says that the impulse force is equal to a scalar j multiplied by the normal vector from the collision point

        This impulse force can then be used to derive velocity and angular velocity.

        Formula: J = j(scalar) * normal(vector);

        velocity of object = ((j * impulseTime) / mass) + initial velocity
        angular velocity of object = Cross product of r(r being collisionPoint - centerOfMass) and J all divided by the momentOfInertia + the initial angular velocity
        
        
        DERIVATION OF FORMULA 


        relative velocity is equal to the normal velocity * (position of object A - position of object B)

        the position of object A(in a collision) is equal to its linear velocity + its angular velocity multiplied by r(which is the collision point - centerOfMass)
        
        the velocity of object A is equal to its initial velocity + acceleration in a given moment -- acceleration is the impulse force divided by mass

        the same is true for B for the three statements above

        now plug in position of object A and position of object B for relative velocity

        now solve for j

        and you will get the formula presented in the slideshow
        
        Essentially j is derived from the main components of the relative velocity and normal vector -- and relative velocity is derivated from the positions of the objects which
        are derived from their linear and angular components made up from their linear and angular velocities
        

        

    */

   /*private Vector3 GetImpulseForce(Vector3 collisionPoint, ValkyrieRigidbody otherVRB)
    {
        
        //rA and rB are simple- the Vector from the collision point to center of masses for both objects
        Vector3 rA = collisionPoint - this.centerOfMass;
        Vector3 rB = collisionPoint - otherVRB.centerOfMass;



        //normal vector is the direction vector we travel upon, it is perpendicular to the plane of the collision point

        Vector3 normal = (otherVRB.transform.position - this.transform.position).normalized;

        //relative velocity is velocity from the perspective of another observer

        
        //jNumerator is just the magnitude of the relativeVelocity multiplied by -(1 + e) where e is 1 so I simplified : e is the coefficient of restitution which describes loss of kinetic energy(1 means no loss, 0 means oof)
        float jNumerator = -2 * relativeVelocityMagnitude;
        float jDenomPart1 = (1 / this.mass) +  (1 / otherVRB.mass);

        //we take the cross product of the normal vector * the dot product of (1 / momentOfInertia) * an arbitrary rotation axis(up in this case) with the cross product of rA and the normal with rA
        Vector3 jDenomPart2 = Vector3.Cross(normal * Vector3.Dot((1 / momentOfInertia) * transform.up, Vector3.Cross(rA, normal)), rA);
        Vector3 jDenomPart3 = Vector3.Cross(normal * Vector3.Dot((1 / otherVRB.momentOfInertia) * otherVRB.transform.up, Vector3.Cross(rB, normal)), rB);

        //then we take the cross product of the vectors we found before and take the magnitude
        float jDenomPart4 = Vector3.Cross(jDenomPart2, jDenomPart3).magnitude;

        //then we add up the mass part with the magnitude part
        float jDenom = jDenomPart1 + jDenomPart4;

        //this gives us our scalar value j which helps to get the impulse force since J = j * normal vector and J = change of momentum

        

        float j = jNumerator / jDenom;

        return j * normal;


    }*/

    private Vector3 GetImpulseForce(Vector3 collisionPoint, ValkyrieRigidbody other)
    {
        //confirmed
        Vector3 rA = collisionPoint - this.centerOfMass;
        Vector3 rB = collisionPoint - other.centerOfMass;

        //confirmed
        Vector3 pA = this.velocity + Vector3.Cross(this.angularVelocity, rA);
        Vector3 pB = other.velocity + Vector3.Cross(this.angularVelocity, rB);

        //confirmed
        Vector3 normal = (collisionPoint - this.centerOfMass).normalized;
        Vector3 relVelocity = normal * (pA - pB).magnitude;


        //confirmed
        float jNumerator = -(1 + coefficientOfRestitution) * relVelocity.magnitude;
        float jDenomPart1 = (1 / this.mass) + (1 / other.mass);

        Vector3 inverseInertiaTensorA = this.transform.rotation * this.inertiaTensor;
        Vector3 inverseInertiaTensorB = other.transform.rotation * other.inertiaTensor;

        float jDenomPart2 = Vector3.Cross(normal * Vector3.Dot(inverseInertiaTensorA, Vector3.Cross(rA, normal)), rA).magnitude;
        float jDenomPart3 = Vector3.Cross(normal * Vector3.Dot(inverseInertiaTensorB, Vector3.Cross(rB, normal)), rB).magnitude;

        float jDenomPart4 = jDenomPart1 + jDenomPart2 + jDenomPart3;

        return (jNumerator / jDenomPart4) * normal;

    }



}
