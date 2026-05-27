using System;
using org.mariuszgromada.math.mxparser.parsertokens;
using System.Text;
using UnityEngine;

[Serializable]
public struct Vec4
{
    // Private backing fields - setting serializefield on these temporarily
    [SerializeField] private float _x;
    [SerializeField] private float _y;
    [SerializeField] private float _z;
    [SerializeField] private float _w;

    //Float Values
    #region
    public float x {
        get { return _x; }
        set { SetX(value); }
    }

    public float y {
        get { return _y; }
        set { SetY(value); }
    }

    public float z {
        get { return _z; }
        set { SetZ(value); }
    }

    public float w {
        get { return _w; }
        set { SetW(value); }
    }

    public float magnitude { get; private set; }
    #endregion

    //Common Vec4 Values
    #region
    public static Vec4 zero  {
        get
        {
            return new Vec4(0f, 0f, 0f, 0f);
        }
    } 
    public static Vec4 one {
        get
        {
            return new Vec4(1f, 1f, 1f, 1f);
        }
    }


    public static Vec4 e1 {
        
        get
        {
            return new Vec4(1f, 0f, 0f, 0f);
        }
    }

    public static Vec4 e2  {
        get
        {
            return new Vec4(0f, 1f, 0f, 0f);
        }
    }
    public static Vec4 e3 {
        get
        {
            return new Vec4(0f, 0f, 1f, 0f);
        }
    }

    public static Vec4 e4
    { 
        get
        {
            return new Vec4(0f, 0f, 0f, 1f);
        }
    }

    public static Vec4 infinity
    {
        get
        {
            return new Vec4(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);
        }
    }

    #endregion

    //Vec4 Operator Overloads
    #region

    //Indexing
    #region
    public float this[int i]
    {
        get
        {
            switch (i)
            {
                case 0: return x;
                case 1: return y;
                case 2: return z;
                case 3: return w;
                default: throw new System.IndexOutOfRangeException("Invalid Vec4 index!");
            }
        }

        set
        {
            switch(i)
            {
                case 0:
                    x = value;
                    break;
                case 1:
                    y = value;
                    break;
                case 2:
                    z = value;
                    break;
                case 3:
                    w = value;
                    break;
                default: throw new System.IndexOutOfRangeException("Invalid Vec4 index!");
            }
        }
    }
    #endregion

    //Equality Overloads
    #region
    public static bool operator ==(Vec4 v1, Vec4 v2)
    {
        return (v1.x == v2.x && v1.y == v2.y && v1.z == v2.z && v1.w == v2.w);
    }

    public static bool operator !=(Vec4 v1, Vec4 v2)
    {
        return !(v1 == v2);
    }
    #endregion

    //Multiplication Overloads
    #region
    public static Vec4 operator *(float a, Vec4 v)
    {
        return new Vec4(a * v.x, a * v.y, a * v.z, a * v.w);
    }

    public static Vec4 operator *(Vec4 v, float a)
    {
        return a * v; 
    }

    #endregion

    //Addition, Subtraction, and Component-wise Multiplication/Division Overloads
    #region
    public static Vec4 operator +(Vec4 v1, Vec4 v2)
    {
        return new Vec4(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z, v1.w + v2.w);
    }

    public static Vec4 operator -(Vec4 v)
    {
        return new Vec4(-v.x, -v.y, -v.z, -v.w);
    }

    public static Vec4 operator -(Vec4 v1, Vec4 v2)
    {
        return v1 + (-v2);
    }

    //NOT THE DOT OR CROSS PRODUCT OR WEDGE PRODUCT
    public static Vec4 operator *(Vec4 v1, Vec4 v2)
    {
        return new Vec4(v1.x * v2.x, v1.y * v2.y, v1.z * v2.z, v1.w * v2.w);
    }

    public static Vec4 operator /(Vec4 v1, Vec4 v2)
    {
        float v3_0;
        float v3_1;
        float v3_2;
        float v3_3;
        //Divide by Zero Checks, sets to Inf or Negative Inf if so
        #region
        if (v2.x == 0)
        {
            if (v1.x >= 0) v3_0 = Mathf.Infinity;
            else v3_0 = Mathf.NegativeInfinity;
        } else
        {
            v3_0 = v1.x / v2.x;
        }

        if (v2.y == 0)
        {
            if (v1.y >= 0) v3_1 = Mathf.Infinity;
            else v3_1 = Mathf.NegativeInfinity;
        } else
        {
            v3_1 = v1.y / v2.y;
        }

        if (v2.z == 0)
        {
            if (v1.z >= 0) v3_2 = Mathf.Infinity;
            else v3_2 = Mathf.NegativeInfinity;

        }
        else
        {
            v3_2 = v1.z / v2.z;
        }

        if (v2.w == 0)
        {
            if (v1.w >= 0) v3_3 = Mathf.Infinity;
            else v3_3 = Mathf.NegativeInfinity;

        }
        else
        {
            v3_3 = v1.w / v2.w;
        }
        #endregion

        return new Vec4(v3_0, v3_1, v3_2, v3_3);
    }

    public static Vec4 operator +(Vec3 v1, Vec4 v2)
    {
        return new Vec4(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z, v2.w);
    }
    #endregion

    #endregion

    //Constructor
    #region
    public Vec4(float x, float y, float z, float w)
    {
        _x = x;
        _y = y;
        _z = z;
        _w = w;
        magnitude = Mathf.Sqrt(Mathf.Pow(_x, 2) + Mathf.Pow(_y, 2) + Mathf.Pow(_z, 2) + Mathf.Pow(_w, 2));
    }

    #endregion

    //Dot Product
    #region
    public static float Dot(Vec4 a, Vec4 b)
    {
        return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
    }

    public static float Dot(Vec4 a, Vector4 b)
    {
        return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
    }

    public static float Dot(Vector4 a, Vec4 b)
    {
        return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
    }
    #endregion


    //Outer Product
    #region
    public static FMatrix4x4 Outer(Vec4 a, Vec4 b)
    {
        return new FMatrix4x4(
             new Vec4(a.x * b.x, a.x * b.y, a.x * b.z, a.x * b.w), 
             new Vec4(a.y * b.x, a.y * b.y, a.y * b.z, a.y * b.w),
             new Vec4(a.z * b.x, a.z * b.y, a.z * b.z, a.z * b.w),
             new Vec4(a.w * b.x, a.w * b.y, a.w * b.z, a.w * b.w)
        );
    }
    #endregion

    //Changing Values
    #region

    private void SetMagnitude()
    {
        magnitude = Mathf.Sqrt(Mathf.Pow(_x, 2) + Mathf.Pow(_y, 2) + Mathf.Pow(_z, 2) + Mathf.Pow(_w, 2));
    }

    public void SetX(float newX)
    {
        if (_x == newX) return;

        _x = newX;
        SetMagnitude();
    }

    public void SetY(float newY)
    {
        if (_y == newY) return;

        _y = newY;
        SetMagnitude();
    }

    public void SetZ(float newZ)
    {
        if (_z == newZ) return;

        _z = newZ;
        SetMagnitude();
    }

    public void SetW(float newW)
    {
        if (_w == newW) return;

        _w = newW;
        SetMagnitude();
    }


    #endregion
    
    
    //To Vec3 
    #region
    public Vec3 ToVec3()
    {
        return new Vec3(x, y, z);
    }

    public Vec3 ToVec3(int omitIdx)
    {
        switch (omitIdx)
        {
            case 0:
                return new Vec3(y, z, w);
            case 1:
                return new Vec3(x, z, w);
            case 2:
                return new Vec3(x, y, w);
            default:
                return ToVec3();
        }
    }

    #endregion

    //Get Norms
    #region
    public float Get1Norm()
    {

        return Mathf.Abs(x) + Mathf.Abs(y) + Mathf.Abs(z) + Mathf.Abs(w);

       
    }
    #endregion


    //To String
    #region
    public override string ToString()
    {
        return $"({x}, {y}, {z}, {w})";
    }
    #endregion

    //Normalize
    public void Normalize()
    {
        if (magnitude == 0) return;
        _x /= magnitude;
        _y /= magnitude;
        _z /= magnitude;
        _w /= magnitude;
    }

    public static FMatrix4x4 GetProjectionMatrix(Vec4 v)
    {
        v.Normalize();
        return Outer(v, v);
    }

}
