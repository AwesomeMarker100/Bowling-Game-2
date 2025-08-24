using JetBrains.Annotations;
using System.Linq;
using UnityEngine;

public class ConvexDecomposer : MonoBehaviour
{
   
    public float GetHausdorffDistance(Vector3[] A, Vector3[] B)
    {

        if(A.Length != B.Length)
        {
            print("Given Point Sets A and B do not have the same length!");
            return -1;
        }

        float[] infimaaB = new float[A.Length];
        float[] infimabA = new float[B.Length];


        for(int i = 0; i < A.Length; i++)
        {
            infimaaB[i] = GetInfimum(A[i], B);
        }

        for(int i = 0; i < B.Length; i++)
        {
            infimabA[i] = GetInfimum(B[i], A);
        }


        return Mathf.Max(infimaaB.Max(), infimabA.Max());
    }

    public float GetInfimum(Vector3 a, Vector3[] B)
    {
        float infimum = float.PositiveInfinity;

        for(int i = 0; i < B.Length; i++)
        {
            float dist = Vector3.Distance(a, B[i]);

            if(dist < infimum)
            {
                infimum = Vector3.Distance(a, B[i]);
            }
        }

        return infimum;
    }

}
