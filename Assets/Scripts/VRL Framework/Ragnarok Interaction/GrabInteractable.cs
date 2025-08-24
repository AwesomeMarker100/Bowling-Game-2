using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static HandTracker;

[System.Serializable]
[RequireComponent(typeof(ValkyrieCollider))]
public class GrabInteractable : MonoBehaviour
{

    [Header("Grab Requirements")]
    [Range(0, 1)] public float minimumGripAmount = 0.3f;
    [Range(0, 1)] public float maximumGripAmount = 0.8f;

    [Range(0, 1)] public float minimumPointAmount = 0.3f;
    [Range(0, 1)] public float maximumPointAmount = 0.8f;

    [SerializeField] public bool requiresBothHands = false;

    [SerializeField] private bool canBeDropped = true;
    [SerializeField] private bool leftHandTouching = false;
    [SerializeField] private bool rightHandTouching = false;
    [SerializeField] private bool useVRB = false;


    [SerializeField] private LayerMask layer;
    


    [Header("Location in Hand")]

    [SerializeField] private HandType handBasisType = HandType.Right;
    [SerializeField] private bool symmetrizeLocation = true;

    [SerializeField] private SerVector3 localPosition;
    [SerializeField] private SerQuaternion localRotation;
    [SerializeField] private SerVector3 localScale;


    [HideInInspector] public GrabInteractor curInteractor;
    [HideInInspector] public bool isHeld = false;

    private new ValkyrieCollider collider;

    private GrabInteractor leftHand;
    private GrabInteractor rightHand;

    private Transform origParent;

    private Vector3 dropVelocity;
    

    public void Start()
    {


        if (localScale.x == 0 || localScale.y == 0 || localScale.z == 0)
        {
            localScale.x = 1;
            localScale.y = 1;
            localScale.z = 1;
        }

        collider = GetComponent<ValkyrieCollider>();

        leftHand = VRLGameObjectManager.instance.TryGetLeftHand().GetComponent<GrabInteractor>();
        rightHand = VRLGameObjectManager.instance.TryGetRightHand().GetComponent<GrabInteractor>();

        layer = this.gameObject.layer;

        isHeld = false;
        origParent = transform.parent;

    }

    public void Update()
    {
        if (!isHeld)
        {
            

            if (rightHandTouching) //default to right hand first
            {


                Teleport(rightHand.transform, HandType.Right);
                SetHeld(rightHand);

                rightHand.HoldItem(this);


            }
            else if (leftHandTouching)
            {

                Teleport(leftHand.transform, HandType.Left);
                SetHeld(leftHand);

                leftHand.HoldItem(this);
            }

            
        } 
        

    }

    public void SetHeld(GrabInteractor curInteractor)
    {
        isHeld = true;
        this.curInteractor = curInteractor;
        this.gameObject.layer = curInteractor.gameObject.layer;
    }

    

    public bool MeetsHandRequirements(GrabInteractor interactor)
    {

        float[] handPoserVals = interactor.GetHandDetails();

        return handPoserVals[0] > minimumGripAmount && handPoserVals[1] > minimumPointAmount;

    }

    public void Teleport(Transform handTransform, HandType handType)
    {
        transform.SetParent(handTransform, false);

        if (handBasisType.Equals(handType))
        {
            transform.localScale = localScale.ToVector3();
            transform.localPosition = localPosition.ToVector3();
            transform.localRotation = localRotation.ToQuaternion();
            
        }
    }

    public void Drop()
    {

        isHeld = false;
        transform.parent = origParent;
        this.gameObject.layer = layer;

        leftHandTouching = false;
        rightHandTouching = false;

        curInteractor.DropInteractable();

        if (useVRB)
        {

            ValkyrieRigidbody thisVRB = GetComponent<ValkyrieRigidbody>();

            if(thisVRB != null)
            {

                thisVRB.velocity = dropVelocity;
                
            }

        }
    }


    //EDITOR FUNCTIONS
    #region
    public void SetLocalPosition(Vector3 pos)
    {

        localPosition.Set(pos);

    }

    public void SetLocalRotation(Quaternion rot)
    {
        localRotation.Set(rot);

    }


    public void SetLocalScale(Vector3 scale)
    {

        localScale.Set(scale);

    }

    #endregion

}
