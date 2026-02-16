using UnityEngine;

public class WaveLaserInteractor : LaserInteractor
{
    [Min(0.01f)] [SerializeField] float frequency = 3;
    [SerializeField] float phaseAngle = 0;
    [Min(0.01f)][SerializeField] float amplitude = 1;
    protected override void Update()
    {
        base.Update();

        float t = 0;
        float tStep = maxLength / resolution;
        int i = 0;

        phaseAngle = phaseAngle % (2 * Mathf.PI);
        SetPositionCount(resolution);
        
        while(t < maxLength)
        {
            if (i == GetPositionCount()) break;
            SetPosition(i, new Vector3(0, amplitude * Mathf.Sin(2 * Mathf.PI * frequency * t + phaseAngle), t));
            i++;
            t += tStep;
        }
    }

}
