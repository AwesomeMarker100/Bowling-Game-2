using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class MeshBaker : MonoBehaviour
{
    [SerializeField] MeshFilter meshFilter;
    [SerializeField] float maxBakeAngle = 45;
    [SerializeField] Camera camera;

    [SerializeField] Transform pointer;

    [SerializeField] bool baked = false;

    private Dictionary<Vector3, BakeType> bakedVertexMap = new Dictionary<Vector3, BakeType>();

    public enum BakeType
    {

        Unchecked,
        Unbaked,
        Baked

    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Application.isPlaying && baked)
        {

            if(GetComponent<Terrain>() != null) CheckIfPointBaked(Input.mousePosition, GetComponent<Terrain>().terrainData);


        }

        if (Application.isPlaying && !baked && GetComponent<Terrain>() != null)
        {

            BakeTerrain();

        }
    }

    public void OnDrawGizmosSelected()
    {

        Gizmos.color = Color.blue;

        if (meshFilter != null) BakeMesh();
        else if (Application.isPlaying && GetComponent<TerrainMesher>() != null)
        {

            BakeTerrain();
            CheckIfPointBaked(Input.mousePosition, GetComponent<Terrain>().terrainData);

        }

    }



    public void CheckIfPointBaked(Vector3 mousePos, TerrainData terData)
    {

        Ray ray = camera.ScreenPointToRay(mousePos);
        RaycastHit hit;

        Vector3 hitPoint;

        if(GetComponent<TerrainCollider>().Raycast(ray, out hit, 1000))
        {
            hitPoint = hit.point;

            hitPoint.x = (int)((hit.point.x - transform.position.x) / terData.size.x);
            hitPoint.z = (int)((hit.point.z - transform.position.z) / terData.size.z);

            hitPoint.y = terData.GetHeight((int)hitPoint.x, (int)hitPoint.z);



            if (bakedVertexMap.ContainsKey(hitPoint))
            {

                switch (bakedVertexMap[hitPoint])
                {

                    case BakeType.Baked:
                        print("Point is baked!");
                        break;

                    case BakeType.Unbaked:
                        print("Point is elock!");
                        break;

                }

            }

        }
        
    }

    public void BakeMesh()
    {
        if (baked) return;

        baked = true;
        print("Baking mesh...");

        bakedVertexMap.Clear();

        //we need to get surface verts(essentially vertices with their normals facing up) and their triangles(to get nearby vertices)
        Vector3[] surfaceVerts = GetSurfaceVertices();
        int[] surfaceTris = GetSurfaceTriangleIndeces(surfaceVerts);

        InitializeBakedMap(surfaceVerts, true);

        for (int i = 0; surfaceTris.Length > i; i += 3)
        {
            //get all three vertices
            Vector3 vertex1 = transform.TransformPoint(surfaceVerts[surfaceTris[i]]);
            Vector3 vertex2 = transform.TransformPoint(surfaceVerts[surfaceTris[i + 1]]);
            Vector3 vertex3 = transform.TransformPoint(surfaceVerts[surfaceTris[i + 2]]);


            if (BakePoints(vertex1, vertex2)) 
            {

                Gizmos.DrawLine(vertex1, vertex2);

            }

            if (BakePoints(vertex1, vertex3))
            {

                Gizmos.DrawLine(vertex1, vertex3);

            }

            if (BakePoints(vertex2, vertex3))
            {

                Gizmos.DrawLine(vertex2, vertex3);

            }

        }
    }


    public void InitializeBakedMap(Vector3[] points, bool globalize)
    {

        bakedVertexMap.Clear();

        foreach (Vector3 point in points)
        {

            if(!globalize) bakedVertexMap.Add(point, BakeType.Unchecked);
            else bakedVertexMap.Add(transform.TransformPoint(point), BakeType.Unchecked);

        }

    }

    public void BakeTerrain()
    {
        //grab the terrain mesh

        if (baked) return;
        baked = true;

        TerrainMesher mesher = GetComponent<TerrainMesher>();
        Vector3[] localVerts = mesher.GetLocalVertices();

        for(int i = 0; i < localVerts.Length; i++)
        {

            Vector3 vert = localVerts[i];
            Vector3[] neighbors = mesher.GetNeighbors(vert);

            for(int j = 0; neighbors.Length > j; j++)
            {

                BakePoints(vert, neighbors[j]);

            }

        }



    }

    public bool BakePoints(Vector3 point1, Vector3 point2)
    {
        if (MeetsAngleReq(point1, point2))
        {

            SetBakeType(BakeType.Baked, point1, point2);
            return true;
        }
        else
        {
            print("unbaked vertices found");

            if (bakedVertexMap[point1] == BakeType.Unchecked) bakedVertexMap[point1] = BakeType.Unbaked;
            if (bakedVertexMap[point2] == BakeType.Unchecked) bakedVertexMap[point2] = BakeType.Unbaked;

            return false;

        }

    }

    public bool MeetsAngleReq(Vector3 point1, Vector3 point2)
    {

        Vector3 diff = point1 - point2;
        Vector3 baseAngle = new Vector3(diff.x, 0, diff.z);

        return Vector3.Angle(baseAngle, diff) <= maxBakeAngle;

    }

    public void SetBakeType(BakeType bakeType, Vector3 point1, Vector3 point2)
    {

        bakedVertexMap[point1] = bakeType;
        bakedVertexMap[point2] = bakeType;

    }


    

    public Vector3[] GetSurfaceVertices()
    {

        Vector3[] vertices = meshFilter.sharedMesh.vertices;
        Vector3[] normals = meshFilter.sharedMesh.normals;

        List<Vector3> surfaceVertices = new List<Vector3>();

        for(int i = 0; normals.Length > i; i++)
        {
            Vector3 normal = normals[i];

            if (Vector3.Dot(normal, Vector3.up) > 0.7f)
            {

                surfaceVertices.Add(vertices[i]);    

            }

        }

        return surfaceVertices.ToArray();


    }

    public int[] GetSurfaceTriangleIndeces(Vector3[] surfaceVertices)
    {
        Vector3[] fullVerts = meshFilter.sharedMesh.vertices;
        int[] triangleIndeces = meshFilter.sharedMesh.triangles;

        List<int> surfaceTriangleIndeces = new List<int>();

        for(int i = 0; i < triangleIndeces.Length; i += 3)
        {

            Vector3 triangleVertex1 = fullVerts[triangleIndeces[i]];
            Vector3 triangleVertex2 = fullVerts[triangleIndeces[i + 1]];
            Vector3 triangleVertex3 = fullVerts[triangleIndeces[i + 2]];

            int triangle1Match = -1;
            int triangle2Match = -1;
            int triangle3Match = -1;

            for (int j = 0; j < surfaceVertices.Length; j++)
            {

                Vector3 surfaceVertex = surfaceVertices[j];

                if (triangle1Match == -1 && triangleVertex1.Equals(surfaceVertex)) triangle1Match = j;
                if (triangle2Match == -1 && triangleVertex2.Equals(surfaceVertex)) triangle2Match = j;
                if(triangle3Match == -1 && triangleVertex3.Equals(surfaceVertex)) triangle3Match = j;

                if (triangle1Match != -1 && triangle2Match != -1 && triangle3Match != -1) break;

            }

            if(triangle1Match != -1 && triangle2Match != -1 && triangle3Match != -1)
            {

                surfaceTriangleIndeces.Add(triangle1Match);
                surfaceTriangleIndeces.Add(triangle2Match);
                surfaceTriangleIndeces.Add(triangle3Match);

            }


        }

        return surfaceTriangleIndeces.ToArray();

    }

    private void Voxellize()
    {


    }
   
}
