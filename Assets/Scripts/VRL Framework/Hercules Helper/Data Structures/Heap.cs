using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Binary Tree Data Structure
public class Heap
{

    public int rootIdx;

    //tree is just an array, important to know what indices correspond to children from any root node 
    public float[] tree;
    


    //must have same length
    public Heap(float[] arr)
    {

        tree = arr;


    }

    public void MaxHeapify(int start, int max) //original function was different, same concept, but this one was more concise - inspired by GeeksForGeeks
    {

        int largestIdx = start;

        int leftChildIdx = start * 2 + 1; //did figure this part out, glad that GeeksForGeeks copied me :)
        int rightChildIdx = leftChildIdx + 1;


        //left child is the greatest out of this leafset
        if (leftChildIdx < max && tree[leftChildIdx] > tree[largestIdx])
        {

            largestIdx = leftChildIdx;

        }


        //right child is the greatest out of this leafset
        if(rightChildIdx < max && tree[rightChildIdx] > tree[largestIdx])
        {

            largestIdx = rightChildIdx;

        }

        if(start != largestIdx)
        {
            //swap so that the larger number is the parent node and then MaxHeapify() at the index of the node that just got switched out
            Swap(start, largestIdx);
            MaxHeapify(largestIdx, max);

        }

        
    }

    public float[] HeapSort()
    {
        //tried it out myself - got most of the conditions by myself but used GeeksForGeeks for inspiration to get the original max heap

        for(int i = tree.Length / 2 - 1; i >= 0; i--)
        {
            //can't just call it once and let it recurse through because you will have to recheck nodes
            MaxHeapify(i, tree.Length);
        }


        for(int j = tree.Length - 1; j > 0; j--)
        {
            //replace largest with the swap, and then create MaxHeap() again
            Swap(0, j);
            MaxHeapify(0, j);

        }

        return tree;
    }

    //simple array swap function
    public void Swap(int idx1, int idx2)
    {
        float num = tree[idx1];

        tree[idx1] = tree[idx2];
        tree[idx2] = num;
    }
}
