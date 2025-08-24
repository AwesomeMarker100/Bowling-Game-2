using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//because we are better
public class UIRaycaster : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private RectTransform[] canvasElements;

    private void Start()
    {
        canvasElements = GetComponentsInChildren<RectTransform>();
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void FixedUpdate()
    {
        

    }
}
