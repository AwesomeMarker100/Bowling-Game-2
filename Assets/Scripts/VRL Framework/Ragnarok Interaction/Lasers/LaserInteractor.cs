using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[ExecuteAlways]
public class LaserInteractor : MonoBehaviour
{


    [SerializeField] Gradient color;

    [SerializeField] protected Vector3 startOffset;
    [SerializeField] protected Vector2 rotationOffset; 
    [Min(0.001f)][SerializeField] protected float scaleFactor = 1;
    [Min(1)][SerializeField] protected int resolution = 1; //# of t_steps per 1 unit distance

    [SerializeField] protected bool doCollisions = false;
    [Min(0.01f)][SerializeField] protected float maxLength = 1f;
    [Min(0.01f)][SerializeField] protected float thickness = 0.3f;

    private LineRenderer lineRenderer;

    protected Vector3 origin; //transform.position + startOffset
    protected Quaternion rotation; //transform.rotation * Quaternion.Euler(rotationOffset)
    protected Vector3 scale; //transform.localScale * scaleFactor
    protected Matrix4x4 TRS;


    protected virtual void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.useWorldSpace = true;

        lineRenderer.colorGradient = color;
        if(doCollisions)
        {
            if (GetComponent<Valkyrie2DRectCollider>() == null)
            {
                gameObject.AddComponent(typeof(Valkyrie2DRectCollider));
            }
        }

    }

    protected virtual void Update()
    {
        origin = transform.position + startOffset;
        rotation = transform.rotation * Quaternion.Euler(rotationOffset);
        scale = transform.localScale * scaleFactor;

        
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        //lineRenderer.colorGradient = color;

        TRS = Matrix4x4.TRS(origin, rotation, scale);
        lineRenderer.SetWidth(thickness, thickness);

        
    }

    //v given in local TRS coordinates
    protected virtual void SetPosition(int i, Vector3 v)
    {
        lineRenderer.SetPosition(i, TRS.MultiplyPoint(v));
    }

    protected virtual int GetPositionCount()
    {
        return lineRenderer.positionCount;
    }

    protected virtual void SetPositionCount(int count)
    {
        lineRenderer.positionCount = count;
    }

    protected Vector3 GetPosition(int i)
    {
        return TRS.inverse.MultiplyPoint(lineRenderer.GetPosition(i));
    }


    protected Vector3 GetAvgPoint(Vector3 a, Vector3 b)
    {
        return (a + b) / 2;
    }

    protected void AdaptiveOptimize(float threshold)
    {
        //essentially like iterative merge sort

        int leftInit = 0;
        int rightInit = lineRenderer.positionCount - 1;

        int leftIdx;
        int rightIdx;
        int midIdx;
        Vector3 leftPt;
        Vector3 rightPt;
        Vector3 midPt;

        float error;

        List<int> startStop = new List<int>() { leftInit, rightInit };

        List<int> belowThresholdIndices = new List<int>();

        for (int i = 0; startStop.Count > i; i += 2)
        {
            leftIdx = startStop[i];
            rightIdx = startStop[i + 1];


            midIdx = (leftIdx + rightIdx) / 2;

            //need at least one point between the points we're analyzing
            if (midIdx == i || midIdx == i + 1) continue;

            leftPt = lineRenderer.GetPosition(leftIdx);
            rightPt = lineRenderer.GetPosition(rightIdx);
            midPt = lineRenderer.GetPosition(midIdx);

            error = Vector3.Magnitude(GetAvgPoint(leftPt, rightPt) - midPt);
            if (error < threshold)
            {
                //iterate over all pts between left and right and discard
                belowThresholdIndices.Add(leftIdx);
                belowThresholdIndices.Add(rightIdx);
            } else
            {
                //splitting into two just like merge sort
                //left to mid
                startStop.Add(leftIdx);
                startStop.Add(midIdx);

                //mid to right
                startStop.Add(midIdx);
                startStop.Add(rightIdx);
            }
        }
         

        
    }

    protected void AdjustDensity(List<int> belowThresholdIndices)
    {
        //belowThresholdIndices has length 2n where n is an integer
        int leftIdx = belowThresholdIndices[0];
        int rightIdx = belowThresholdIndices[1];
        int nextIdx = belowThresholdIndices[1] + 1;

        Vector3 rightPt;


        for(int i = 0; i < belowThresholdIndices.Count; i++)
        {
            leftIdx = belowThresholdIndices[i];
            rightIdx = belowThresholdIndices[i + 1];

            rightPt = lineRenderer.GetPosition(rightIdx);
            lineRenderer.SetPosition(leftIdx + 1, rightPt);
        }
    }
}
