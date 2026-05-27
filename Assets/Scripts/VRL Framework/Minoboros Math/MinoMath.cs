using org.mariuszgromada.math.mxparser;
using System;
using System.Linq.Expressions;
using UnityEngine;
using Unity.Mathematics;
using org.mariuszgromada.math.mxparser.parsertokens;

public class MinoMath : MonoBehaviour
{

    //Precomputed Legendre Polynomial Roots (n = 1 to n = 10) 
    //Precomputed GLQ Legendre Weights
    static float[,] legendreInfo;
    static bool initializedLegendreInfo = false;
    const float THRESHOLD = 0.001f;
    const int MAX_ITER = 20;

    //import constants
    public const float e = 2.718281828f;
    
    static MinoMath()
    {
        PrecomputeLegendreInfo(1, 7);
        initializedLegendreInfo = true;

    }

    public static bool VApproximately(Vector3 v1, Vector3 v2, float threshold)
    {
        if (threshold < 0) throw new ArgumentException("Threshold must be positive!");
        return Vector3.Magnitude(v2 - v1) < threshold;
    }

    public static bool VApproximately(Vector3 v1, Vector3 v2)
    {
        return Vector3.Magnitude(v2 - v1) <= Mathf.Epsilon;
    }

    public static bool FApproximately(float f1, float f2, float threshold)
    {
        if (threshold < 0) throw new ArgumentException("Threshold must be positive!");

        return Mathf.Abs(f2 - f1) < threshold;
    }

    public static bool FApproximately(float f1, float f2)
    {
        return Mathf.Abs(f2 - f1) <= 10E-4;
    }

    public static bool Within(float val, float min, float max)
    {
        if (min > max || min == max) throw new ArgumentException("Min must be strictly less than max!");
        return val > min && val < max; 
    }




    //Numerical Integration Techniques

    //Simpson's Rule

    //Euler's Method

    //Newton-Raphson Method

    //Runge-Kutta Methods

   

    private static void PrecomputeLegendreInfo(int nStart, int nEnd)
    {

        float[] roots7 = GetLegendreRoots(7);

    }

    //Legendre Polynomial Helpers
    #region
    //note: even n is even function, odd n is odd functino
    public static float GetLegendrePolynomial(int n, float x)
    {
        if (n < 0)
        {
            print("Cannot generate negative Legendre Polynomials");
            return -1; 
        }

        if (n == 0)
            return 1;
        else if (n == 1)
            return x;
        else
            return ((2 * n - 1) * x * GetLegendrePolynomial(n - 1, x) - (n - 1) * GetLegendrePolynomial(n - 2, x)) / n;
    }

    public static float GetLegendreDerivative(int n, float x)
    {
        if (n < 0)
        {
            print("Cannot generate negative Legendre derivative Polynomials");
            return -1;
        }

        if (n == 0) return 0;
        else if (n == 1) return 1;
        return (2 * (n - 1) + 1) * GetLegendrePolynomial(n - 1, x) + GetLegendreDerivative(n - 2, x);
    }

    //returns Legendre roots from (-1, 1) 
    public static float[] GetLegendreRoots(int n)
    {
        if (n == 0) return null;
        if (n == 1) { return new float[1] { 0 }; }

        float[] roots = new float[n];

        int halfIdx = n / 2;

        //odd
        if (n % 2 != 0) roots[halfIdx] = 0;

        float initGuessStart = -0.99f;
        float initGuessOffset = (2 / n);

        //ADJUST LATER
        for(int i = 0; i < n; i++)
        {
            float xOld = initGuessStart;
            float xNew = Mathf.Infinity;

            int j = 0; 
            while(j < MAX_ITER)
            {
                xNew = xOld - (GetLegendrePolynomial(n, xOld) / GetLegendreDerivative(n, xOld));
                if (Mathf.Abs(xNew - xOld) < THRESHOLD)
                {
                    //if(xNew == )
                }
                xOld = xNew;
                j++;
            }

            roots[i] = xNew;
            initGuessStart = xNew + 0.1f;
        }

        return roots;
    }


    public static float GetLegendreWeight(int n, float xRoot)
    {
        return 2 / ((1 - Mathf.Pow(xRoot, 2)) * Mathf.Pow(GetLegendreDerivative(n, xRoot), 2));
    }




    #endregion

    /*
    public FMatrixSqr FormCompanionMat(string polynomial)
    {

    }

    //if polynomial is n-degree, coefficients matrix will be n + 1 length, companion matrix will be size n x n
    public FMatrixSqr FormCompanionMat(float[] coefficients)
    {

    }
    */

    /* 
     * Vector Operations
     * 
     */

    public static float[,] GetOuterProduct(float[] v1, float[] v2)
    {
        int l1 = v1.Length;
        int l2 = v2.Length;

        float[,] outerProduct = new float[l1, l2];

        for(int i = 0; i < l1; i++)
        {
            for(int j = 0; j < l2; j++)
            {
                outerProduct[i, j] = v1[i] * v2[j];
            }
        }

        return outerProduct;
    }
}
