using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Binary Tree Data Structure
public class Heap
{



    public static void MaxHeapify(float[] arr, int start, int max) //original function was different, same concept, but this one was more concise - inspired by GeeksForGeeks
    {

        int largestIdx = start;

        int leftChildIdx = start * 2 + 1; //did figure this part out, glad that GeeksForGeeks copied me :)
        int rightChildIdx = leftChildIdx + 1;


        //left child is the greatest out of this leafset
        if (leftChildIdx < max && arr[leftChildIdx] > arr[largestIdx])
        {

            largestIdx = leftChildIdx;

        }


        //right child is the greatest out of this leafset
        if (rightChildIdx < max && arr[rightChildIdx] > arr[largestIdx])
        {

            largestIdx = rightChildIdx;

        }

        if (start != largestIdx)
        {
            //swap so that the larger number is the parent node and then MaxHeapify() at the index of the node that just got switched out
            Swap(arr, start, largestIdx);
            MaxHeapify(arr, largestIdx, max);

        }


    }

    public static float[] Sort(float[] arr)
    {
        //tried it out myself - got most of the conditions by myself but used GeeksForGeeks for inspiration to get the original max heap

        for (int i = arr.Length / 2 - 1; i >= 0; i--)
        {
            //can't just call it once and let it recurse through because you will have to recheck nodes
            MaxHeapify(arr, i, arr.Length);
        }


        for (int j = arr.Length - 1; j > 0; j--)
        {
            //replace largest with the swap, and then create MaxHeap() again
            Swap(arr, 0, j);
            MaxHeapify(arr, 0, j);

        }

        return arr;
    }

    //simple array swap function
    public static void Swap(float[] arr, int idx1, int idx2)
    {
        float num = arr[idx1];

        arr[idx1] = arr[idx2];
        arr[idx2] = num;
    }
}
