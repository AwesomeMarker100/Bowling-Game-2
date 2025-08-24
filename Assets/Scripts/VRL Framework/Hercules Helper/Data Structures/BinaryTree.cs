using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BinaryTree<T>
{
    public T root;

    //where 0 is left and 1 is right
    public Dictionary<T, T[]> tree;


    public BinaryTree()
    {
         


    }

    //AUTOMATICALLY CREATES BINARY TREE
    public BinaryTree(T[] values)
    {
        //tree 
        if (values.Length < 3) return;

        tree = new Dictionary<T, T[]>();

        for(int i = 0; i < values.Length; i += 3)
        {

            tree.Add(values[i], new T[] {values[i + 1], values[i + 2]});

        }
    }

    public void Swap(T node, T otherNode)
    {
        T[] copy = tree[node];

        tree[node] = tree[otherNode];
        tree[otherNode] = copy;
    }


    public T[] GetChildren(T node)
    {

        return tree[node];

    }

    public void SetLeftChild(T root, T left)
    {
        tree[root][0] = left;
    }

    public void SetRightChild(T root, T right)
    {

        tree[root][1] = right;

    }

}
