using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KDTree<T>
{


    public int maxItemsInRegion = 3;

    public T[] globalMembers;

    private Dictionary<T, Vector3> valuePosPairs = new Dictionary<T, Vector3>();
    private Dictionary<T, KDRegion<T>> valueRegionPairs = new Dictionary<T, KDRegion<T>>();


    private float[] splitValues;
    private int regionCount = 0;

    private IList<Vector3> positions;


    public List<KDRegion<T>> regions = new List<KDRegion<T>>();


    public KDTree(T[] globalMembers, IList<Vector3> positions)
    {
        //set positions and members
        this.positions = positions;
        this.globalMembers = globalMembers;
        splitValues = new float[globalMembers.Length];

        for(int i = 0; i < globalMembers.Length; i++)
        {
            //add value pos and value region pairs to their dictionaries
            valuePosPairs.Add(globalMembers[i], positions[i]);
            valueRegionPairs.Add(globalMembers[i], null);

        }
    }

    public KDTree(T[] globalMembers, IList<Vector3> positions, float[] splitValues)
    {
        //set positions and members
        this.positions = positions;
        this.globalMembers = globalMembers;
        this.splitValues = splitValues;

        for (int i = 0; i < globalMembers.Length; i++)
        {
            //add value pos and value region pairs to their dictionaries
            valuePosPairs.Add(globalMembers[i], positions[i]);
            valueRegionPairs.Add(globalMembers[i], null);

        }
    }

    /*
     * 
     * 
     */

    public void Build(int k)
    {
        maxItemsInRegion = k;

        KDRegion<T> initNode = new KDRegion<T>();
        initNode.SetMembers(globalMembers);

        regions.Add(initNode);
        CreateRegion(k, 0, initNode);
    }


    public T[] GetAllMembers()
    {
        return globalMembers;
    }

    public IList<Vector3> GetAllPositions()
    {

        return positions;

    }


    //where k is the maximum amount of members in a region, the level dictates what axis we're splitting on(x, y, or z) and the lastRegion
    public void CreateRegion(int k, int level, KDRegion<T> lastRegion)
    {
        regionCount++;

        lastRegion.randomNumber = Random.Range(0, 1000);
        if (lastRegion.GetMembers().Count <= k) return;

        List<T> members = lastRegion.GetMembers();

        //for this lastRegion, create a leftRegion and rightRegion KDRegion

        KDRegion<T> leftRegion = new KDRegion<T>();
        KDRegion<T> rightRegion = new KDRegion<T>();

        SetupRegions(level, lastRegion, leftRegion, rightRegion);

        //get a split value based on the level and split members into left and right
        float splitVal = GetMedian(level, members);

        lastRegion.SetSplitValue(splitVal);

        SplitMembers(members, splitVal, level, leftRegion, rightRegion);

        CreateRegion(k, level + 1, leftRegion);
        CreateRegion(k, level + 1, rightRegion);

    }

    private void SplitMembers(List<T> members, float splitValue, int level, KDRegion<T> leftRegion, KDRegion<T> rightRegion)
    {
        foreach(T member in members) //within last region
        {

            //number on whatever axis we're on is greater than median value, rightRegion
            if(OnPosSide(level, valuePosPairs[member], splitValue))
            {
                rightRegion.AddMember(member);
                valueRegionPairs[member] = rightRegion;

            } else //less than median value, leftRegion
            {

                leftRegion.AddMember(member);
                valueRegionPairs[member] = leftRegion;

            }

        }

    }

    public bool OnPosSide(int level, Vector3 pos, float splitVal)
    {

        switch (level % 3)
        {

            case 0:

                return pos.x >= splitVal; //first we split on x

            case 1:

                return pos.y >= splitVal; //then y 

            default:

                return pos.z >= splitVal; //then z 

        }

    }


    private void SetupRegions(int level, KDRegion<T> parent, KDRegion<T> leftRegion, KDRegion<T> rightRegion)
    {
        //set the left and rightRegion nodes connected to the parent
        parent.SetLeftChild(leftRegion);
        parent.SetRightChild(rightRegion);
        parent.SetLevel(level);

        leftRegion.SetParent(parent);
        rightRegion.SetParent(parent);

        regions.Add(leftRegion);
        regions.Add(rightRegion);

    }


    public KDRegion<T> GetRegion(Vector3 pos)
    {

        KDRegion<T> lastRegion = regions[0];

        while(lastRegion.GetLeftChild() != null) //in the KDTree, we always have two children so just check if the left child exists
        {
            int level = lastRegion.GetLevel();

            if(OnPosSide(level, pos, lastRegion.GetSplitValue()))
            {

                lastRegion = lastRegion.GetRightChild();

            } else
            {

                lastRegion = lastRegion.GetLeftChild();

            }

        }

        return lastRegion;

    }


    public float GetMedian(int level, List<T> members)
    {

        float[] initArr = new float[members.Count];

        //splitting on the x-axis
        if (level % 3 == 0)
        {
            //make an array of all the members' x
            for (int i = 0; i < members.Count; i++)
            {
                Vector3 memberPos = valuePosPairs[members[i]];
                initArr[i] = memberPos.x;

            }
            //use heapsort to sort x-values and return the median value to split em by
            return Sorter.HeapSort(initArr)[members.Count / 2];
        
            //splitting on that y axis
        } else if (level % 3 == 1)
        {

            for (int i = 0; i < members.Count; i++)
            {
                Vector3 memberPos = valuePosPairs[members[i]];
                initArr[i] = memberPos.y;


            }

            

            return Sorter.HeapSort(initArr)[members.Count / 2];


        } else //splitting on that z-axis
        {

            for (int i = 0; i < members.Count; i++)
            {
                Vector3 memberPos = valuePosPairs[members[i]];

                initArr[i] = memberPos.z;

            }

            return Sorter.HeapSort(initArr)[members.Count / 2];

        }

    }



    public List<KDRegion<T>> GetAllRegions()
    {

        return regions;

    }

    public KDRegion<T>[] GetEndRegions()
    {

        List<KDRegion<T>> endRegions = new List<KDRegion<T>>();

        for(int i = 0; regions.Count > i; i++)
        {

            if (regions[i].GetLeftChild() == null)
            {
                endRegions.Add(regions[i]);

            }


        }

        return endRegions.ToArray();


    }


}
