using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class HittableObject : MonoBehaviour
{
    

    [Header("Borders")]
    public Vector3 localCenter;
    public float xSize;
    public float ySize;

    public Color hoverColor;
    public Color normalColor;

    public UnityEvent onHover;

    //LOCAL COORDINATES

    [HideInInspector] public Vector3 topLeft;
    [HideInInspector] public Vector3 topRight;
    [HideInInspector] public Vector3 bottomLeft;
    [HideInInspector] public Vector3 bottomRight;

     public bool isBeingHovered = false;

    private Image image;

    public void OnDrawGizmosSelected()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        InitializePoints();

        Gizmos.color = Color.blue;

        Gizmos.DrawSphere(localCenter, 6);
        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(bottomLeft, bottomRight);
        Gizmos.DrawLine(topLeft, bottomLeft);
        Gizmos.DrawLine(bottomRight, topRight);
    }

    public virtual void Start()
    {
        image = gameObject.GetComponent<Image>();
        normalColor = image.color;
    }

    private void Update()
    {
        InitializePoints();
    }

    private void InitializePoints()
    {
        localCenter = transform.rotation * localCenter;

        topLeft = localCenter + Vector3.left * (xSize / 2) + Vector3.up * (ySize / 2);
        topRight = localCenter + Vector3.right * (xSize / 2) + Vector3.up * (ySize / 2);
        bottomLeft = localCenter + Vector3.left * (xSize / 2) + Vector3.down * (ySize / 2);
        bottomRight = localCenter + Vector3.right * (xSize / 2) + Vector3.down * (ySize / 2);

    }
    public virtual bool PointInBounds(Vector3 point)
    {
        Vector3 globalTopRight = transform.TransformPoint(topRight);
        Vector3 globalTopLeft = transform.TransformPoint(topLeft);
        Vector3 globalBottomLeft = transform.TransformPoint(bottomLeft);

        Vector3 topSide = globalTopLeft - globalTopRight;
        Vector3 leftSide = globalTopLeft - globalBottomLeft;

        Vector3 leftDiff = point - globalTopLeft;
        Vector3 rightDiff = point - globalTopRight;
        Vector3 bottomDiff = point - globalBottomLeft;

        bool isRightOfLeftSide = Vector3.Dot(topSide.normalized, leftDiff.normalized) < 0;
        bool isLeftOfRightSide = Vector3.Dot(topSide.normalized, rightDiff.normalized) > 0;

        bool isBelowTopSide = Vector3.Dot(leftSide.normalized, leftDiff.normalized) < 0;
        bool isAboveBottomSide = Vector3.Dot(leftSide.normalized, bottomDiff.normalized) > 0;

        
        return isRightOfLeftSide && isLeftOfRightSide && isBelowTopSide && isAboveBottomSide && Vector3.Angle(leftDiff, globalBottomLeft - point) > 169f;

    }

    public virtual void ActivateHover()
    {
        isBeingHovered = true;
        onHover.Invoke();


        if (image != null) image.color = hoverColor;
        //image.color = hoverColor;

    }

    public virtual void DeactivateHover()
    {

        isBeingHovered = false;
        if (image != null) image.color = normalColor;

    }
    

}
