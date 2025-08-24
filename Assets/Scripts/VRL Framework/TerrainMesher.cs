using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainMesher : MonoBehaviour
{

    [Tooltip("Higher increases precision but also load")] public int regionCount = 80;

    private Vector3[] localVerts;
    private Vector3[] globalVerts;
    

    private KDTree<Vector3> terrainMesh;

    private Terrain terrain;
    private TerrainData terrainData;

    // Start is called before the first frame update
    void Awake()
    {
        terrain = GetComponent<Terrain>();
        terrainData = terrain.terrainData;

        SetTerrainVerts();
        CreateTerrainMesh();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetTerrainVerts()
    {
        TerrainData terrainData = GetComponent<Terrain>().terrainData;


        localVerts = new Vector3[(int)terrainData.size.x * (int)terrainData.size.z];
        int i = 0;

        print("Terrain X Size: " + terrainData.size.x);
        print("Terrain Z Size: " + terrainData.size.z);

        for (int x = 0; terrainData.size.x > x; x++)
        {

            for (int z = 0; terrainData.size.z > z; z++)
            {

                float y = terrainData.GetHeight(x, z);
                localVerts[i] = new Vector3(x, y, z);

                i++;
            }

        }

        

    }

    private void CreateTerrainMesh()
    {
        terrainMesh = new KDTree<Vector3>(localVerts, localVerts);
        terrainMesh.Build(20);

        print(terrainMesh.GetEndRegions().Length);

    }

    public Vector3[] GetNeighbors(Vector3 point)
    {
        List<Vector3> neighbors = new List<Vector3>();

        if (point.x + 1 < terrainData.size.x)
        {
            neighbors.Add(new Vector3(point.x + 1, terrainData.GetHeight((int)point.x + 1, (int)point.z), point.z)); // RIGHT

            if(point.z + 1 < terrainData.size.z)
            {

                neighbors.Add(new Vector3(point.x, terrainData.GetHeight((int)point.x, (int)point.z + 1), point.z + 1)); //TOP
                neighbors.Add(new Vector3(point.x + 1, terrainData.GetHeight((int)point.x + 1, (int)point.z + 1), point.z + 1)); //TOP RIGHT

            }

            if(point.z - 1 >= 0)
            {

                neighbors.Add(new Vector3(point.x, terrainData.GetHeight((int)point.x, (int)point.z - 1), point.z - 1)); //BOTTOM 
                neighbors.Add(new Vector3(point.x + 1, terrainData.GetHeight((int)point.x + 1, (int)point.z - 1), point.z - 1)); //BOTTOM RIGHT


            }

        }

        if (point.x - 1 >= 0)
        {

            neighbors.Add(new Vector3(point.x - 1, terrainData.GetHeight((int)point.x - 1, (int)point.z), point.z)); //LEFT

            if (point.z + 1 < terrainData.size.z)
            {
                neighbors.Add(new Vector3(point.x - 1, terrainData.GetHeight((int)point.x - 1, (int)point.z + 1))); //TOP LEFT

            }

            if(point.z - 1 >= 0)
            {

                neighbors.Add(new Vector3(point.x - 1, terrainData.GetHeight((int)point.x - 1, (int)point.z - 1), point.z -1)); //BOTTOM LEFT


            }

        }

        return neighbors.ToArray();

    }


    public KDTree<Vector3> GetTerrainMesh() {


        return terrainMesh;

    }

    public Vector3[] GetLocalVertices()
    {

        return localVerts;

    }

    public Vector3[] GetGlobalVertices()
    {

        return globalVerts;

    }


    
}
