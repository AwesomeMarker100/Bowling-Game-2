using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LaserInteractor : MonoBehaviour
{

    public enum LaserType
    {
        Linear,
        Parabolic,
        Arc

    }

    public enum Axis
    {

        X, Y, Z

    }

    [Header("Basic Info")]
    public LineRenderer laser;
    public float maximumDistance = 5f;
    public Vector3 rotationOffset;
    public Gradient normal;
    public Gradient hit;
    public HandTracker hand;

    [Header("Collisions")]
    [SerializeField] public bool doCollisions = true;
    [SerializeField] private Transform endPoint;
    [SerializeField] private LayerMask ignoreLayers;

    [SerializeField] public Transform ballPoint;
    [SerializeField] public Material normalMaterial;
    [SerializeField] private Material hitMaterial;


    [HideInInspector] public LaserType laserType;
    private Vector3 surfaceEndpointPosition;

    private ValkyrieCollider hitCol;
    private HittableObject uiObject;

    private Vector3 hitPoint;
    
    private ValkPhys phys;

    // Start is called before the first frame update
    void Start()
    {
        laser.colorGradient = normal;
        laser.useWorldSpace = false;

        phys = FindObjectOfType<ValkPhys>();
    }
    


    private void Update()
    {

       CreateLaser();

    }


    public virtual void CreateLaser()
    {

    }

    public virtual Vector3 GetEndpointPosition()
    {

        return surfaceEndpointPosition;

    }


    //COLLISIONS BELOW
    public virtual bool DidCollide(int latestIndex, float dist)
    {
        if (latestIndex == 0) return false;


        //LASER POSITIONS ARE IN LOCAL COORDINATES SO TRANSFORM POINTS TO WORLD SPACE

        Vector3 start = transform.TransformPoint(laser.GetPosition(latestIndex));
        Vector3 dir = transform.TransformDirection(laser.GetPosition(latestIndex) - laser.GetPosition(latestIndex - 1));


        //using the custom raycasting - utilizes the super fast KDTree technique(not really but stfu)

        ValkyrieRaycastHit hit;
        bool raycastHit = phys.Raycast(start, dir, dist, ignoreLayers, out hit);

        if (raycastHit)
        {
            SetBallPointMaterial(true);
            laser.SetPosition(latestIndex, transform.InverseTransformPoint(hit.hitPoint)); //transform global coordinates to local coordinates

            surfaceEndpointPosition = hit.hitPoint;

            //if we didn't hit anything before, then just set a new hitCol but if we did, we need to tell the previous thing we hit that we're not hitting it anymore
            if (hit.collider != null) hitCol = hit.collider;
            else ReplaceHitObject(hit.uiObject);

            return true;

        }
        else
        {
            //we didn't hit anything
            SetBallPointMaterial(false);
            hitCol = null;

            DisableHitObject();

            surfaceEndpointPosition = Vector3.zero;

            return false;
        }

    }



    public void SetBallPointMaterial(bool hit)
    {
        if (!hit)
        {
            ballPoint.GetComponent<MeshRenderer>().material = normalMaterial;


        } else
        {
            ballPoint.GetComponent<MeshRenderer>().material = hitMaterial;

        }

    }

    //utilizes the custom HittableObject script
    public void ReplaceHitObject(HittableObject newUIObject)
    {

        DisableHitObject();

        if (newUIObject != null)
        {
            newUIObject.ActivateHover();
            uiObject = newUIObject;
        }

    }

    public void DisableHitObject()
    {
        if (uiObject != null) uiObject.DeactivateHover();
        uiObject = null;
    }

}
