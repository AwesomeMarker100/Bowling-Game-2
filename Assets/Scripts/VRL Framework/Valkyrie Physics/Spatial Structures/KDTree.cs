using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;


public class KDNode
{
    public List<NodeData> nodeData { get; }

    public KDNode(List<NodeData> nodeData)
    {
        this.nodeData = nodeData;
    }

    public bool InsertNodeData(NodeData nodeData)
    {
        if (this.nodeData.Contains(nodeData)) return false;
        this.nodeData.Add(nodeData);
        return true;
    }


    public bool RemoveNodeData(NodeData nodeData)
    {

        return this.nodeData.Remove(nodeData);

    }
} 

public class KDTree
{
    public List<KDNode> leafNodes { get; } = new List<KDNode>();
    public int maxDataPerNode;
    private List<float> splitValues;

    public KDTree(int maxDataPerNode)
    {
        this.maxDataPerNode = maxDataPerNode;
    }

    public void Build(int[] data, Vector3[] positions)
    {
        if(data.Length != positions.Length) throw new ArgumentException("Data and position array lengths are not the same!");

        NodeData[] nodeData = new NodeData[data.Length];
        
        for(int i = 0; i < data.Length; i++)
        {
            nodeData[i] = new NodeData(data[i], positions[i]);
        }

        CreateNode(0, nodeData);

    }

    public void CreateNode(int level, NodeData[] nodeData)
    {
        if(nodeData.Length > maxDataPerNode)
        {
            //create children
            float median = GetMedian(level, nodeData);
            splitValues.Add(median);
            NodeData[] leftHalf = new NodeData[nodeData.Length / 2];
            NodeData[] rightHalf = new NodeData[(nodeData.Length / 2) + 1];
            int i = 0;
            int j = 0;

            switch (level)
            {
                case 0:
                    
                    while(i + j < nodeData.Length)
                    {
                        if (nodeData[i + j].pos.x < median)
                        {
                            leftHalf[i] = nodeData[i + j];
                            i++;

                        } else
                        {
                            rightHalf[j] = nodeData[i + j];
                            j++;
                        } 
                    }

                    break;
                case 1:

                    while (i + j < nodeData.Length)
                    {
                        if (nodeData[i + j].pos.y < median)
                        {
                            leftHalf[i] = nodeData[i + j];
                            i++;

                        }
                        else
                        {
                            rightHalf[j] = nodeData[i + j];
                            j++;
                        }
                    }

                    break;
                case 2:

                    while (i + j < nodeData.Length)
                    {
                        if (nodeData[i + j].pos.z < median)
                        {
                            leftHalf[i] = nodeData[i + j];
                            i++;
                        }
                        else
                        {
                            rightHalf[j] = nodeData[i + j];
                            j++;
                        }
                    }
          
                    break;
            }

            CreateNode((level + 1) % 3, leftHalf);
            CreateNode((level + 1) % 3, rightHalf);

        } else
        {
            List<NodeData> data = new List<NodeData>();
            data.AddRange(nodeData);
            leafNodes.Add(new KDNode(data));
        }
    }

    public KDNode GetNode(Vector3 pos)
    {
        int nodeIdx = 0;

        int numRightShifts = 0;

        for(int i = 0; i < splitValues.Count; i++)
        {
            switch(i % 3)
            {
                case 0:
                    if(pos.x < splitValues[i]) nodeIdx *= 2;
                    else nodeIdx += (int)Mathf.Pow(2, numRightShifts++); 
                    break;
                case 1:
                    if (pos.y < splitValues[i]) nodeIdx *= 2;
                    else nodeIdx += (int)Mathf.Pow(2, numRightShifts++);
                    break;

                case 2:

                    if (pos.z < splitValues[i]) nodeIdx *= 2;
                    else nodeIdx += (int)Mathf.Pow(2, numRightShifts++);
                    break;
            }
        }

        return leafNodes[nodeIdx];
    }

    public float GetMedian(int level, NodeData[] nodeData)
    {
        float[] values = { Mathf.Infinity };

        switch(level)
        {
            case 0:
                values = nodeData.Select(nod => nod.pos.x).ToArray();
                break;

            case 1:
                values = nodeData.Select(nod => nod.pos.y).ToArray();
                break;
            case 2:
                values = nodeData.Select(nod => nod.pos.z).ToArray();
                break;

        }

        return Heap.Sort(values)[values.Length / 2];
    }

}
