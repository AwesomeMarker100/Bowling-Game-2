using JetBrains.Annotations;
using NUnit.Framework.Constraints;
using org.mariuszgromada.math.mxparser.parsertokens;
using System;
using Unity.VisualScripting;
using UnityEngine;
using System.Runtime.CompilerServices;


[Serializable]
public struct FMatrix3x3
{

    private Vec3[] matArr;


    //index in range helper
    public static bool InRange(int i) => i is >= 0 and <= 2;

    //common static values
    #region
    public static FMatrix3x3 I => new(Vec3.e1, Vec3.e2, Vec3.e3);
    public static FMatrix3x3 Zero => new(Vec3.zero, Vec3.zero, Vec3.zero);
    #endregion


    //expression bodied properties
    public FMatrix3x3 Transpose => new(new Vec3(this[0, 0], this[1, 0], this[2, 0]), new Vec3(this[0, 1], this[1, 1], this[2, 1]), new Vec3(this[0, 2], this[1, 2], this[2, 2]));
    public FMatrix3x3 Inverse => GetInverse();

    public bool IsIllConditioned => throw new NotImplementedException();

    public bool IsUpperTriangular => this[1, 0] == 0 && this[2, 0] == 0 && this[2, 1] == 0;
    public bool IsLowerTriangular => this[0, 1] == 0 && this[0, 2] == 0 && this[1, 2] == 0;
    

    public bool IsOrthogonal => Transpose * this == I;
    public float Determinant => GetDeterminant();

    public bool IsSingular => Determinant == 0;



    //operators
    #region

    //equality
    #region
    public static bool operator ==(FMatrix3x3 a, FMatrix3x3 b) => a[0] == b[0] && a[1] == b[1] && a[2] == b[2];
    public static bool operator !=(FMatrix3x3 a, FMatrix3x3 b) => !(a == b);
    #endregion

    //indexing
    #region
    public float this[int i, int j]
    {
        get
        {
            return i is >= 0 and <= 2 && j is >= 0 and <= 2 ? matArr[i][j] : throw new ArgumentOutOfRangeException("Indices must be between 0 and 2 inclusive");
        }

        set
        {
            if (i is >= 0 and <= 2 && j is >= 0 and <= 2) matArr[i][j] = value;
            else throw new ArgumentOutOfRangeException();
        }
    }

    public Vec3 this[int i]
    {
        get
        {
            return i is >= 0 and <= 2 ? matArr[i] : throw new ArgumentOutOfRangeException("Index must be between 0 and 2 inclusive!");
        }

        set
        {
            if (i is >= 0 and <= 2) matArr[i] = value;
            else throw new ArgumentOutOfRangeException();
        }
    }

    #endregion

    //scalar multiplication / division
    #region
    public static FMatrix3x3 operator *(FMatrix3x3 a, float s) => new(s * a[0], s * a[1], s * a[2]);
    public static FMatrix3x3 operator *(float s, FMatrix3x3 a) => a * s;

    public static FMatrix3x3 operator /(FMatrix3x3 a, float s) => new(a[0] / s, a[1] / s, a[2] / s);

    #endregion

    //vector and matrix multiplication
    #region
    public static Vec3 operator *(FMatrix3x3 a, Vec3 v) => new Vec3(Vec3.Dot(a[0], v), Vec3.Dot(a[1], v), Vec3.Dot(a[2], v)); 
    public static FMatrix3x3 operator *(FMatrix3x3 a, FMatrix3x3 b)
    {
        FMatrix3x3 bT = b.Transpose;

        float c00 = Vec3.Dot(a[0], bT[0]);
        float c01 = Vec3.Dot(a[0], bT[1]);
        float c02 = Vec3.Dot(a[0], bT[2]);

        float c10 = Vec3.Dot(a[1], bT[0]);
        float c11 = Vec3.Dot(a[1], bT[1]);
        float c12 = Vec3.Dot(a[1], bT[2]);

        float c20 = Vec3.Dot(a[2], bT[0]);
        float c21 = Vec3.Dot(a[2], bT[1]);
        float c22 = Vec3.Dot(a[2], bT[2]);

        return new FMatrix3x3(new Vec3(c00, c01, c02), new Vec3(c10, c11, c12), new Vec3(c20, c21, c22));
    }
    #endregion

    //addition and subtraction
    #region
    public static FMatrix3x3 operator +(FMatrix3x3 a, FMatrix3x3 b) => new(a[0] + b[0], a[1] + b[1], a[2] + b[2]);
    public static FMatrix3x3 operator -(FMatrix3x3 a, FMatrix3x3 b) => new(a[0] - b[0], a[1] - b[1], a[2] - b[2]);
    #endregion

    #endregion

    //constructors
    #region
    public FMatrix3x3(Vec3 r0, Vec3 r1, Vec3 r2)
    {
        matArr = new Vec3[3] { r0, r1, r2 };
    }

    #endregion

    //expression bodied functions
    public void SwapRows(int i, int j) => (this[i], this[j]) = i != j && InRange(i) && InRange(j) ? (this[j], this[i]) : throw new ArgumentOutOfRangeException("Indices must be between 0 and 2 inclusive!");



    //inverse
    public FMatrix3x3 GetInverse()
    {
        if (this == I) return I;
        if (IsOrthogonal) return Transpose;
        if (IsSingular) return Zero;

        float det = Determinant;

        //using adjugate method -- note inv_{ij} = det(M_{ji}) / det
        float inv00 = (this[1, 1] * this[2, 2] - this[1, 2] * this[2, 1]) / det;
        float inv01 = -(this[0, 1] * this[2, 2] - this[2, 1] * this[0, 2]) / det;
        float inv02 = (this[0, 1] * this[1, 2] - this[1, 1] * this[0, 2]) / det;

        float inv10 = -(this[1, 0] * this[2, 2] - this[1, 2] * this[2, 0]) / det;
        float inv11 = (this[0, 0] * this[2, 2] - this[2, 0] * this[0, 2]) / det;
        float inv12 = -(this[0, 0] * this[1, 2] - this[0, 2] * this[1, 0]) / det;

        float inv20 = (this[1, 0] * this[2, 1] - this[1, 1] * this[2, 0]) / det;
        float inv21 = -(this[0, 0] * this[2, 1] - this[2, 0] * this[0, 1]) / det;
        float inv22 = (this[0, 0] * this[1, 1] - this[0, 1] * this[1, 0]) / det;


        return new(new Vec3(inv00, inv01, inv02), new Vec3(inv10, inv11, inv12), new Vec3(inv20, inv21, inv22));
    }

   

    public float GetDeterminant()
    {
        //deal with ill conditioned case later
        if (IsIllConditioned) return 0;
        if (this == Zero) return 0;
        if (this == I) return 1;
        if (IsUpperTriangular || IsLowerTriangular) return this[0, 0] * this[1, 1] * this[2, 2];

        //"determinant cofactor 1" - using column based determinant
        float detCof1 = this[1, 1] * this[2, 2] - this[1, 2] * this[2, 1];
        float detCof2 = this[0, 1] * this[2, 2] - this[2, 1] * this[0, 2];
        float detCof3 = this[0, 1] * this[1, 2] - this[1, 1] * this[0, 2];

        return this[0, 0] * detCof1 - this[1, 0] * detCof2 + this[2, 0] * detCof3;
    }

    public readonly override bool Equals(object obj) => obj is FMatrix3x3 x && x == this;
    public readonly override int GetHashCode() => base.GetHashCode();
    
}