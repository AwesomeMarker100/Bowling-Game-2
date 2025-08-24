using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//THIS REGION IS JUST A NODE
public class KDRegion<T>
{

    public enum Dimension
    {
        x, y, z
    }

    public bool onLeft;
    public bool isHead = false;

    public int height;

    private List<T> members;
    private KDRegion<T> parent;
    private KDRegion<T> sibling;

    private KDRegion<T> leftChild;
    private KDRegion<T> rightChild;

    private int level;
    private float splitValue;

    public int randomNumber;
    
    public KDRegion()
    {
        members = new List<T>();
        
    }
    //GETTERS AND SETTERS
    #region
    public KDRegion<T> GetParent()
    {

        return parent;

    }

    public void SetParent(KDRegion<T> parent)
    {
        
        this.parent = parent;

    }

    public KDRegion<T> GetLeftChild()
    {
        return leftChild;
    }

    public KDRegion<T> GetRightChild()
    {

        return rightChild;

    }

    public int GetLevel()
    {

        return level;

    }

    public float GetSplitValue()
    {

        return splitValue;

    }

    public void SetSplitValue(float val)
    {

        splitValue = val;
    }

    public void SetLevel(int level)
    {

        this.level = level;

    }


    public void SetLeftChild(KDRegion<T> child)
    {
        this.leftChild = child;

    }

    public void SetSibling(KDRegion<T> sibling)
    {

        this.sibling = sibling;

    }

    public KDRegion<T> GetSibling()
    {

        return sibling;

    }

    public void SetRightChild(KDRegion<T> child)
    {

        this.rightChild = child;

    }

    public List<T> GetMembers()
    {

        return members;

    }


    public void SetMembers(List<T> members)
    {
        this.members = members;
    }

    public void SetMembers(T[] members)
    {

        this.members = new List<T>(members);

    }

    #endregion

    //MEMBER FUNCTIONS
    #region
    public void AddMember(T member)
    {

        members.Add(member);

    }

    public void AddMembers(List<T> newMembers)
    {

        foreach(T member in newMembers)
        {

            if (!members.Contains(member))
            {

                members.Add(member);

            }

        }

    }

    public void RemoveMember(T member)
    {

        members.Remove(member);

    }

    public void RemoveAllMembers()
    {

        this.members.Clear();
    }

    #endregion


}
