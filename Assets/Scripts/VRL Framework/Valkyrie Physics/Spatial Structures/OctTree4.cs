using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Runtime.ConstrainedExecution;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using static UnityEditor.PlayerSettings;


public enum Octant
{
    BTR, //back top right
    FTR, //front top right
    BBR, //back bottom right
    FBR, //front bottom right
    BTL, //back top left
    FTL, //front top left
    BBL, //back bottom left
    FBL, //front bottom left
    OOB //out of bounds
}

public struct BoxBounds
{
    //add InBox() method here
    public Vector3 topRight;
    public Vector3 bottomLeft;

    public BoxBounds(Vector3 topRight, Vector3 bottomLeft)
    {
        if (topRight.x <= bottomLeft.x || topRight.y <= bottomLeft.y || topRight.z <= bottomLeft.z) throw new ArgumentException("Your top right corner and bottom left corner are invalid, check that you have not switched the two!");
        this.topRight = topRight;
        this.bottomLeft = bottomLeft;
    }
    public bool InBounds(Vector3 pos)
    {
        return pos.x <= topRight.x && pos.x >= bottomLeft.x && pos.y <= topRight.y && pos.y >= bottomLeft.y && pos.z <= topRight.z && pos.z >= bottomLeft.z;
    }
}
public struct NodeData
{
    //data can refer to an index in a list of gameobjects or something else
    public short data;
    public Vector3 pos;
    public NodeData(int data, Vector3 pos)
    {
        this.data = (short)data;
        this.pos = pos;
    }
}

//center of mass calculation to determine what triangles are most in need of dots-  could simplify octtree children replacement
public class OctNode
{
    public int childStart;
    public List<NodeData> nodeData { get; }
    public BoxBounds bounds { get; }
    public OctNode(BoxBounds bounds)
    {
        this.bounds = bounds;
        this.nodeData = new List<NodeData>();
        this.childStart = -1;
    }
    public bool InBounds(Vector3 pos)
    {
        return bounds.InBounds(pos);
    }

    public bool InsertNodeData(NodeData data)
    {
        if (!InBounds(data.pos)) return false;
        nodeData.Add(data);
        return true;
    }

    public bool RemoveNodeData(NodeData data)
    {
        if (!nodeData.Contains(data)) return false;
        nodeData.Remove(data);
        return true;
    }

    public Octant GetOctant(Vector3 pos)
    {
        if (!InBounds(pos)) return Octant.OOB;
        Vector3 center = (bounds.topRight + bounds.bottomLeft) / 2;

        if (pos.x > center.x)
        {
            if (pos.y > center.y)
            {
                if (pos.z > center.z)
                {
                    //BTR
                    return Octant.BTR;
                }
                else
                {
                    return Octant.FTR;
                }
            }
            else
            {
                if (pos.z > center.z)
                {
                    return Octant.BBR;
                }
                else
                {
                    return Octant.FBR;
                }
            }
        }
        else
        {
            if (pos.y > center.y)
            {
                if (pos.z > center.z)
                {

                    return Octant.BTL;
                }
                else
                {
                    return Octant.FTL;
                }
            }
            else
            {
                if (pos.z > center.z)
                {
                    return Octant.BBL;
                }
                else
                {
                    return Octant.FBL;
                }
            }
        }


    }

    public void SetChildStart(int childStart)
    {
        this.childStart = childStart;
    }

}

public class OctTree4
{ 
    public List<OctNode> nodes { get; } = new List<OctNode>();
    private int maxDataPerNode = 5;


    //bounds defines the largest box we're drawing
    public OctTree4(BoxBounds bounds, int maxDataPerNode)
    {
        if (maxDataPerNode < 0) maxDataPerNode = 1;

        nodes.Add(new OctNode(bounds));
        this.maxDataPerNode = maxDataPerNode;
    }

    public bool InsertData(int[] data, Vector3[] positions)
    {
        if (data.Length != positions.Length) throw new ArgumentException("Data and position arrays given are not the same length!");
      

        for(int i = 0; data.Length > i; i++)
        {
            if (!InsertData(data[i], positions[i])) return false;
        }

        return true;
    }

    public bool InsertData(int data, Vector3 pos)
    {
        NodeData nodeData = new NodeData(data, pos);
        OctNode node = GetNode(pos);

        //check this order later
        if (node == null) return false;
        
        node.InsertNodeData(nodeData);
        if (node.nodeData.Count + 1 > maxDataPerNode) OctSplit(node);
      
        return true;
    }

    private void OctSplit(OctNode node)
    {
        node.SetChildStart(nodes.Count);

        Vector3 topRight = node.bounds.topRight;
        Vector3 bottomLeft = node.bounds.bottomLeft;
        Vector3 center = (topRight + bottomLeft) / 2;

        Vector3 width = new Vector3(topRight.x - bottomLeft.x, 0, 0);
        Vector3 height = new Vector3(0, topRight.y - bottomLeft.y, 0);
        Vector3 depth = new Vector3(0, 0, topRight.z - bottomLeft.z);

        nodes.Add(new OctNode(new BoxBounds(topRight, center))); //BTR
        nodes.Add(new OctNode(new BoxBounds(topRight - depth / 2, center - depth / 2))); //FTR
        nodes.Add(new OctNode(new BoxBounds(topRight - height / 2, center - height / 2))); //BBR
        nodes.Add(new OctNode(new BoxBounds(topRight - (depth + height) / 2, center - (depth + height) / 2))); //FBR
        nodes.Add(new OctNode(new BoxBounds(topRight - width / 2, bottomLeft + (height + depth) / 2))); //BTL
        nodes.Add(new OctNode(new BoxBounds(center + height / 2, bottomLeft + height / 2))); //FTL
        nodes.Add(new OctNode(new BoxBounds(center + depth / 2, bottomLeft + depth / 2))); //BBL
        nodes.Add(new OctNode(new BoxBounds(center, bottomLeft))); //FBL

        RealignChildren(node);
    }

    private OctNode GetNode(Vector3 pos)
    {
        int j = 0;
        OctNode cur = null;

        //while this valid node has children 
        while (j < nodes.Count && (cur = nodes[j]).childStart != -1)
        {
            Octant octant = cur.GetOctant(pos);
            switch (octant)
            {
                case Octant.BTR:
                    j = cur.childStart;
                    break;
                case Octant.FTR:
                    j = cur.childStart + 1;
                    break;
                case Octant.BBR:
                    j = cur.childStart + 2;
                    break;
                case Octant.FBR:
                    j = cur.childStart + 3;
                    break;
                case Octant.BTL:
                    j = cur.childStart + 4;
                    break;
                case Octant.FTL:
                    j = cur.childStart + 5;
                    break;
                case Octant.BBL:
                    j = cur.childStart + 6;
                    break;
                case Octant.FBL:
                    j = cur.childStart + 7;
                    break;
                case Octant.OOB:
                    throw new ArgumentOutOfRangeException("Given a position out of box bounds!");
            }
        }

        return nodes[j];
    }

    private void RealignChildren(OctNode node)
    {

        //take each nodeData point in this node and place them in the appropriate children nodes
        foreach(NodeData nodeData in node.nodeData)
        {
            Octant octant = node.GetOctant(nodeData.pos);
            
            switch (octant)
            {
                case Octant.BTR:
                    nodes[node.childStart].InsertNodeData(nodeData);
                    break;
                case Octant.FTR:
                    nodes[node.childStart + 1].InsertNodeData(nodeData);
                    break;
                case Octant.BBR:
                    nodes[node.childStart + 2].InsertNodeData(nodeData);
                    break;
                case Octant.FBR:
                    nodes[node.childStart + 3].InsertNodeData(nodeData);
                    break;
                case Octant.BTL:
                    nodes[node.childStart + 4].InsertNodeData(nodeData);
                    break;
                case Octant.FTL:
                    nodes[node.childStart + 5].InsertNodeData(nodeData);
                    break;
                case Octant.BBL:
                    nodes[node.childStart + 6].InsertNodeData(nodeData);
                    break;
                case Octant.FBL:
                    nodes[node.childStart + 7].InsertNodeData(nodeData);
                    break;
            }
        }

        node.nodeData.Clear();
    }

    public List<NodeData> GetData(Vector3 point)
    {
        return GetNode(point).nodeData;
    }

}