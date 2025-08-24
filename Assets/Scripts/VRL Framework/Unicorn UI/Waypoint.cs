using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    [HideInInspector] public Vector3 position;
    public float cost; //heuristic
    private List<Waypoint> neighbors;

    public Waypoint parent;


    public void Awake()
    {
        this.position = transform.position;
    }

    public void AddNeighbor(Waypoint neighbor)
    {

        neighbors.Add(neighbor);

    }

    public List<Waypoint> GetNeighbors()
    {

        return neighbors;

    }

    public void SetNeighbors(List<Waypoint> neighbors)
    {

        this.neighbors = neighbors;

    }

    

}
