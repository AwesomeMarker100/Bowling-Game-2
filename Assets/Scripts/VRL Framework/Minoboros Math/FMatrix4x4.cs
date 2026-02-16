using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public struct FMatrix4x4
{
    //row wise storage for matrix
    private Vec4[] matArr;

    //Indexing
    #region
    public float this[int i, int j]
    {
        get
        {
            return i is >= 0 and <= 3 && j is >= 0 and <= 3 ? matArr[i][j] : throw new ArgumentOutOfRangeException("i and j must be between 0 and 3 inclusive!");
        }

        set
        {
            if (i is >= 0 and <= 3 && j is >= 0 and <= 3) matArr[i][j] = value;
            else throw new ArgumentOutOfRangeException("i and j must be between 0 and 3 inclusive!");
        }
    }

    public Vec4 this[int i]
    {
        get
        {
            return i is >= 0 and <= 3 ? matArr[i] : throw new ArgumentOutOfRangeException("i must be between 0 and 3 inclusive!");
        }

        set
        {
            if (i is >= 0 and <= 3) matArr[i] = value;
            else throw new ArgumentOutOfRangeException("i must be between 0 and 3 inclusive!");
        }
    }
    #endregion

    //Decomposition - PLU, Cholesky, QR 
    #region
    public (FMatrix4x4, FMatrix4x4, FMatrix4x4) PLU => PLUDecompose();
    public (FMatrix4x4, FMatrix4x4) Cholesky => CholeskyDecompose();

    #endregion

    


    //Expression Based Properties
    public bool IsUpperTriangular => IsUpperTriangularHelper();
    public bool IsLowerTriangular => GetTranspose().IsUpperTriangularHelper();
    public float Determinant => CalcDeterminant();
    public bool IsSingular => Determinant == 0;
    public bool IsSymmetric => this == GetTranspose();

    public FMatrix4x4 Inverse => CalcInverse();

    public FMatrix4x4 Transpose => GetTranspose();

    //Operator Overloads
    #region

    //Equality Overloads
    #region
    public static bool operator ==(FMatrix4x4 m1, FMatrix4x4 m2)
    {
        return (m1.matArr[0] == m2.matArr[0] && m1.matArr[1] == m2.matArr[1] && m1.matArr[2] == m2.matArr[2] && m1.matArr[3] == m2.matArr[3]);
    }

    public static bool operator !=(FMatrix4x4 m1, FMatrix4x4 m2)
    {
        return !(m1 == m2);
    }
    #endregion

    //Basic Operators
    #region

    public static FMatrix4x4 operator +(FMatrix4x4 mat1, FMatrix4x4 mat2) => new FMatrix4x4(mat1[0] + mat2[0], mat1[1] + mat2[1], mat1[2] + mat2[2], mat1[3] + mat2[3]);
 

    public static FMatrix4x4 operator -(FMatrix4x4 mat1, FMatrix4x4 mat2) => new FMatrix4x4(mat1[0] - mat2[0], mat1[1] - mat2[1], mat1[2] - mat2[2], mat1[3] - mat2[3]);


    public static FMatrix4x4 operator -(FMatrix4x4 mat) => -1 * mat;
   
    public static Vector4 operator *(FMatrix4x4 mat, Vector4 v)
    {
        Vec4[] matrix = mat.matArr;

        float v1 = Vec4.Dot(matrix[0], v);
        float v2 = Vec4.Dot(matrix[1], v);
        float v3 = Vec4.Dot(matrix[2], v);
        float v4 = Vec4.Dot(matrix[3], v);

        return new Vector4(v1, v2, v3, v4);
    }

    public static Vec4 operator *(FMatrix4x4 mat, Vec4 v)
    {
        Vec4[] matrix = mat.matArr;

        float v1 = Vec4.Dot(matrix[0], v);
        float v2 = Vec4.Dot(matrix[1], v);
        float v3 = Vec4.Dot(matrix[2], v);
        float v4 = Vec4.Dot(matrix[3], v);

        return new Vec4(v1, v2, v3, v4);
    }

    public static FMatrix4x4 operator *(FMatrix4x4 a, FMatrix4x4 b)
    {

        FMatrix4x4 bTranspose = b.GetTranspose();

        float c00 = Vec4.Dot(a[0], bTranspose[0]);
        float c01 = Vec4.Dot(a[0], bTranspose[1]);
        float c02 = Vec4.Dot(a[0], bTranspose[2]);
        float c03 = Vec4.Dot(a[0], bTranspose[3]);

        float c10 = Vec4.Dot(a[1], bTranspose[0]);
        float c11 = Vec4.Dot(a[1], bTranspose[1]);
        float c12 = Vec4.Dot(a[1], bTranspose[2]);
        float c13 = Vec4.Dot(a[1], bTranspose[3]);

        float c20 = Vec4.Dot(a[2], bTranspose[0]);
        float c21 = Vec4.Dot(a[2], bTranspose[1]);
        float c22 = Vec4.Dot(a[2], bTranspose[2]);
        float c23 = Vec4.Dot(a[2], bTranspose[3]);

        float c30 = Vec4.Dot(a[3], bTranspose[0]);
        float c31 = Vec4.Dot(a[3], bTranspose[1]);
        float c32 = Vec4.Dot(a[3], bTranspose[2]);
        float c33 = Vec4.Dot(a[3], bTranspose[3]);


        return new FMatrix4x4(new Vec4(c00, c01, c02, c03), new Vec4(c10, c11, c12, c13), new Vec4(c20, c21, c22, c23), new Vec4(c30, c31, c32, c33));



    }

    public static FMatrix4x4 operator *(float scalar, FMatrix4x4 a) => new FMatrix4x4(a[0] * scalar, a[1] * scalar, a[2] * scalar, a[3] * scalar);


    public static FMatrix4x4 operator *(FMatrix4x4 a, float scalar) => scalar * a;
    #endregion

    #endregion

    //Common 4x4 Matrices (I, Zero)
    #region
    public static FMatrix4x4 I => new FMatrix4x4(Vec4.e1, Vec4.e2, Vec4.e3, Vec4.e4);
        

    
    public static FMatrix4x4 Zero => new FMatrix4x4(Vec4.zero, Vec4.zero, Vec4.zero, Vec4.zero);
        
    
    #endregion

    //Constructor
    #region
    public FMatrix4x4(Vec4 r0, Vec4 r1, Vec4 r2, Vec4 r3)
    {

        matArr = new Vec4[] { r0, r1, r2, r3 };

    }



    #endregion

    private bool IsUpperTriangularHelper() => this[1, 0] == 0 && this[2, 0] == 0 && this[2, 1] == 0 && this[3, 0] == 0 && this[3, 1] == 0 && this[3, 2] == 0;

    //Calculating Determinant

    //im not doing the cofactor method and sort of "bruteforcing" it bc its only 4x4 and also FMatrix3x3 needs refining
    public float CalcDeterminant()
    {
        FMatrix4x4 upperTriangular = PLU.Item3;

        float det = upperTriangular[0, 0] * upperTriangular[1, 1] * upperTriangular[2, 2] * upperTriangular[3, 3];
        return det;
    }

    //Getting RREF
    #region




    #endregion

    //Check if Cholesky Satisfied


    //Cholesky Decomposition


    //QR Decomposition

    //Solve (given b vector) for Ax = b

    //Helper Functions

    private static int GetBestPivotRowIdx(FMatrix4x4 matrix, int pivotIdx)
    {
        int bestPivotRowIdx = pivotIdx;
        float maxAbsPivotVal = Mathf.Abs(matrix[pivotIdx, pivotIdx]);

        for (int i = pivotIdx + 1; i < 4; i++)
        {
            float absPivot = Mathf.Abs(matrix[i, pivotIdx]);
            if (absPivot > maxAbsPivotVal) {
                bestPivotRowIdx = i;
                maxAbsPivotVal = absPivot;
            } else if (absPivot == maxAbsPivotVal)
            {
                if (matrix[i].Get1Norm() > matrix[bestPivotRowIdx].Get1Norm())
                {
                    bestPivotRowIdx = i;
                    maxAbsPivotVal = absPivot;
                }
            }
        }

        return bestPivotRowIdx;
    }
    //Helper Functions
    #region
    private static void SwapRows(ref FMatrix4x4 matrix, int rowIdx1, int rowIdx2)
    {

        //any inappropriate indices and corner cases
        if (rowIdx1 < 0 || rowIdx1 > 3 || rowIdx2 < 0 || rowIdx2 > 3)
        {
            Debug.LogError("Row Indices must be in between 0-3 inclusive.");
            return;
        }

        if (rowIdx1 == rowIdx2) return;

        //basic swap
        Vec4 temp = matrix[rowIdx1];
        matrix[rowIdx1] = matrix[rowIdx2];
        matrix[rowIdx2] = temp;
    }

    #endregion
    private void PLUSwapRows(ref FMatrix4x4 P, ref FMatrix4x4 L, ref FMatrix4x4 U, int rowIdx1, int rowIdx2)
    {
        //P and U swap normally, L only swaps below the diagonal
        SwapRows(ref P, rowIdx1, rowIdx2);
        SwapRows(ref U, rowIdx1, rowIdx2);

        //if you're swapping the i'th and j'th rows for L where i < j, then only elements matrix[i][0-i) and matrix[j][0-i) will be swapped.
        int minRowIdx = -1;
        int otherRowIdx = -1; 

        if(rowIdx1 < rowIdx2)
        {
            minRowIdx = rowIdx1;
            otherRowIdx = rowIdx2;
        } else
        {
            minRowIdx = rowIdx2;
            otherRowIdx = rowIdx1;
        }
        
        Vec4 minRowL1 = L[minRowIdx];
        Vec4 otherRow = L[otherRowIdx];

        for(int k = 0; k < rowIdx1; k++)
        {
            float temp = L[minRowIdx, k];
            L[minRowIdx, k] = L[otherRowIdx, k];
            L[otherRowIdx, k] = temp;
        }
    }

    
    private (FMatrix4x4 P, FMatrix4x4 L, FMatrix4x4 U) PLUDecompose()
    {
        FMatrix4x4 U = MakeCopy(this);
        FMatrix4x4 L = I;
        FMatrix4x4 P = I;

        for (int i = 0; i < 3; i++)
        {
            int bestPivotRowIdx = GetBestPivotRowIdx(U, i);

            if (bestPivotRowIdx != i)
            {
                PLUSwapRows(ref P, ref L, ref U, i, bestPivotRowIdx);
            }

            float bestPivotVal = U[i, i];
            if (bestPivotVal == 0) continue;

            for (int j = i + 1; j < 4; j++)
            {
                float scaleFactor = U[j, i] / bestPivotVal;
                U[j] = U[j] - scaleFactor * U[i];

                for (int k = i; k < 4; k++)
                {
                    L[j, k] = L[j, k] + scaleFactor * L[i, k];
                }
            }


        }

        return (P, L, U);

    }

    private (FMatrix4x4 L, FMatrix4x4 S) CholeskyDecompose()
    {
        if (!IsSymmetric) return (FMatrix4x4.Zero, FMatrix4x4.Zero);

        (FMatrix4x4, FMatrix4x4, FMatrix4x4) PLU = this.PLU;

        FMatrix4x4 U = PLU.Item3;
        if (U[0, 0] > 0 && U[1, 1] > 0 && U[2, 2] > 0 && U[3, 3] > 0) //valid for Cholesky Decomp
        {
            FMatrix4x4 S = new FMatrix4x4(new Vec4(Mathf.Sqrt(U[0, 0]), 0, 0, 0), new Vec4(0, Mathf.Sqrt(U[1, 1]), 0, 0), 
                new Vec4(0, 0, Mathf.Sqrt(U[2, 2]), 0), new Vec4(0, 0, 0, Mathf.Sqrt(U[3, 3])));

            FMatrix4x4 LS = PLU.Item2 * S;
            return (LS, LS.GetTranspose());

        }

        return (FMatrix4x4.Zero, FMatrix4x4.Zero);
    }


    #region



    public Vec4[] Solve(Vec4 b)
    {

        //return CholeskyDecompose ? SolveQR(b) : SolvePLU(b);
        return new Vec4[0];
    }


    //better in efficincy in general
    public Vec4[] SolvePLU(Vec4 b)
    {

        return new Vec4[3];

    }


    //QR Helpers

    public Vec4 GetElementaryRow(int rowIdx)
    {
        switch(rowIdx)
        {
            case 0:
                return Vec4.e1;
            case 1:
                return Vec4.e2;
            case 2:
                return Vec4.e3;
            case 3:
                return Vec4.e4;
            default:
                return Vec4.infinity;
        }

    }

    public void QRDecompose()
    {
        FMatrix4x4[] hMatrices = new FMatrix4x4[3];
        FMatrix4x4 A = GetTranspose();

        for (int i = 0; i < 3; i++)
        {

            Vec4 e_i = GetElementaryRow(i);
            Vec4 v_i = A[i];
            //zero out the 0th - (i-1)th entries
            for(int j = 0; j < i; j++)
            {
                v_i[j] = 0;
            }



            float alpha = A[i, i] >= 0 ? -Mathf.Abs(v_i.magnitude) : Mathf.Abs(v_i.magnitude);

            Vec4 w_i = v_i == Vec4.zero ? Vec4.zero : v_i - alpha * e_i;
            FMatrix4x4 projMatrix = w_i == Vec4.zero ? FMatrix4x4.Zero : Vec4.GetProjectionMatrix(w_i);

            hMatrices[i] = I - 2 * projMatrix;
            


            A = A * hMatrices[i];
        }

        FMatrix4x4 R = A.GetTranspose();
        FMatrix4x4 Q = hMatrices[0] * hMatrices[1] * hMatrices[2];

        FMatrix4x4 final = Q * R;
        final.ApplyFunction((float a) => Mathf.Round(a));
        Debug.Log(final);
    }


    //better for ill-conditioned systems
    public Vec4[] SolveQR(Vec4 b)
    {

        QRDecompose();
        return new Vec4[3];
    }


    #endregion

    //hard-coded since small
    public FMatrix4x4 GetTranspose()
    {

        Vec4 transposeRow1 = new Vec4(this[0, 0], this[1,0], this[2, 0], this[3, 0]);
        Vec4 transposeRow2 = new Vec4(this[0, 1], this[1, 1], this[2, 1], this[3, 1]);
        Vec4 transposeRow3 = new Vec4(this[0, 2], this[1, 2], this[2, 2], this[3, 2]);
        Vec4 transposeRow4 = new Vec4(this[0, 3], this[1, 3], this[2, 3], this[3, 3]);

        return new FMatrix4x4(transposeRow1, transposeRow2, transposeRow3, transposeRow4);

    }

    public static FMatrix4x4 MakeCopy(FMatrix4x4 toCopy) => new FMatrix4x4(toCopy[0], toCopy[1], toCopy[2], toCopy[3]);

    


    public void ApplyFunction(Func<float, float> fn)
    {
        for(int i = 0; i < 4; i++)
        {
            for(int j = 0; j < 4; j++)
            {
                this[i, j] = fn(this[i, j]);
            }
        }
    }

    public override string ToString()
    {
        return $"{matArr[0]}, {matArr[1]}, {matArr[2]}, {matArr[3]}";
    }

    //Use as inner product matrix (perform x^T A y) 
    public void SetRow(int i, Vec4 row)
    {
        this[i] = row;
    }

     
    public FMatrix4x4 CalcInverseUpperTriangular()
    {
        if (Determinant == 0) return Zero;
        FMatrix4x4 inverse = Zero;

        for(int j = 0; j < 4; j++)
        {
            
            inverse[j, j] = 1 / this[j, j];

            for(int k = j - 1; k >= 0; k--)
            {
                float coeff = -1 / this[k, k];
                float sum = 0;

                for(int m = k; m < j; m++)
                {
                    sum += this[k, m + 1] * inverse[m + 1, j];
                }

                inverse[k, j] = coeff * sum;
            }
        }

        return inverse;

    }
    
    public FMatrix4x4 CalcInverseLowerTriangular()
    {
        if (Determinant == 0) return Zero;

        Debug.Log("here");
        FMatrix4x4 upperTriangular = GetTranspose();
        return upperTriangular.CalcInverseUpperTriangular().GetTranspose();
    }
    
    public FMatrix4x4 CalcInverse()
    {
        if (Determinant == 0) return Zero; //no inverse exists

        if (IsUpperTriangular) return CalcInverseUpperTriangular();
        else if (IsLowerTriangular) return CalcInverseLowerTriangular();

        FMatrix4x4 U_inverse = PLU.Item3.CalcInverseUpperTriangular();
        FMatrix4x4 L_inverse = PLU.Item2.CalcInverseLowerTriangular();

        return U_inverse * L_inverse * PLU.Item1;

    }

    public override bool Equals(object obj) => obj is FMatrix4x4 x && x == this;
    public override int GetHashCode() => base.GetHashCode();


}

