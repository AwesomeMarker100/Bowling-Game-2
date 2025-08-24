using UnityEngine;

public struct Vec3
{
    public static FMatrix3x3 innerProduct = FMatrix3x3.identity;

    public float x;
    public float y;
    public float z;

    public static Vec3 Cross(Vec3 a, Vec3 b)
    {
        return new Vec3(a.y * b.z - a.z * b.y, -(a.x * b.z - a.z * b.x), a.x * b.y - a.y * b.x);
    }

    public static float Dot(Vec3 a, Vec3 b)
    {
        return a.x * b.x + a.y * b.y + a.z * b.z;
    }


    public Vec3(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;

    }
}
