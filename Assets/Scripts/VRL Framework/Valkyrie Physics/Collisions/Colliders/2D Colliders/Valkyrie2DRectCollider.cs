using System.Linq.Expressions;
using UnityEditor;
using UnityEngine;
using static VPhys;
public class Valkyrie2DRectCollider : ValkyrieCollider
{
    [SerializeField] Vector2 size;
    [SerializeField] Vector3 rotation;
    [SerializeField][Min(0.001f)] float normalLength = 1.5f;
    [SerializeField] Color color = Color.black;

    [SerializeField] bool flipNormal = false;

    [SerializeField] float thickness = 120f;
    [SerializeField][Min(0.001f)] float scaleFactor = 0.2f;


    Matrix4x4 TRS;


    private Vector3 bottomRightCorner;
    private Vector3 topRightCorner;
    private Vector3 bottomLeftCorner;
    private Vector3 topLeftCorner;

    private Vector3 normal;

    public BoundedRect plane;

    public override void DrawGizmos()
    {
        base.DrawGizmos();
        SetBounds();

        Gizmos.color = color;
        Gizmos.matrix = TRS;

        Handles.color = color;
        Handles.matrix = TRS;

        using (new Handles.DrawingScope(Color.violetRed))
        {
            Gizmos.DrawSphere(Vector3.zero, 0.3f);
            Handles.DrawLine(Vector3.zero, normal * normalLength, thickness / scaleFactor);
        }

        using (new Handles.DrawingScope(Color.cadetBlue))
        {
            Handles.DrawLine(Vector3.zero, Vector3.forward * normalLength, thickness / scaleFactor);
        }

        using (new Handles.DrawingScope(Color.darkSeaGreen))
        {
            Handles.DrawLine(Vector3.zero, Vector3.up * normalLength, thickness / scaleFactor);
        }

        using (new Handles.DrawingScope(Color.darkRed))
        {
            Handles.DrawLine(Vector3.zero, Vector3.right * normalLength, thickness / scaleFactor);
        }

        Handles.DrawLine(bottomLeftCorner, bottomRightCorner, thickness / scaleFactor);
        Handles.DrawLine(bottomRightCorner, topRightCorner, thickness / scaleFactor);
        Handles.DrawLine(bottomLeftCorner, topLeftCorner, thickness / scaleFactor);
        Handles.DrawLine(topLeftCorner, topRightCorner, thickness / scaleFactor);
    }


    public override void SetBounds()
    {
        TRS = Matrix4x4.TRS(globalCenter, transform.rotation * Quaternion.Euler(rotation), transform.localScale * scaleFactor);


        bottomRightCorner = new Vector3(size.x, -size.y, 0);
        bottomLeftCorner = new Vector3(-size.x, -size.y, 0);
        topLeftCorner = new Vector3(-size.x, size.y, 0);
        topRightCorner = new Vector3(size.x, size.y, 0);

        plane = new BoundedRect(TRS.MultiplyPoint3x4(bottomLeftCorner), TRS.MultiplyPoint3x4(topLeftCorner), TRS.MultiplyPoint3x4(bottomRightCorner));
        normal = plane.normal * normalLength;
        if (flipNormal) normal *= -1;
    }
}
