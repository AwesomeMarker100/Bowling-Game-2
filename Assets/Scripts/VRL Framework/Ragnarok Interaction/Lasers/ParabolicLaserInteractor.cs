using UnityEngine;

public class ParabolicLaserInteractor : MonoBehaviour
{
    [SerializeField][Min(3)] float numVertices = 3f;

    [SerializeField] Vector3 laserStartOffset = Vector3.zero;
    [SerializeField] Vector3 vertexOffset = Vector3.forward + Vector3.up;

    [SerializeField] LineRenderer lineRenderer;

    private float a;

    private void Start()
    {
        if(laserStartOffset.x == vertexOffset.x || laserStartOffset.y == vertexOffset.y)
        {
            return;
        }
        a = laserStartOffset.y - vertexOffset.y / Mathf.Pow((laserStartOffset.x - vertexOffset.x), 2);

        if (lineRenderer == null) lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        
    }

    private void DrawParabola()
    { 
        if (lineRenderer == null) return;

        float t = 0;
        while (t < numVertices)
        {
            
        }
    }
}
