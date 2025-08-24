using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Sorter
{
    

    //property name must refer to a numeric value!
    public static void QuickSort(Object[] arr, string propertyName, int start, int end)
    {

        //since it's recursive, check that the array length that we're sorting is 2 or more
        if (end - start < 1) return;

        //choose a pivot point, i chose the middle
        int pivot = (end + start) / 2;
        int j = end;

        //i is your left pointer, j is the right pointer
        for (int i = start; i <= j;)
        {
            //if your right pointer is less than the pivot or your left pointer is greater than the pivot, swap
            if (GetFloatVal(arr[j], propertyName) < GetFloatVal(arr[pivot], propertyName) || GetFloatVal(arr[i], propertyName) > GetFloatVal(arr[pivot], propertyName))
            {

                Object temp = arr[j];

                arr[j] = arr[i];
                arr[i] = temp;

            }
            else
            {

                j--;
                i++;

            }
        }

        //call quick sort for the left and right arrays
        QuickSort(arr, propertyName, start, pivot);
        QuickSort(arr, propertyName, pivot + 1, end);

    }


    public static float[] HeapSort(float[] values)
    {

        Heap heap = new Heap(values);

        return heap.HeapSort();
       

    }


    public static float GetFloatVal(Object obj, string propertyName)
    {

        return (float)obj.GetType().GetProperty(propertyName).GetValue(obj);

    }






}
