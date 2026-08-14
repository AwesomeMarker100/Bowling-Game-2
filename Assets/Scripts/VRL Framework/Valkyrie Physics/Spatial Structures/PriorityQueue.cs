using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class Data
{
    public int value;
    public int priority;
    public Data from;
    public Data(int value, int priority, Data from)
    {
        this.value = value;
        this.priority = priority;
        this.from = from;

    }
}
public class PriorityQueue
{
    private LinkedList<Data> queue;

    public PriorityQueue()
    {
        queue = new LinkedList<Data>();
    }

    public bool Enqueue(int s, int priority, Data from)
    {
        if (queue.Count == 0 || priority > queue.First.Value.priority) { queue.AddFirst(new Data(s, priority, from)); return true; }
        if(priority < queue.Last.Value.priority) { queue.AddLast(new Data(s, priority, from)); return true; }

        LinkedListNode<Data> prev = null;
        LinkedListNode<Data> cur = queue.First;
        
        while(cur != null)
        {
            if(cur.Value.priority < priority)
            {
                queue.AddAfter(cur, new Data(s, priority, from));
                return true;
            }

            cur = cur.Next;
        }
        
        return false;
    }

    public Data Dequeue()
    {
        return queue.Last.Value;
    }

    


}
