using UnityEngine;
using Unity.Mathematics;
using UnityEditor.Rendering;
using System.Collections;
using UnityEngine.Rendering;
using UnityEngine.Analytics;
using UnityEngine.Events;
using System;
using Unity.VisualScripting;

[RequireComponent(typeof(ValkyrieCollider))]
public class ValkyrieRigidbody2 : MonoBehaviour
{
    [SerializeField] private float mass = 1f; //kg 
    [SerializeField] private Vector3 centerOfMass = Vector3.zero;

    [SerializeField] bool isStatic = false;

    [SerializeField] ValkPhysMat physicsMaterial;


    //THRESHOLDS
    #region
    [SerializeField] float impulseMagnitudeThreshold = 0.08f;
    [SerializeField] private float velocityThreshold = 0.08f;
    [SerializeField] private float accelerationThreshold = 0.08f;
    #endregion

    
    //IMPORTANT QUANTITIES (VELOCITY, ACCELERATION, ANGULAR VERSIONS)
    #region
    private Vector3 velocity = Vector3.zero;
    private Vector3 acceleration = Vector3.zero;

    private Vector3 angularAcceleration = Vector3.zero;
    private Vector3 angularVelocity = Vector3.zero;
    #endregion

    //COLLISION INFO
    #region
    private bool inCollision = false;
    private bool slidingOnSurface = false;
    private ValkyrieCollider col;
    private ValkyrieCollision curCollision;
    #endregion


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        col = GetComponent<ValkyrieCollider>();

        col.SubscribeToCollisionAwake(OnCollisionAwake);
        col.SubscribeToCollisionPersistent(OnCollisionPersistent);
        col.SubscribeToCollisionDead(OnCollisionEnd);


    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isStatic)
        {
            Displace();
            Rotate();
        }
    }

    //ADJUSTMENTS FOR DISPLACEMENT
    #region
    private void AdjustForSliding()
    {
        if (inCollision && slidingOnSurface)
        {
            acceleration -= Vector3.Dot(acceleration, curCollision.penetrationNormal) * curCollision.penetrationNormal;
            velocity -= Vector3.Dot(velocity, curCollision.penetrationNormal) * curCollision.penetrationNormal;
        }
    }

    private void AdjustForThresholds()
    {
        if(acceleration.magnitude < 0.001f)
        {
            acceleration = Vector3.zero;
        }

        if(velocity.magnitude < 0.001f)
        {
            velocity = Vector3.zero;
        }
    }
    #endregion

    //DISPLACE AND ROTATE
    #region
    private void Displace()
    {

        AdjustForSliding();
        AdjustForThresholds();

        velocity = velocity + acceleration * Time.fixedDeltaTime;
        transform.position = transform.position + velocity * Time.fixedDeltaTime;

        acceleration = Vector3.zero;
    }

    private void Rotate()
    {
        Vector3 newRotation = transform.eulerAngles + angularVelocity * Time.fixedDeltaTime + 0.5f * Mathf.Pow(Time.fixedDeltaTime, 2) * angularAcceleration;
        angularVelocity = (newRotation - transform.eulerAngles) / Time.fixedDeltaTime;

        transform.eulerAngles = newRotation;
    }

    #endregion
    //APPLY FORCES
    #region
    //force given in Newtons, acceleration has units ms^-2. This is for Impulse Forces
    public void ApplyForce(Vector3 force)
    {
        acceleration += force / mass;
    }

    public void ApplyForce(float seconds, Vector3 force)
    {
        StartCoroutine(Apply(seconds, force));
    }

    private IEnumerator Apply(float seconds, Vector3 force)
    {
        float timer = 0f;

        while (timer <= seconds)
        {
            ApplyForce(force);
            yield return new WaitForFixedUpdate();
            timer += Time.fixedDeltaTime + 0.00001f;
           // print(velocity);

        }

       
    }


    public void ApplyTorque(Vector3 torque)
    {
        //angularAcceleration = inertiaTensor.GetInverse() * torque; //because torque equals = inertia tensor * angular acceleration
    }

    //adjust GJK to get point of contact on collision
    public void ApplyTorque(Vector3 force, Vector3 pointOfApplication)
    {
        //ApplyTorque(Vector3.Cross(pointOfApplication - pivotPoint, force)); //for torque, force relationship 

    }
    #endregion
    //COLLISION RESPONSE
    #region
    public void OnCollisionAwake(ValkyrieCollision collision)
    {
        inCollision = true;
        curCollision = collision;

        //if(name == "Test Cube") print("AWAKE");
        if(!isStatic) RespondCollision(collision);
    }

    public void OnCollisionPersistent(ValkyrieCollision collision)
    {
        inCollision = true;
        curCollision = collision;

        if(!isStatic) RespondCollision(collision);
    }

    public void OnCollisionEnd(ValkyrieCollision collision)
    {
        inCollision = false;
        curCollision = null;

        slidingOnSurface = false;
        //if (name == "Test Cube") print("DEAD");
    }

    public void RespondCollision(ValkyrieCollision collision)
    {
        //note: collision.penetrationNormal points TOE-WARD (Mr. Gray reference) the current VRB2

        ValkyrieCollider otherCol = collision.GetOtherCollider(col);
        ValkyrieRigidbody2 otherVRB;

        otherCol.TryGetComponent(out otherVRB);
        slidingOnSurface = false;

        if (otherVRB)
        {

            collision.relVelDotNorm = GetRelVelDotNorm(otherVRB.velocity, collision.penetrationNormal);
            float cor = 1f;

            float effectiveInverseMass = GetEffectiveInverseMass(collision);

            if (effectiveInverseMass == 0) print("Effective Inverse Mass is 0.");
            

            collision.impulseMagnitude = -(1 + cor) * collision.relVelDotNorm / effectiveInverseMass;
            float relVelDotNorm = collision.relVelDotNorm;

            if(relVelDotNorm < 0 && collision.impulseMagnitude > impulseMagnitudeThreshold)
            {

                SeparateObjects(collision, 0.003f);
                velocity += -collision.impulseMagnitude * collision.penetrationNormal / mass;

            } else if(MinoMath.FApproximately(relVelDotNorm, 0, impulseMagnitudeThreshold))
            {

                velocity -= Vector3.Dot(velocity, collision.penetrationNormal) * collision.penetrationNormal;
                SeparateObjects(collision, -0.001f);
                //print("sliding");
                slidingOnSurface = true;

            } else
            {
                SeparateObjects(collision, 0.003f);
            }
            

        }


    }
    #endregion

    //COLLISION RESPONSE HELPER FUNCTIONS
    #region
    public float GetRelVelDotNorm(Vector3 otherVel, Vector3 penetrationNormal)
    {
        return Vector3.Dot(otherVel - velocity, penetrationNormal);
    }


    public float GetEffectiveInverseMass(ValkyrieCollision collision)
    {
        float effectiveInverseMass = 0;

        ValkyrieCollider otherCol = collision.GetOtherCollider(col);
        ValkyrieRigidbody2 otherVRB;

        otherCol.TryGetComponent(out otherVRB);

        if(!otherVRB.isStatic)
        {
            effectiveInverseMass += 1 / otherVRB.mass;
        }

        if(!isStatic)
        {
            effectiveInverseMass += 1 / this.mass;
        }

        return effectiveInverseMass;
    }

    private void SeparateObjects(ValkyrieCollision collision, float threshold)
    {


        ValkyrieCollider otherCol = collision.GetOtherCollider(col);
        ValkyrieRigidbody2 otherVRB;

        otherCol.TryGetComponent(out otherVRB);

        Vector3 separationDirection = collision.penetrationNormal;
        Vector3 inBetweenVector = otherCol.transform.position - this.transform.position;

        if (Vector3.Dot(separationDirection, inBetweenVector) > 0) separationDirection *= -1;

        if (otherVRB)
        {
            if(!isStatic && otherVRB.isStatic)
            {
                //other doesn't move, we move entirely
                transform.position = transform.position + separationDirection * (collision.penetrationDepth + threshold);
            }
            else if(!(isStatic || otherVRB.isStatic))
            {
                //both of us move halfway 
                transform.position = transform.position + separationDirection * (collision.penetrationDepth + threshold) / 2;


            }
        } else
        {
            transform.position = transform.position + separationDirection * (collision.penetrationDepth + threshold);
        }
    }
    #endregion

    //GETTERS AND SETTERS
    #region
    public Vector3 GetVelocity()
    {
        return this.velocity;
    }

    public void SetVelocity(Vector3 velocity)
    {
        this.velocity = velocity;
    }

    public float GetVelocitySqrMagnitude()
    {
        return Vector3.SqrMagnitude(velocity);
    }

    public Vector3 GetAcceleration()
    {
        return this.acceleration;
    }

    public Vector3 GetSquaredVelocity()
    {
        return new Vector3(Mathf.Pow(velocity.x, 2), Mathf.Pow(velocity.y, 2), Mathf.Pow(velocity.z, 2));
    }

    //elastic collision response

    public float GetMass()
    {
        return this.mass;
    }

    public Vector3 GetMomentum()
    {
        return mass * velocity;
    }

    #endregion

    //FMatrix3x3
    #region
    //inertiaTensor = new FMatrix3x3(row1.x, row1.y, row1.z, row2.x, row2.y, row2.z, row3.x, row3.y, row3.z);


    //print(inertiaTensor.GetInverse());


    /* centerOfMassWorldCoords = transform.TransformPoint(centearOfMass);
     basicDampingForce = -velocity * dampingConstant;
     basicDampingTorque = -angularVelocity * dampingConstant;

     ApplyForce(testForce);
     ApplyForce(basicDampingForce);
     ApplyTorque(testTorque);*/
    //ApplyTorque(basicDampingTorque);

    //Rotate();
    //FMatrix3x3 matrix = new FMatrix3x3(testMatrix[0, 0], testMatrix[0, 1], testMatrix[0, 2], testMatrix[1, 0], testMatrix[1, 1], testMatrix[1, 2], testMatrix[2, 0], testMatrix[2, 1], testMatrix[2, 2]);
    // print("Normal Matrix: " + matrix);
    //print("Upper Triangular Matrix: " + matrix.GetPrintableUpperTriangular());
    //print("Inertia Tensor: " + inertiaTensor);
    //print("Inverse: " + inertiaTensor.inverse);

    /*linearMomentum = mass * velocity;

    ApplyForce(testForce);
    ApplyForce(basicDampingForce);
    ApplyTorque(testTorque);
    ApplyTorque(basicDampingTorque);
    */
    #endregion


    //ASYNC VOID TEST
    #region
    //basically only should use async void for asynchronous event handling from my research
    //we don't need to return Awaitable since this method is not meant to be awaited by the ProgrammableNode
    public async void SayHi(ProgrammableNodeSignal signal)
    {
        signal.SetTaskStarted();
        await Awaitable.WaitForSecondsAsync(4);
        signal.SetTaskCompleted();
    }
    #endregion
}

