using JetBrains.Annotations;
using System;
using Unity.VisualScripting;
using UnityEngine;

public struct FMatrix3x3
{

    private static readonly float[,] zero = new float[3, 3] { { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 } };
    public static readonly float[,] identity = new float[3, 3] { { 1, 0, 0 }, { 0, 1, 0 }, { 0, 0, 1 } };

    private float minimumVal;
    public int rank;

    private float[] eigenValues;
    private Vector3[] eigenSpace;

    private float determinant;
    public float[,] arr;

    //For PLU factorization
    public float[,] permutationMatrix;
    public float[,] lowerTriangular;
    public float[,] upperTriangular;

    //for cholesky factorization
    public float[,] choleskyLowerTriangular;
    public float[,] choleskyUpperTriangular;

    public float[,] transpose;

    float[] row0;
    float[] row1;
    float[] row2;

    public static Vector3 operator *(FMatrix3x3 a, Vector3 b)
    {
        return new Vector3(a[0, 0] * b[0] + a[0, 1] * b[1] + a[0, 2] * b[2], a[1, 0] * b[0] + a[1, 1] * b[1] + a[1, 2] * b[2], a[2, 0] * b[0] + a[2, 1] * b[1] + a[2, 2] * b[2]);
    }

    public static FMatrix3x3 operator *(FMatrix3x3 a, FMatrix3x3 b)
    {
        return new FMatrix3x3(a[0, 0] * b[0, 0] + a[0, 1] * b[1, 0] + a[0, 2] * b[2, 0], a[0, 0] * b[0, 1] + a[0, 1] * b[1, 1] + a[0, 2] * b[2, 1],
            a[0, 0] * b[0, 2] + a[0, 1] * b[1, 2] + a[0, 2] * b[2, 2], a[1, 0] * b[0, 0] + a[1, 1] * b[1, 0] + a[1, 2] * b[2, 0], 
            a[1, 0] * b[0, 1] + a[1, 1] * b[1, 1] + a[1, 2] * b[2, 1], a[1, 0] * b[0, 2] + a[1, 1] * b[1, 2] + a[1, 2] * b[2, 2],
            a[2, 0] * b[0, 0] + a[2, 1] * b[1, 0] + a[2, 2] * b[2, 0], a[2, 0] * b[0, 1] + a[2, 1] * b[1, 1] + a[2, 2] * b[2, 1],
            a[2, 0] * b[0, 2] + a[2, 1] * b[1, 2] + a[2, 2] * b[2, 2]);
    }

    public static FMatrix3x3 operator +(FMatrix3x3 a, FMatrix3x3 b)
    {
        return new FMatrix3x3(a[0, 0] + b[0, 0], a[0, 1] + b[0, 1], a[0, 2] + b[0, 2], a[1, 0] + b[1, 0], a[1,1] + b[1, 1], a[1, 2] + b[1, 2],
            a[2, 0] + b[2, 0], a[2,1] + b[2, 1], a[2, 2] + b[2, 2]);
    }

    public static FMatrix3x3 operator -(FMatrix3x3 a, FMatrix3x3 b)
    {
        return new FMatrix3x3(a[0, 0] - b[0, 0], a[0, 1] - b[0, 1], a[0, 2] - b[0, 2], a[1, 0] - b[1, 0], a[1, 1] - b[1, 1], a[1, 2] - b[1, 2],
            a[2, 0] - b[2, 0], a[2, 1] - b[2, 1], a[2, 2] - b[2, 2]);
    }

    public static implicit operator float[,](FMatrix3x3 a)
    {
        return a.arr;
    }

    public static implicit operator FMatrix3x3(float[,] a)
    {
        return new FMatrix3x3(a[0, 0], a[0, 1], a[0, 2], a[1, 0], a[1, 1], a[1, 2], a[2, 0], a[2, 1], a[2, 2]);
    }

    public static bool operator ==(FMatrix3x3 a, FMatrix3x3 b)
    {
        return a[0, 0] == b[0, 0] && a[0, 1] == b[0, 1] && a[0, 2] == b[0, 2] && a[1, 0] == b[1, 0] && a[1, 1] == b[1, 1] && a[1, 2] == b[1, 2]
            && a[2, 0] == b[2, 0] && a[2, 1] == b[2, 1] && a[2, 2] == b[2, 2];
    }

    public static bool operator !=(FMatrix3x3 a, FMatrix3x3 b)
    {
        return !(a[0, 0] == b[0, 0] && a[0, 1] == b[0, 1] && a[0, 2] == b[0, 2] && a[1, 0] == b[1, 0] && a[1, 1] == b[1, 1] && a[1, 2] == b[1, 2]
            && a[2, 0] == b[2, 0] && a[2, 1] == b[2, 1] && a[2, 2] == b[2, 2]);
    }

    //public static bool operator ==(FMatrix3x3 a, FMatrix3x3 b) => a.Equals(b);
   // public static bool operator !=(FMatrix3x3 a, FMatrix3x3 b) => !a.Equals(b);


    //for upper triangular matrices 


    public FMatrix3x3(float a00, float a01, float a02, float a10, float a11, float a12, float a20, float a21, float a22)
    {
        arr = new float[3,3];

        arr[0,0] = a00;
        arr[0,1] = a01;
        arr[0,2] = a02;
        arr[1,0] = a10;
        arr[1,1] = a11;
        arr[1, 2] = a12;
        arr[2, 0] = a20;
        arr[2, 1] = a21;
        arr[2, 2] = a22;

        row0 = new float[] { a00, a01, a02 };
        row1 = new float[] { a10, a11, a12 };
        row2 = new float[] { a20, a21, a22 };

        upperTriangular = new float[3, 3];
        lowerTriangular = new float[3, 3] { { 1, 0, 0 }, { 0, 1, 0 }, { 0, 0, 1 } };
        permutationMatrix = new float[3, 3];
        choleskyLowerTriangular = null;
        choleskyUpperTriangular = null;

        transpose = new float[3, 3];

        eigenValues = null;
        eigenSpace = null;
        determinant = 0;
        rank = 0;

        minimumVal = 1E-2f;



        RefreshMatrix();

    }

    private void SetDeterminant()
    {
        determinant = upperTriangular[0, 0] * upperTriangular[1, 1] * upperTriangular[2, 2];
    }

    private void SetEigenValues()
    {

    }



    /// <summary>
    /// Performs arr[secondRow] = arr[secondRow] + arr[firstRow] * c;
    /// </summary>
    /// <param name="matrix"></param>
    /// <param name="firstRow"></param>
    /// <param name="secondRow"></param>
    /// <param name="c"></param>
    public void AddRows(float[,] matrix, int firstRow, int secondRow, float c)
    {
        matrix[secondRow, 0] += c * matrix[firstRow, 0];
        matrix[secondRow, 1] += c * matrix[firstRow, 1];
        matrix[secondRow, 2] += c * matrix[firstRow, 2];
    }

    public void RefreshMatrix()
    {

        PLUFactorize();
        SetDeterminant();
        SetTranspose();
        SetEigenValues();
    }

    public float this[int i1, int i2]
    {
        get => arr[i1, i2];
        set { 
            arr[i1, i2] = value;
            RefreshMatrix();
        }
        
    }

    //should do QR Factorization Instead but PLU works pretty much the same efficiency for this small 3x3 

    //A = QR -> det(A) = det(Q) * det(R). we know QQ^T = I so det(QQ^T) = 1 ->  (detQ)^2 = 1 -> detQ = +- 1, det(R) = product of diagonals
    //learned from APPM 3310 - Matrix Methods and Applications
    private void PLUFactorize()
    {
        //if (this == FMatrix3x3.zero) upp
        Array.Copy(arr, upperTriangular, 9);

        permutationMatrix = new float[3, 3]
        {
            {1, 0, 0 } ,
            {0, 1, 0 },
            {0, 0, 1 }
        };

        for(int j = 0; j < 2; j++) //iterate over columns
        {

            if (upperTriangular[j, j] == 0)
            {
                for(int i = j + 1; i < 3; i++)
                {
                    if (upperTriangular[i, j] != 0)
                    {
                        SwapRows(upperTriangular, j, i);
                        SwapRows(permutationMatrix, j, i);

                        //swap rows under diagonal only for lower triangular
                        if (i == 0 || j == 0) break;

                        float a = lowerTriangular[1, 0];
                        lowerTriangular[1, 0] = lowerTriangular[2, 0];
                        lowerTriangular[2, 0] = a;
                        break;
                    }
                }
            }

            for (int i = j + 1; i < 3; i++) //iterate over rows above jth column
            {
                float c = -upperTriangular[i, j] / upperTriangular[j, j];

                AddRows(upperTriangular, j, i, c);
                //AddRows(lowerTriangular, j, i, -c);

                if(i == 1 && j == 0)
                {
                    lowerTriangular[1, 0] += -c;
                }

                if(i == 2 && j == 1)
                {
                    lowerTriangular[2, 1] += -c;

                }

                if (i == 2 && j == 0)
                {
                    lowerTriangular[2, 0] += -c;
                }
                    
                if (Mathf.Abs(upperTriangular[i, 0]) < minimumVal) upperTriangular[i, 0] = 0;
                if (Mathf.Abs(upperTriangular[i, 1]) < minimumVal) upperTriangular[i, 1] = 0;
                if (Mathf.Abs(upperTriangular[i, 2]) < minimumVal) upperTriangular[i, 2] = 0;

                if (Mathf.Abs(lowerTriangular[i, 0]) < minimumVal) lowerTriangular[i, 0] = 0;
                if (Mathf.Abs(lowerTriangular[i, 1]) < minimumVal) lowerTriangular[i, 1] = 0;
                if (Mathf.Abs(lowerTriangular[i, 2]) < minimumVal) lowerTriangular[i, 2] = 0;
            }
            
        }


    }

    public bool IsBasis()
    {
        return determinant != 0;
    }

    public Vector3[] GetKernel()
    {
        return new Vector3[3];
    }

    public Vector3[] GetImage()
    {
        return new Vector3[3];
    }

    private Vector3 SolveSystem(Vector3 b)
    {
        //PLUx = b
        //LUx = P^T * b
        b  = (FMatrix3x3)GetTranspose(permutationMatrix) * b;
        b = SolveSystemLowerTriangular(lowerTriangular, b);
        return SolveSystemUpperTriangular(upperTriangular, b);
    }

    //ONLY PASS IN UPPER TRIANGULAR MATRIX
    private Vector3 SolveSystemUpperTriangular(float[,] upperTri, Vector3 b)
    {

        float zComp = b.z / upperTri[2, 2];
        float yComp = (b.y - upperTri[1, 2] * zComp) / upperTri[1, 1];
        float xComp = (b.x - upperTri[0, 1] * yComp - upperTri[0, 2] * zComp) / upperTri[0, 0];

        return new Vector3(xComp, yComp, zComp);
    }


    private Vector3 SolveSystemLowerTriangular(float[,] lowerTri, Vector3 b)
    {
        float xComp = b[0] / lowerTri[0, 0];
        float yComp = (b[1] - lowerTri[1, 0] * xComp) / lowerTri[1, 1];
        float zComp = (b[2] - lowerTri[2, 0] * xComp - lowerTri[2, 1] * yComp) / lowerTri[2, 2];

        return new Vector3(xComp, yComp, zComp);

    }

    public FMatrix3x3 GetInverse()
    {
        if (determinant == 0) return FMatrix3x3.zero; //not invertible

        float[,] pTranspose = GetTranspose(permutationMatrix);

        Vector3 yCol1 = SolveSystemLowerTriangular(lowerTriangular, new Vector3(pTranspose[0, 0], pTranspose[1, 0], pTranspose[2, 0]));//new Vector3(pTranspose[0, 0], pTranspose[1, 0], pTranspose[2, 0]));
        Vector3 yCol2 = SolveSystemLowerTriangular(lowerTriangular, new Vector3(pTranspose[1, 1], pTranspose[1, 1], pTranspose[2, 1]));
        Vector3 yCol3 = SolveSystemLowerTriangular(lowerTriangular, new Vector3(pTranspose[2, 2], pTranspose[1, 2], pTranspose[2, 2]));

        Vector3 inverseCol1 = SolveSystemUpperTriangular(upperTriangular, yCol1);
        Vector3 inverseCol2 = SolveSystemUpperTriangular(upperTriangular, yCol2);
        Vector3 inverseCol3 = SolveSystemUpperTriangular(upperTriangular, yCol3);



        //check master tablet for proof
        return new FMatrix3x3(inverseCol1.x, inverseCol2.x, inverseCol3.x, inverseCol1.y, inverseCol2.y, inverseCol3.y, inverseCol1.z, inverseCol2.z, inverseCol3.z);
    }


    private void SetTranspose()
    {
        transpose[0, 0] = arr[0, 0]; //, arr[1, 0], arr[2, 0], arr[0, 1], arr[1, 1], arr[2, 1], arr[0, 2], arr[1, 2], arr[2, 2] };
        transpose[0, 1] = arr[1, 0];
        transpose[0, 2] = arr[2, 0];
        transpose[1, 0] = arr[0, 1];
        transpose[1, 1] = arr[1, 1];
        transpose[1, 2] = arr[2, 1];
        transpose[2, 0] = arr[0, 2];
        transpose[2, 1] = arr[1, 2];
        transpose[2, 2] = arr[2, 2];
    }

    //LU Factorization

    //LDLT Factorization

    //QR Factorization

    public void QRFactorize()
    {


    }

    public void SwapRows(float[,] arr, int firstRow, int secondRow)
    {
        float t1 = arr[firstRow, 0];
        float t2 = arr[firstRow, 1];
        float t3 = arr[firstRow, 2];

        arr[firstRow, 0] = arr[secondRow, 0];
        arr[firstRow, 1] = arr[secondRow, 1];
        arr[firstRow, 2] = arr[secondRow, 2];

        arr[secondRow, 0] = t1;
        arr[secondRow, 1] = t2;
        arr[secondRow, 2] = t3;

    }

    public float[] GetEigenValues()
    {
        return new float[4];

    }


    public Vector3[] GetEigenVectors()
    {
        return new Vector3[4];

    }

   

    public readonly override string ToString()
    {
        return "[" + arr[0, 0] + ", " + arr[0, 1] + ", " + arr[0, 2] + "] " + 
            "[" + arr[1, 0] + ", " + arr[1, 1] + ", " + arr[1, 2] + "] " + 
            "[" + arr[2, 0] + ", " + arr[2, 1] + ", " + arr[2, 2] + "]";
    }


    public static float[,] GetTranspose(float[,] matrix)
    {
        float[,] transpose = new float[3, 3];


        transpose[0, 0] = matrix[0, 0]; //, arr[1, 0], arr[2, 0], arr[0, 1], arr[1, 1], arr[2, 1], arr[0, 2], arr[1, 2], arr[2, 2] };
        transpose[0, 1] = matrix[1, 0];
        transpose[0, 2] = matrix[2, 0];
        transpose[1, 0] = matrix[0, 1];
        transpose[1, 1] = matrix[1, 1];
        transpose[1, 2] = matrix[2, 1];
        transpose[2, 0] = matrix[0, 2];
        transpose[2, 1] = matrix[1, 2];
        transpose[2, 2] = matrix[2, 2];

        return transpose;
    }


    //L(LT)x = b where LT is L transpose
    //let Y = (LT)x
    //solve LY = b with forward sub 
    //solve (LT)x = Y with back sub
    //done 
    public Vector3 SolveSystemCholesky(Vector3 b)
    {
        Vector3 Y = SolveSystemLowerTriangular(choleskyLowerTriangular, b);
        return SolveSystemUpperTriangular(choleskyUpperTriangular, Y);
    }


    //CHOLESKY DECOMPOSITION
    public bool CholeskyDecomp()
    {
        if ((FMatrix3x3)GetTranspose(arr) != (FMatrix3x3)arr) return false;
        //attempt cholesky

        if (upperTriangular[0, 0] < 0 || upperTriangular[1, 1] < 0 || upperTriangular[2, 2] < 0) return false;

        //we can do cholesky factorization

        float[,] squareRootDiagonalMatrix = new float[3, 3]
        {
            { Mathf.Sqrt(upperTriangular[0, 0]), 0, 0 },
            { 0, Mathf.Sqrt(upperTriangular[1, 1]), 0},
            { 0, 0, Mathf.Sqrt(upperTriangular[2, 2])}
        };

        //should probably combine and replace with just one upperTriangular, lowerTriangular set
        choleskyLowerTriangular = (FMatrix3x3)lowerTriangular * (FMatrix3x3)squareRootDiagonalMatrix;
        choleskyUpperTriangular = GetTranspose(choleskyLowerTriangular);
        return true;
    }

}
