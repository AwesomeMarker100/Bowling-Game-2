using UnityEngine;

public class MinoMath : MonoBehaviour
{
    public static bool VApproximately(Vector3 v1, Vector3 v2, float threshold)
    {
        if(threshold < 0)
        {
            print("Threshold must be positive!");
            return false;
        }
        return Vector3.Magnitude(v2 - v1) < threshold;
    }

    public static bool VApproximately(Vector3 v1, Vector3 v2)
    {
        return Vector3.Magnitude(v2 - v1) <= Mathf.Epsilon;
    }

    public static bool FApproximately(float f1, float f2, float threshold)
    {
        if(threshold < 0)
        {
            print("Threshold must be positive!");
            return false;
        }

        return Mathf.Abs(f2 - f1) < threshold;
    }

    public static bool FApproximately(float f1, float f2)
    {
        return Mathf.Abs(f2 - f1) <= Mathf.Epsilon;
    }

    public static bool Within(float val, float min, float max)
    {
        if (min > max || min == max)
        {
            print("Min must be strictly lesser than max!");
            return false;
        }
        return val > min && val < max; 
    }






}
