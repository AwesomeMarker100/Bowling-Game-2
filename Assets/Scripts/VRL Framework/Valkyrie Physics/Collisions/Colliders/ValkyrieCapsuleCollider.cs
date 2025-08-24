
using UnityEditor;
using UnityEngine;

public class ValkyrieCapsuleCollider : ValkyrieCollider
{

    private Vector3 STD_LOCAL_CTR = Vector3.zero;
    private float STD_LOCAL_SCALE = 0.001f;
    private Vector3 STD_ROT_OFFSET = Vector3.zero;

    public enum Orientation
    {

        X,
        Y,
        Z

    }


    [Header("Dimensions")]

    [SerializeField][Min(0)] private float height = 2f;
    [SerializeField][Min(0)] private float radius = 1f;
    [SerializeField] [Min(0)] private float thickness = 2f;

    private bool offsetApplied = false;

    [Header("Transform")]
    [SerializeField] bool setDefault = false;
    [SerializeField][Min(0.001f)] float scaleFactor = 0.001f;
    [SerializeField] Vector3 rotation = Vector3.zero;


    [SerializeField] float offsetFactor = 0.01f;
    [SerializeField] Vector3 testDir = Vector3.zero;

    private float minAngle;
    private float maxAngle;

    private Matrix4x4 TRS;
    private Vector3 furthestPoint;

    public ValkyrieCapsuleCollider()
    {

        this.type = ColliderType.CapsuleCollider;

    }


    public override void SetBounds()
    {
        //setting a new basis
        TRS = Matrix4x4.TRS(globalCenter, transform.rotation * Quaternion.Euler(rotation), transform.localScale * scaleFactor);
        maxAngle = Mathf.Rad2Deg * Mathf.Acos(radius / Mathf.Sqrt(Mathf.Pow(radius, 2) + (Mathf.Pow(height, 2) / 16)));
    }

    public void OnValidate()
    {
        if(setDefault)
        {
            localCenter = STD_LOCAL_CTR;
            rotation = STD_ROT_OFFSET;
            scaleFactor = STD_LOCAL_SCALE;

            setDefault = false;
        } 
    }


    public override void DrawGizmos()
    {

        base.DrawGizmos();

        float inverseScale = 1 / scaleFactor;

        
        //create transform, rotation, scaling matrix s.t. we can use adjusted local coordinate basis that takes into account rotation!
        //setting a new basis 
        TRS = Matrix4x4.TRS(globalCenter, transform.rotation * Quaternion.Euler(rotation), transform.localScale * scaleFactor);
        Handles.matrix = TRS;
        Gizmos.matrix = TRS;

        float rewiredThickness = thickness * inverseScale;

        //drawing axes
        Handles.color = Color.blue;
        Handles.DrawLine(Vector3.zero, Vector3.forward * radius, rewiredThickness);

        Handles.color = Color.green;
        Handles.DrawLine(Vector3.zero, Vector3.up * radius, rewiredThickness);

        Handles.color = Color.red;
        Handles.DrawLine(Vector3.zero, Vector3.right * radius, rewiredThickness);

        Vector3 wireDiscCenter = Vector3.up * (height / 4);

        Vector3 p1 = wireDiscCenter + Vector3.forward * radius;
        Vector3 p2 = -wireDiscCenter + Vector3.forward * radius;

        Vector3 p3 = wireDiscCenter + Vector3.back * radius;
        Vector3 p4 = -wireDiscCenter + Vector3.back * radius;

        Vector3 p5 = wireDiscCenter + Vector3.right * radius;
        Vector3 p6 = -wireDiscCenter + Vector3.right * radius;

        Vector3 p7 = wireDiscCenter + Vector3.left * radius;
        Vector3 p8 = -wireDiscCenter + Vector3.left * radius;

        Handles.color = Color.black;
        Handles.DrawWireDisc(wireDiscCenter, Vector3.up, radius, rewiredThickness);
        Handles.DrawWireDisc(-wireDiscCenter, Vector3.down, radius, rewiredThickness);
        Handles.DrawLine(p1, p2, rewiredThickness);
        Handles.DrawLine(p3, p4, rewiredThickness);
        Handles.DrawLine(p5, p6, rewiredThickness);
        Handles.DrawLine(p7, p8, rewiredThickness);

        Handles.DrawWireArc(wireDiscCenter, Vector3.forward, Vector3.right * radius, 180, radius, rewiredThickness);
        Handles.DrawWireArc(wireDiscCenter, Vector3.right, Vector3.forward * radius, -180, radius, rewiredThickness);

        Handles.DrawWireArc(-wireDiscCenter, Vector3.forward, Vector3.right * radius, -180, radius, rewiredThickness);
        Handles.DrawWireArc(-wireDiscCenter, Vector3.right, Vector3.forward * radius, 180, radius, rewiredThickness);

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(Vector3.zero, radius / 10);

        Gizmos.matrix = Matrix4x4.identity;
        Handles.matrix = Matrix4x4.identity;

        if(testDir != Vector3.zero)
        {
            maxAngle = Mathf.Rad2Deg * Mathf.Acos(radius / Mathf.Sqrt(Mathf.Pow(radius, 2) + (Mathf.Pow(height, 2) / 16)));
            Gizmos.color = Color.cyan;
            Handles.color = Color.cyan;

            testDir.Normalize();
            Handles.DrawLine(globalCenter, globalCenter + testDir * radius, thickness);

            //print(TRS.MultiplyVector(globalCenter + testDir * radius));

            //something is in world coords
            Vector3 something = GetFurthestPoint(testDir);
            Gizmos.DrawSphere(something, radius / 9);
        }

    }

    //https://www.desmos.com/3d/ecck9khnd7 - shows work

    //dir is given in world coords
    public override Vector3 GetFurthestPoint(Vector3 dir)
    {

        //given a world direction, we want to find the corresponding vector in our new basis 
        //TRS * dir

        dir = TRS.inverse.MultiplyVector(dir).normalized; //we are now in TRS space
        Vector3 compVec = new Vector3(dir.x, 0, dir.z).normalized;

        float a = height / 4;
        float t = 0;
        float angle = Mathf.Rad2Deg * Mathf.Acos(Vector3.Dot(dir, compVec));

        if(angle <= maxAngle)
        {
            //we are in the middle cylinder portion
            t = radius / Mathf.Sqrt(Mathf.Pow(dir.x, 2) + Mathf.Pow(dir.z, 2));

        } else
        {
            if(dir.y >= 0)
            {
                //top hemisphere
                float discriminant = Mathf.Pow(a, 2) * (Mathf.Pow(dir.y, 2) - 1) + Mathf.Pow(radius, 2);

                if(discriminant < 0)
                {
                    return Vector3.zero;
                } 

                t = a * dir.y + Mathf.Sqrt(discriminant);
            } else
            {

                float discriminant = Mathf.Pow(a, 2) * (Mathf.Pow(dir.y, 2) - 1) + Mathf.Pow(radius, 2);

                if(discriminant < 0)
                {
                    return Vector3.zero;
                }

                //bottom hemisphere
                t = -a * dir.y + Mathf.Sqrt(discriminant);
            }

        }

        return TRS.MultiplyPoint3x4(dir * t);
    }

    
    
}
