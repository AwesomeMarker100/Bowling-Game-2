using UnityEngine;

[ExecuteInEditMode]
public class LinearLaserInteractor : MonoBehaviour
{
    [SerializeField] Vector3 laserStartOffset = Vector3.zero;
    [SerializeField] Vector3 direction = Vector3.forward;
    [SerializeField] float length = 2;
    [SerializeField] LineRenderer lineRenderer; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

        lineRenderer.useWorldSpace = false;
    }

    // Update is called once per frame
    void Update()
    {
        DrawLine();
    }


    private void DrawLine()
    {
        //<x, y, z> = start + t * direction
        lineRenderer.SetPosition(0, laserStartOffset);
        lineRenderer.SetPosition(1, laserStartOffset + length * direction);

    }

}
