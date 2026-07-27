using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteAlways]
public class ValkyrieMeshCollider : ValkyrieCollider
{
    [SerializeField] MeshFilter meshFilter;

    [SerializeField] int combineFactor = 1;

    private Mesh mesh;
    private Vector3[] globalizedNormals;

    private Vector3[] triangleAverages;
    private KDTree<Vector3> colliderTree;

    private bool gizmosRefreshed = false;

    private void Awake()
    {
        this.mesh = meshFilter.sharedMesh;
        this.type = ColliderType.MeshCollider;

        GlobalizeVertices();

        SetTriangleAverages();
        /*colliderTree = new KDTree<Vector3>(globalizedNormals, globalizedNormals);
        colliderTree.Build(10000);*/

    }

    private void GlobalizeVertices()
    {

        Vector3[] globalVerts = new Vector3[mesh.normals.Length];

        for(int i = 0; globalVerts.Length > i; i++)
        {

            globalVerts[i] = transform.TransformPoint(mesh.normals[i]);

        }

        globalizedNormals = globalVerts;

    }

    private void SetTriangleAverages()
    {
        
        int[] theTris = mesh.triangles;
        Vector3[] vertices = mesh.vertices;

        triangleAverages = new Vector3[theTris.Length / 3];
        int j = 0;


        for (int i = 0; i < theTris.Length; i += 3)
        {

            int i1 = theTris[i];
            int i2 = theTris[i + 1];
            int i3 = theTris[i + 2];

            triangleAverages[j] = (vertices[i1] + vertices[i2] + vertices[i3])  / 3;
            j++;
        }

    }

    public override void DrawGizmos()
    {
        base.DrawGizmos();

        Gizmos.matrix = transform.localToWorldMatrix;
        this.mesh = meshFilter.sharedMesh;

        int[] theTris = mesh.triangles;
        Vector3[] vertices = mesh.vertices;

        triangleAverages = new Vector3[theTris.Length / 3];

        for (int i = 0; i < theTris.Length; i += 3)
        {

            int i1 = theTris[i];
            int i2 = theTris[i + 1];
            int i3 = theTris[i + 2];


            Gizmos.DrawLine(vertices[i1], vertices[i2]);
            Gizmos.DrawLine(vertices[i1], vertices[i3]);
            Gizmos.DrawLine(vertices[i2], vertices[i3]);
            
        }


    }



    public override Vector3 GetClosestPointOnBounds(Vector3 point)
    {
        /*  Vector3 closestPoint = triangleAverages[0];

          foreach (Vector3 triangleAverage in triangleAverages)
          {

              Vector3 trueTriangleAvg = transform.TransformPoint(triangleAverage);
              if (Vector3.Distance(trueTriangleAvg, point) < Vector3.Distance(closestPoint, point)) closestPoint = trueTriangleAvg;


          }

          return closestPoint;*/

        return Vector3.zero;
    }



}
