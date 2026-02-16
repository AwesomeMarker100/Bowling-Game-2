using UnityEngine;

[ExecuteAlways]
public class LinearLaserInteractor : LaserInteractor
{
    private Vector3 endPoint;

    //resolution does not matter
    protected override void Update()
    {
        base.Update();

        this.resolution = 1;
        endPoint = Vector3.forward * maxLength;

        SetPositionCount(2);
        SetPosition(0, Vector3.zero);
        SetPosition(1, endPoint);
    }
}
