using NUnit.Framework.Constraints;
using UnityEditor.AssetImporters;
using UnityEngine;
public struct Vec3
{
    public float x { get; private set; }
    public float y { get; private set; }
    public float z { get; private set; }

    public float magnitude { get; private set; }

    public static Vec3 zero => new Vec3(0, 0, 0);
    public static Vec3 one => new Vec3(1, 1, 1);

    //elementary rows
    public static Vec3 e1 => new Vec3(1, 0, 0);
    public static Vec3 e2 => new Vec3(0, 1, 0);
    public static Vec3 e3 => new Vec3(0, 0, 1);

    //indexing
    #region
    public float this[int i]
    {
        get
        {
           switch(i)
            {
                case 0: 
                    return x;
                case 1: 
                    return y;
                case 2: 
                    return z;
                default:
                    throw new System.ArgumentOutOfRangeException();
            }
        }

        set
        {
            switch(i)
            {
                case 0:
                    SetX(value);
                    break;
                case 1:
                    SetY(value);
                    break;
                case 2:
                    SetZ(value);
                    break;
                default:
                    throw new System.ArgumentOutOfRangeException();
            }       
        }
    }

    #endregion
    /*
     * Operator Overloads
     */

    #region

    public static Vec3 operator *(float a, Vec3 v)
    {
        return new Vec3(a * v.x, a * v.y, a * v.z);
    }

    public static Vec3 operator *(Vec3 v, float a)
    {
        return a * v;
    }

    public static Vec3 operator /(Vec3 v, float s) => new(v.x / s, v.y / s, v.z / s);
    public static Vec3 operator +(Vec3 v1, Vec3 v2)
    {
        return new Vec3(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);
    }

    public static Vec3 operator -(Vec3 v)
    {
        return new Vec3(-v.x, -v.y, -v.z);
    }

    public static Vec3 operator -(Vec3 v1, Vec3 v2)
    {
        return v1 + (-v2);
    }

    //NOT THE DOT OR CROSS PRODUCT OR WEDGE PRODUCT
    public static Vec3 operator *(Vec3 v1, Vec3 v2)
    {
        return new Vec3(v1.x * v2.x, v1.y * v2.y, v1.z * v2.z);
    }

    public static Vec3 operator /(Vec3 v1, Vec3 v2)
    {
        return new Vec3(v1.x / v2.x, v1.y / v2.y, v1.z / v2.z);
    }

    public static bool operator ==(Vec3 v1, Vec3 v2)
    {
        return v1.x == v2.x && v1.y == v2.y && v1.z == v2.z;
    }

    public static bool operator !=(Vec3 v1, Vec3 v2)
    {
        return !(v1 == v2);
    }

    #endregion

    /*
     * 
     * Static Functions
     * 
     */
    public static Vec3 Cross(Vec3 a, Vec3 b)
    {
        return new Vec3(a.y * b.z - a.z * b.y, -(a.x * b.z - a.z * b.x), a.x * b.y - a.y * b.x);
    }

    public static float Dot(Vec3 a, Vec3 b)
    {
        return a.x * b.x + a.y * b.y + a.z * b.z;
    }
       

    public static float Angle(Vec3 a, Vec3 b)
    {
        float dotProd = Dot(a, b);
        return Mathf.Acos(dotProd / (a.magnitude * b.magnitude));
    }


    //both a and b are length 3
   /* public static FMatrix3x3 OuterProd(Vec3 a, Vec3 b)
    {
        return new FMatrix3x3(a.x * b.x, a.x * b.y, a.x * b.z, a.y * b.x, a.y * b.y, a.y * b.z, a.z * b.x, a.z * b.y, a.z * b.z);
    }
   */

    public void SetX(float newX)
    {
        if (x == newX) return;
       
        x = newX;
        magnitude = Mathf.Sqrt(Mathf.Pow(x, 2) + Mathf.Pow(y, 2) + Mathf.Pow(z, 2));
    }

    public void SetY(float newY)
    {
        if (y == newY) return;

        y = newY;
        magnitude = Mathf.Sqrt(Mathf.Pow(x, 2) + Mathf.Pow(y, 2) + Mathf.Pow(z, 2));
    }

    public void SetZ(float newZ)
    {
        if (z == newZ) return;

        z = newZ;
        magnitude = Mathf.Sqrt(Mathf.Pow(x, 2) + Mathf.Pow(y, 2) + Mathf.Pow(z, 2));
    }



    public Vec3(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;

       magnitude = Mathf.Sqrt(Mathf.Pow(x, 2) + Mathf.Pow(y, 2) + Mathf.Pow(z, 2));
    }


    public float Norm1 => Mathf.Abs(x) + Mathf.Abs(y) + Mathf.Abs(z);

    public override string ToString()
    {
        return $"({x}, {y}, {z})";
    }

}
