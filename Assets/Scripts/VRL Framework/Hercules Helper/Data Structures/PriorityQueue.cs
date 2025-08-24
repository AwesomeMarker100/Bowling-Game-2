using System;
using System.Collections.Generic;
using System.Text;

public class PriorityQueue<T> //angled brackets with T are similar to lists where you specify the type
{

    public struct Item //structs are value-type, not reference type. Encapsulates multiple data fields and respective functions
    {

        //Each item has a priority (that is used to sort it) and a value (the actual thing we're representing) 
        public float priority { get; set; }
        public T value { get; set; }
    }

    List<Item> nodeList = new List<Item>(); //nodes are represented by the Item struct
    private int count = 0;

    public bool Contains(T value) //checks if given value is in the queue
    {
        foreach (Item item in nodeList) //iterate over each item in nodeList and check values
        {
            if (value.Equals(item.value)) return true;
        }

        return false;

    }

    public void Enqueue(float priority, T value) //add item in correct placement based on assigned priority value
    {

        Item item = new Item();

        item.priority = priority;
        item.value = value;

        for(int i = 0; i < nodeList.Count; i++)
        {

            if (item.priority < nodeList[i].priority)
            {
                //put item ahead if it has a higher priority (lower #) 
                nodeList.Insert(i, item);

            }

        }

        count++;
    }

    public int Count()
    {
        return count;
    }

    public Item Peek() //returns item with most priority(lowest number)
    {
        if(count > 0)
        return nodeList[0];

        return default;
    }


    public T Dequeue() //return the highest priority item (lowest #) and remove from queue
    {
        return Peek().value; //returns default of inferred type(type T)
    }



}
