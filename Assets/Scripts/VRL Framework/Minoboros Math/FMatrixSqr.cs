using NUnit.Framework.Constraints;
using UnityEngine;
using Unity.Mathematics;
using Unity.Burst;
using TMPro.EditorUtilities;
public struct FMatrixSqr
{

    public readonly int N;
    public readonly float[,] mat;

    public readonly float[,] identity;

    public FMatrixSqr(int N, float[,] mat)
    {
        this.N = N;
        this.mat = mat;

        this.identity = new float[N, N];

        for (int i = 0; i < N; i++)
        {
            identity[i, i] = 1;
        }
    }


    private bool CheckValidPos((int, int) pos)
    {
        if(pos.Item1 < 0)
        {
            Debug.LogWarning("Row index cannot be less than 0!");
            return false;
        } else if(pos.Item1 > N - 1)
        {
            Debug.LogWarning("Row index cannot be greater than N - 1!");
            return false;
        }

        if(pos.Item2 < 0)
        {
            Debug.LogWarning("Column index cannot be less than 0!");
            return false;
        } else if(pos.Item2 > N - 1)
        {
            Debug.LogWarning("Column index cannot be greater than N - 1!");
            return false;
        }

        return true;

    }

    public FMatrixSqr(int N)
    {
        this.N = N;
        this.mat = new float[N, N];
        this.identity = new float[N, N];

        for(int i = 0; i < N; i++)
        {
            identity[i, i] = 1;
        }
    }

    public bool Put((int, int) pos, float val)
    {
        if (!CheckValidPos(pos)) return false;
        
        int i = pos.Item1;
        int j = pos.Item2;

        mat[i, j] = val;
        return true;
    }
    
    public float Dot(float[] v1, float[] v2)
    {
        float dotProd = 0; 

        for(int i = 0; i < N; i++)
        {
            dotProd += v1[i] * v2[i];
        }

        return dotProd;
    }

    public void FillRow(int rowIdx, float val)
    {
        for (int i = 0; i < N; i++)
        {
            mat[rowIdx, i] = val;
        }
    }

    public void FillCol(int colIdx, float val)
    {
        for(int i = 0; i < N; i++)
        {
            mat[i, colIdx] = val;
        }
    }
    /*
    public float[] GetEigenValues()
    {

    }

    public Vector3[] GetEigenVectors()
    {

    
    }
    */

    private float GetMagnitudeNVec(float[] vec)
    {
        float sumSqr = 0;

        for (int i = 0; i < N; i++)
        {
            sumSqr += math.pow(vec[i], 2);
        }

        return math.sqrt(sumSqr);
    }

    private float[] NormalizeNVec(float[] vec)
    {
        float mag = GetMagnitudeNVec(vec);
        
        for(int i = 0; i < N; i++)
        {
            vec[i] /= mag;
        }

        return vec;
    }

    public float[] GetColumn(int n)
    {
        float[] col = new float[n];

        for(int i = 0; i < N;i++)
        {
            col[i] = mat[i, n];
        }

        return col;
    }

    public float[] GetRow(int n)
    {
        float[] row = new float[n];

        for(int j = 0; j < N; j++)
        {
            row[j] = mat[n, j];
        }

        return row;
    }
   
    

    //uses Householder's QR Algorithm
    /*public (float[,] Q, float[,] R) GetQRFactorization()
    {
       
        for(int i = 0; i < N; i++)
        {
            float[] v_i = GetColumn(i);
            NormalizeNVec(v_i);

        }
    }*/
}
