using System;
using UnityEngine;

[ExecuteInEditMode]
public class EditorTest : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        float[] v1 = new float[] { 5, 2};
        float[] v2 = new float[] { 2, 3, 4 };

        float[,] outerProd = MinoMath.GetOuterProduct(v1, v2);

        for(int i = 0; i < v1.Length; i++)
        {
            for(int j = 0; j < v2.Length; j++)
            {
                print(outerProd[i, j]);
            }
        }

    }


}
