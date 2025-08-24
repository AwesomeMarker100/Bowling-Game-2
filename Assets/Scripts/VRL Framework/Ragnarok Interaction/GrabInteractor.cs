using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HandTracker;

public class GrabInteractor : MonoBehaviour
{
    [SerializeField] private HandTracker handTracker;
    [SerializeField] private HandPoser handPoser;

    [SerializeField] private GrabInteractable editorItem;

    private Vector3 previousEditorItemPosition;

    private ValkyrieCollider thisCol;
    public bool isHoldingObject = false;

    private void Start()
    {
        thisCol = GetComponent<ValkyrieCollider>();
        isHoldingObject = false;
       // thisCol.onCollisionPersistent.AddListener(OnCollisionPersistent);
    }

    //if the hand is gripping enough
 
    public float[] GetHandDetails()
    {

        return new float[]
        {
            handPoser.gripAmount,
            handPoser.pointAmount,

        };

    }

    
    public void HoldItem(GrabInteractable interactable)
    {

        LimitHandMovement(interactable);
        AddDropConditions(interactable);

        isHoldingObject = true;

    }

    //INTERACTOR RESPONSIBILITIES WHEN HOLDING INTERACTABLE
    #region
    //stop the hand animator from clipping into the item(user set)
    public void LimitHandMovement(GrabInteractable interactable)
    {
        handPoser.SetMinimumGrip(interactable.minimumGripAmount);
        handPoser.SetMinimumPoint(interactable.minimumPointAmount);

        handPoser.SetMaximumGrip(interactable.maximumGripAmount);
        handPoser.SetMaximumPoint(interactable.maximumPointAmount);

    }

    //if your grip starts to break, drop the item
    private void AddDropConditions(GrabInteractable interactable)
    {
        handPoser.belowGripAmount.AddListener(interactable.Drop);
        handPoser.belowPointAmount.AddListener(interactable.Drop);

    }

    #endregion

    public HandType GetHandType()
    {

        return handTracker.handType;

    } 


    public void DropInteractable()
    {

        isHoldingObject = false;
        

        handPoser.SetMinimumGrip(0);
        handPoser.SetMinimumPoint(0);

        handPoser.SetMaximumGrip(1);
        handPoser.SetMaximumPoint(1);

    }



    //EDITOR SCRIPTS
    #region

    public void SetLocation()
    {

        if(editorItem.transform.parent != this.transform) previousEditorItemPosition = editorItem.transform.position;

        editorItem.transform.parent = this.transform;
        editorItem.transform.localPosition = Vector3.zero;

        

    }

    public void ResetEditorItemPosition()
    {

        if(editorItem.transform.parent == this.transform)
        {
            editorItem.transform.parent = null;
            editorItem.transform.position = previousEditorItemPosition;
        }

    }

    #endregion
}
