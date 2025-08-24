using UnityEngine;

public class RotateObjectNode : SprigganNode
{
    public GameObject gameObject;
    public Vector3 rotationEulerAngles;

    public override async Awaitable Execute()
    {
        if(gameObject == null)
        {
            Debug.LogWarning("GameObject assigned is null!");
            return;
        }

        gameObject.transform.rotation *= Quaternion.Euler(rotationEulerAngles);
        await Awaitable.EndOfFrameAsync();
    }
}
