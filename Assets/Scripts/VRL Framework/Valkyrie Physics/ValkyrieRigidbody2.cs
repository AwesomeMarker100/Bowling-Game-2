using UnityEngine;
using Unity.Mathematics;
using UnityEditor.Rendering;
using System.Collections;
using System.Collections.Generic;
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

    [SerializeField] private Vector3 inertiaTensorRow0 = new Vector3(1, 0, 0);
    [SerializeField] private Vector3 inertiaTensorRow1 = new Vector3(0, 1, 0);
    [SerializeField] private Vector3 inertiaTensorRow2 = new Vector3(0, 0, 1);

    private FMatrix3x3 inertiaTensor;


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
    private Vector3 angularVelocity = Vector3.zero; //rad / s 
    #endregion

    //COLLISION INFO
    #region
    private bool inCollision = false;
    private bool slidingOnSurface = false;
    private ValkyrieCollider col;
    private List<ValkyrieCollision> activeCollisions = new List<ValkyrieCollision>();
    #endregion


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        col = GetComponent<ValkyrieCollider>();

        col.SubscribeToCollisionAwake(OnCollisionAwake);
        col.SubscribeToCollisionPersistent(OnCollisionPersistent);
        col.SubscribeToCollisionDead(OnCollisionEnd);

        FMatrix4x4 matrix = new FMatrix4x4(new Vec4(4, 1, 2, 3), new Vec4(1, 6, 0, 2), new Vec4(2, 0, 5, 1), new Vec4(3, 2, 1, 4));
        matrix.SolvePLU(Vec4.zero);

        inertiaTensor = new FMatrix3x3(new Vec3(inertiaTensorRow0.x, inertiaTensorRow0.y, inertiaTensorRow0.z),
                                      new Vec3(inertiaTensorRow1.x, inertiaTensorRow1.y, inertiaTensorRow1.z),
                                      new Vec3(inertiaTensorRow2.x, inertiaTensorRow2.y, inertiaTensorRow2.z));

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
        if (inCollision && slidingOnSurface && activeCollisions.Count > 0)
        {
            ValkyrieCollision collision = activeCollisions[0];
            acceleration -= Vector3.Dot(acceleration, collision.penetrationNormal) * collision.penetrationNormal;
            velocity -= Vector3.Dot(velocity, collision.penetrationNormal) * collision.penetrationNormal;
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

        angularVelocity = angularVelocity + angularAcceleration * Time.fixedDeltaTime ;


        float angleChange = angularVelocity.magnitude * Time.fixedDeltaTime * Mathf.Rad2Deg;

        if(angleChange > 0.001f)
            transform.rotation = Quaternion.AngleAxis(angleChange, angularVelocity.normalized) * transform.rotation;

        angularAcceleration = Vector3.zero;
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
            //print(velocity);

        }

       
    }
    
    public void ApplyTorque(Vector3 torque)
    {
        //angularAcceleration = inertiaTensor.GetInverse() * torque; //because torque equals = inertia tensor * angular acceleration
    }

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
        if(!isStatic) RespondCollision(collision);
    }

    public void OnCollisionPersistent(ValkyrieCollision collision)
    {
        inCollision = true;

     
        if(!isStatic) RespondCollision(collision);
    }

    public void OnCollisionEnd(ValkyrieCollision collision)
    {
    }

    //returns the angular component of the impulse denominator
    // Formula: (r_A x n)·(I_A^-1(r_A x n)) + (r_B x n)·(I_B^-1(r_B x n))
    public float GetAngularImpulseComp(Vector3 pointOfContact, Vector3 normal, ValkyrieRigidbody2 otherVRB)
    {
        if (pointOfContact == Vector3.negativeInfinity) return 0;
        

        float componentA = 0;
        

        if(!isStatic)
        {
            /*Vector3 rA = transform.InverseTransformPoint(pointOfContact) - centerOfMass;
            Vector3 normalLocalA = transform.InverseTransformDirection(normal);
            FMatrix3x3 inverseInertiaA = inertiaTensor.GetInverse();

            Vector3 rACrossN = Vector3.Cross(rA, normalLocalA);
            Vector3 angularTermA = inverseInertiaA * rACrossN;
            componentA = Vector3.Dot(rACrossN, angularTermA);*/
            Vector3 rA = pointOfContact - (transform.position + transform.rotation *  centerOfMass);
            Matrix4x4 rotateMatrix = Matrix4x4.Rotate(transform.rotation);
            FMatrix3x3 inverseInertiaMat = rotateMatrix * inertiaTensor.GetInverse() * rotateMatrix.transpose;

            Vector3 rACrossN = Vector3.Cross(rA, normal);
            Vector3 angularTermA = inverseInertiaMat * rACrossN;
            componentA = Vector3.Dot(rACrossN, angularTermA);

        }

        // Angular component for object B: (r_B x n)·(I_B^-1(r_B x n))
 
        float componentB = 0;

        if (!otherVRB.isStatic)
        {

            /*Vector3 rB = otherVRB.transform.InverseTransformPoint(pointOfContact) - otherVRB.centerOfMass;
            Vector3 normalLocalB = otherVRB.transform.InverseTransformDirection(normal);
            FMatrix3x3 inverseInertiaB = otherVRB.inertiaTensor.GetInverse();

            Vector3 rBCrossN = Vector3.Cross(rB, normalLocalB);
            Vector3 angularTermB = inverseInertiaB * rBCrossN;
            componentB = Vector3.Dot(rBCrossN, angularTermB); */
            Vector3 rB = pointOfContact - (otherVRB.transform.position + otherVRB.transform.rotation * otherVRB.centerOfMass);
            Matrix4x4 rotateMatrix = Matrix4x4.Rotate(otherVRB.transform.rotation);

            FMatrix3x3 inverseInertiaB = rotateMatrix * otherVRB.inertiaTensor.GetInverse() * rotateMatrix.transpose;
            Vector3 rBCrossN = Vector3.Cross(rB, normal);
            Vector3 angularTermB = inverseInertiaB * rBCrossN;
            componentB = Vector3.Dot(rBCrossN, angularTermB);
        } 

        return componentA + componentB; //temporary!
    }

    public void RespondCollision(ValkyrieCollision col)
    {

        // Calculate impulses for all active collisions and sum them
        Vector3 totalLinearImpulse = Vector3.zero;
        Vector3 totalAngularImpulse = Vector3.zero;
        bool anySlidingOnSurface = false;

        ValkyrieCollider otherCol = col.GetOtherCollider(this.col);
        ValkyrieRigidbody2 otherVRB;

        otherCol.TryGetComponent(out otherVRB);

        Matrix4x4 rotMatrix = Matrix4x4.Rotate(transform.rotation);
        float translationalKE = 0.5f * mass * Mathf.Pow(velocity.magnitude, 2);
        float rotationalKE = 0.5f * Vector3.Dot(angularVelocity, (rotMatrix * inertiaTensor * rotMatrix.transpose) * angularVelocity);

        if (otherVRB)
        {


            //relative velocity dotted with collision normal 
            col.relVelDotNorm = GetRelVelDotNorm(otherVRB.velocity, otherVRB.angularVelocity,
                                                        col.penetrationNormal, col.pointOfContact, otherVRB);

            if (col.relVelDotNorm < 0) return;

            float angularDenom = GetAngularImpulseComp(col.pointOfContact, col.penetrationNormal, otherVRB);

            float cor = physicsMaterial != null ? physicsMaterial.GetCoefficientOfRestitution() : 1f;

            float effectiveInverseMass = GetEffectiveInverseMass(col);

            if (effectiveInverseMass == 0) return;
            // Original formula (naturally gives positive magnitude when approaching, negative when separating)
            col.impulseMagnitude = -(1 + cor) *
                col.relVelDotNorm / (effectiveInverseMass + angularDenom);
            //if(name == "Cube") print($"Velocity: {velocity}, Numerator: {-(1 + cor) * col.relVelDotNorm}, Denominator: {effectiveInverseMass + angularDenom}");

            //print($"AngularDenom: {angularDenom}, TotalDenom: {effectiveInverseMass + angularDenom}, VelDelt: {col.impulseMagnitude * col.penetrationNormal / mass}");

            // col.impulseMagnitude = Mathf.Abs(-(1 + cor) * col.relVelDotNorm / (effectiveInverseMass + angularDenom));

            float relVelDotNorm = col.relVelDotNorm;
            //print(otherVRB.name + ", " + col.penetrationNormal + ", " + col.relVelDotNorm);

            if (Mathf.Abs(col.impulseMagnitude) > 0f)
            {
                totalLinearImpulse += col.impulseMagnitude * col.penetrationNormal;

                // Sum angular impulse
                Vector3 rA = col.pointOfContact - (transform.position + centerOfMass);
                Vector3 impulseDir = col.penetrationNormal;
                Vector3 angularImpulse = Vector3.Cross(rA, col.impulseMagnitude * impulseDir);
                totalAngularImpulse += angularImpulse;

                SeparateObjects(col, 0.0001f);

            }
            //sliding response
            else if (MinoMath.FApproximately(relVelDotNorm, 0, impulseMagnitudeThreshold))
            {
                SeparateObjects(col, 0f);
                anySlidingOnSurface = true;
            }
            else
            {
                SeparateObjects(col, .0001f);
            }
        }

        // Apply summed impulses once


        velocity += totalLinearImpulse / mass;

        angularVelocity += (rotMatrix * inertiaTensor.Inverse * rotMatrix.transpose) * totalAngularImpulse;

        translationalKE = 0.5f * mass * Mathf.Pow(velocity.magnitude, 2);
        rotationalKE = 0.5f * Vector3.Dot(angularVelocity, (rotMatrix * inertiaTensor * rotMatrix.transpose) * angularVelocity);
        /* slidingOnSurface = anySlidingOnSurface;

         // Handle sliding velocity adjustment if needed
         if (slidingOnSurface && activeCollisions.Count > 0)
         {
             Vector3 slidingNormal = activeCollisions[0].penetrationNormal;
             velocity -= Vector3.Dot(velocity, slidingNormal) * slidingNormal;
         }*/
    }
    #endregion

    //COLLISION RESPONSE HELPER FUNCTIONS
    #region
    /*public float GetRelVelDotNorm(Vector3 otherVel, Vector3 otherAngVel, Vector3 penetrationNormal, 
                              Vector3 pointOfContact, ValkyrieRigidbody2 otherVRB)
    {
        // Linear velocity difference
        Vector3 linearVelDiff = otherVel - velocity;
        
        // Angular velocity contribution at contact point (in local space)
        Vector3 rA = transform.InverseTransformPoint(pointOfContact) - centerOfMass;
        Vector3 rB = otherVRB.transform.InverseTransformPoint(pointOfContact) - otherVRB.centerOfMass;
        
        Vector3 angularVelLocal = transform.InverseTransformDirection(angularVelocity);
        Vector3 otherAngVelLocal = otherVRB.transform.InverseTransformDirection(otherAngVel);
        
        Vector3 angularContributionA = Vector3.Cross(angularVelLocal, rA);
        Vector3 angularContributionB = Vector3.Cross(otherAngVelLocal, rB);
        
        Vector3 totalRelVel = linearVelDiff + angularContributionB - angularContributionA;
        
        return Vector3.Dot(totalRelVel, penetrationNormal);
    } */


    //Getting Relative Velocity of "this" object velocity relative to the otherVRBD
    public float GetRelVelDotNorm(Vector3 otherVel, Vector3 otherAngVel, Vector3 penetrationNormal,
                          Vector3 pointOfContact, ValkyrieRigidbody2 otherVRB)
    {
        // Position vectors from center of mass to contact point (world space)
        Vector3 rA = pointOfContact - (transform.position + centerOfMass);
        Vector3 rB = pointOfContact - (otherVRB.transform.position + otherVRB.centerOfMass);

        // Linear velocity difference: velocity of THIS relative to OTHER
        Vector3 linearVelDiff = velocity - otherVel;

        // Angular velocity contributions at contact point (world space)
        Vector3 angularContributionA = Vector3.Cross(angularVelocity, rA);
        Vector3 angularContributionB = Vector3.Cross(otherAngVel, rB);



        // Total relative velocity at contact point
        Vector3 totalRelVel = linearVelDiff + angularContributionA - angularContributionB;
        //print($"LinearDiff: {linearVelDiff}, AngA: {angularContributionA}, AngB: {angularContributionB}, TotalRV: {totalRelVel}, PenNorm: {penetrationNormal}, Dot:{Vector3.Dot(totalRelVel, penetrationNormal)}");


        //print($"Linear Vel Diff: {linearVelDiff}, AngA: {angularContributionA}, AngB: {angularContributionB}, Total: {totalRelVel}, Norm: {penetrationNormal}, Dot: {Vector3.Dot(totalRelVel, penetrationNormal)}");

        // Project onto normal
        return Vector3.Dot(totalRelVel, penetrationNormal);
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

    private void SeparateObjects(ValkyrieCollision collision, float threshold = 0f)
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

