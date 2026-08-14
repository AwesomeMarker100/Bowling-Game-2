using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementEngine : MonoBehaviour
{
    [Header("Necessary Transforms")]
    [SerializeField] private RadixTracker radixTracker;
    [SerializeField] private Camera rotationController;
    [SerializeField] LayerMask ignoreLayers;

    [Header("Controls")]
    public ControllerInput changeMovementController;


    [Header("Ability Settings")]
    public bool canMove = true;
    public bool canRotate = true;
    public bool clampToGround = true;

    [Header("Movement Controllers")]
    public List<MovementController> allowedControllers;
    private int currentIndex = 0;

    public MovementController activeMovementController;
    public string activeMovementControllerName;


    [SerializeField] private Vector3 moveVec;
    [SerializeField] private Quaternion rotQuat;

    private ValkyrieCollider radixCollider;
    private ValkyrieRigidbody2 radixVRB;

    // Start is called before the first frame update
    void Start()
    {
        //if the user didn't add movement controllers from the inspector

        radixVRB = radixTracker.GetComponent<ValkyrieRigidbody2>();
        radixCollider = radixTracker.GetComponent<ValkyrieCollider>();

        radixTracker.TryGetComponent(out radixVRB);
        radixTracker.TryGetComponent(out radixCollider);

        if (allowedControllers.Count == 0)
        {

            allowedControllers = new List<MovementController>(GetComponentsInChildren<MovementController>());

            for (int i = 0; allowedControllers.Count > i; i++)
            {
                MovementController controller = allowedControllers[i];

            }

            if (allowedControllers.Count > 0) currentIndex = 0;

        }

        OculusInputManager.GetInputEvent(changeMovementController).AddListener(ChangeMovementController);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        SetActiveMovementController();
        CommenceMovement(); 
        CommenceRotation();

        if (clampToGround) ClampToGround();
        

    }

    //BASIC FUNCTIONS

    #region
    //only one movement controller allowed at a time
    private void SetActiveMovementController()
    {

        for(int i = 0; i < allowedControllers.Count; i++)
        {

            if (i != currentIndex)
            {

                allowedControllers[i].isActive = false;

            }


        }

        activeMovementController = allowedControllers[currentIndex];
        activeMovementController.isActive = true;

        activeMovementControllerName = activeMovementController.name;

        this.clampToGround = activeMovementController.clampToGround;

    }

    

    private void ChangeMovementController(ControllerInputInfo cia)
    {

        if (currentIndex == allowedControllers.Count - 1) currentIndex = 0;
        else currentIndex++;

    }

    public void SetPosition(Vector3 pos)
    {

        radixTracker.transform.position = pos;

    }

    public void SetParent(Transform parent)
    {
        radixTracker.transform.parent = parent;

    }

    #endregion


    //ACTUAL MOVEMENT AND ROTATION FUNCTIONS
    #region
    //if we can move, move
    private void CommenceMovement()
    {


        if (canMove)
        {

            //set the velocity of the RadixTracker to the moveVec


            radixVRB.ApplyForce(moveVec);
            if (clampToGround) ClampToGround();

            moveVec = Vector3.zero;

        }


    }

    private void CommenceRotation()
    {

        if (canRotate)
        {

            rotationController.transform.rotation *= rotQuat;
            rotQuat = Quaternion.Euler(0, 0, 0);

        }

    }

    


    private void ClampToGround()
    {
        RaycastHit hit;
        hit = GetRaycastHit(radixTracker.transform.position, Vector3.down, Mathf.Infinity, LayerMask.GetMask("Ground"));

        if (hit.transform == null)
        {
            hit = GetRaycastHit(radixTracker.transform.position, Vector3.up, Mathf.Infinity, LayerMask.GetMask("Ground"));
            float y = GetNearestTerrain(hit.point).SampleHeight(hit.point);

            hit.point = new Vector3(radixTracker.transform.position.x, y, radixTracker.transform.position.z);
        }

        if (hit.transform != null)
        {
            Vector3 hitPoint = hit.point;
            radixTracker.transform.position = new Vector3(radixTracker.transform.position.x, hitPoint.y, radixTracker.transform.position.z);
            
        }

    }

    #endregion


    //CLAMP TO GROUND HELPER FUNCTIONS

    #region

    private Terrain GetNearestTerrain(Vector3 point)
    {
        //get all terrains, find the terrain with the closest distance to a point and return
        Terrain[] terrains = FindObjectsOfType<Terrain>();

        Terrain closestTerrain = terrains[0];

        for(int i = 1; terrains.Length > i; i++)
        {

            if(Vector3.Distance(terrains[i].transform.position, point) < Vector3.Distance(closestTerrain.transform.position, point))
            {

                closestTerrain = terrains[i];

            }

        }

        return closestTerrain;

    }

    private RaycastHit GetRaycastHit(Vector3 pos, Vector3 dir, float distance, int layerMask)
    {
        //return if a raycastHit, really only used for the ClampToGround() function

        RaycastHit hit;
        Physics.Raycast(pos, dir, out hit, distance, layerMask);

        return hit;
    }


    #endregion


    //MOVEVEC MANIPULATION FUNCTIONS
    #region
    public void BlendRotation(Quaternion rotQuat)
    {
        this.rotQuat *= rotQuat;

    }

  

    public void OverrideMovement(Vector3 moveVec, float speed)
    {
        //directly sets the moveVec, overriding what it previously was
        this.moveVec = moveVec * speed * Time.fixedDeltaTime;

    }

    #endregion

    /*  public void AddMovement(Vector3 moveVec, float speed)
    {

        this.moveVec += moveVec * speed * Time.deltaTime;

    }*/


}
