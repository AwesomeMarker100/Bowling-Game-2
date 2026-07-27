using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.Intrinsics;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.LowLevelPhysics;

public static class GJK
{

    //represents the shape we can make in any n-dimension with least amount of vertices - e.g. 1-simplex is a line (one dimensional and can contain a point), 2-simplex is a triangle and can contain a line, etc

    public static float supportThreshold = 0.00001f;

    //Structs
    #region

    public struct SimplexPt
    {
        //pt = support point, dir = direction
        public Vector3 pt;
        public Vector3 dir;

        public SimplexPt(Vector3 pt, Vector3 dir) => (this.pt, this.dir) = (pt, dir);

        public override bool Equals(object obj) =>
            obj is SimplexPt b && MinoMath.VApproximately(pt, b.pt, 0.005f) && MinoMath.VApproximately(dir, b.dir, 0.005f);

        public static bool operator ==(SimplexPt a, SimplexPt b) => a.Equals(b);

        public static bool operator !=(SimplexPt a, SimplexPt b) => !a.Equals(b);


        public override string ToString()
        {
            return $"Dir: {dir}, Pt: {pt}";
        }
        // Quantize components to make hash compatible with approximate Equals (tolerance ~0.005) AI did this one
        public override int GetHashCode()
        {
            const float quant = 1000f; // 0.001 resolution, smaller than Equals tolerance
            int px = Mathf.RoundToInt(pt.x * quant);
            int py = Mathf.RoundToInt(pt.y * quant);
            int pz = Mathf.RoundToInt(pt.z * quant);
            int dx = Mathf.RoundToInt(dir.x * quant);
            int dy = Mathf.RoundToInt(dir.y * quant);
            int dz = Mathf.RoundToInt(dir.z * quant);

            unchecked
            {
                int hash = 17;
                hash = hash * 31 + px;
                hash = hash * 31 + py;
                hash = hash * 31 + pz;
                hash = hash * 31 + dx;
                hash = hash * 31 + dy;
                hash = hash * 31 + dz;
                return hash;
            }
        }
    }
    public struct Simplex
    {
        //Parameters
        #region
        public List<SimplexPt> points;
        public List<SimplexPt> directions;
        #endregion
        //Add/Remove Points
        #region
        public void AddPoint(SimplexPt pt)
        {
            if (points.Contains(pt)) { Debug.LogException(new Exception("Failed to add point as it is already contained in simplex!")); return; }
        }

        public void RemovePoint(SimplexPt pt)
        {
            if (!points.Contains(pt)) { Debug.LogException(new Exception("Failed to remove point as it is not contained in simplex!")); return; }
        }
        #endregion

    }

    public struct Polytope
    {
        //Parameters
        #region
        private Simplex initialSimplex;
        private List<PolytopeTri> triangles;
        #endregion

        //confirmed
        public Polytope(Simplex initialSimplex)
        {
            //add initial simplex, and create new triangles list
            this.initialSimplex = initialSimplex;
            triangles = new List<PolytopeTri>();

            SetInitTriangles();
        }


        //Managing Triangles List
        #region

        //confirmed
        private void SetInitTriangles()
        {

            SimplexPt p0 = initialSimplex.points[0];
            SimplexPt p1 = initialSimplex.points[1];
            SimplexPt p2 = initialSimplex.points[2];
            SimplexPt p3 = initialSimplex.points[3];

            try
            {
                PolytopeTri tri1 = new PolytopeTri(p0, p1, p2);
                PolytopeTri tri2 = new PolytopeTri(p0, p1, p3);
                PolytopeTri tri3 = new PolytopeTri(p0, p2, p3);
                PolytopeTri tri4 = new PolytopeTri(p1, p2, p3);


                AddTriangles(new PolytopeTri[]{ tri1,tri2,tri3,tri4});

            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }
        }

        //confirmed 
        public void AddTriangle(PolytopeTri tri)
        {
            if (triangles.Contains(tri)) { Debug.LogException(new Exception("Triangle already added to polytope!")); return; }
            triangles.Add(tri);
        }

        //confirmed 
        public void AddTriangles(PolytopeTri[] triList)
        {
            foreach (PolytopeTri tri in triList)
            {
                AddTriangle(tri);
            }
        }

        //confirmed

        public void RemoveTriangle(PolytopeTri tri)
        {
            if (!triangles.Contains(tri)) { Debug.LogException(new Exception("Triangle is not in this polytope!")); return; }
            triangles.Remove(tri);
        }
        #endregion

        //Adding a New Point
        #region

        //confirmed
        private bool FacesSameDirection(PolytopeTri tri, Vector3 point)
        {

            SimplexPt v1 = tri.GetVertex(0);
            Vector3 norm = tri.normal;

            float dotProd = Vector3.Dot(point - v1.pt, norm);

            return dotProd > 0;
        }

        //confirmed
        private void CollectFaultyEdges(List<UndirectedEdge> faultyEdges, SimplexPt point)
        {
            for (int i = 0; i < triangles.Count; i++)
            {
                PolytopeTri tri = triangles[i];

                //check if this triangle normal points the same direction as the support point
                if (FacesSameDirection(tri, point.pt))
                {
                    //if the edge appears twice, that means it's an "interior" edge and will essentially create a triangle encapsulated by the polytope which we do not want
                    //we only want to store outer triangles that create our polytope, no extra interior triangles
                    if (!faultyEdges.Contains(tri.edge1))
                    {

                        faultyEdges.Add(tri.edge1);
                    }
                    else
                    {
                        faultyEdges.Remove(tri.edge1);

                    }

                    if (!faultyEdges.Contains(tri.edge2))
                    {
                        faultyEdges.Add(tri.edge2);
                    }
                    else
                    {
                        faultyEdges.Remove(tri.edge2);
                    }

                    if (!faultyEdges.Contains(tri.edge3))
                    {
                        faultyEdges.Add(tri.edge3);
                    }
                    else
                    {
                        faultyEdges.Remove(tri.edge3);
                    }

                    triangles.RemoveAt(i);
                }
            }

        }

        //confirmed
        public void AddPoint(SimplexPt point)
        {

            List<UndirectedEdge> faultyEdges = new List<UndirectedEdge>();


            CollectFaultyEdges(faultyEdges, point);
            ReconstructTriangles(faultyEdges, point);
        }

        //confirmed
        private void ReconstructTriangles(List<UndirectedEdge> faultyEdges, SimplexPt point)
        {
            for (int i = 0; i < faultyEdges.Count; i++)
            {
                UndirectedEdge e = faultyEdges[i];
                triangles.Add(new PolytopeTri(e.v1, e.v2, point));
            }
        }

        #endregion

        //Getter
        #region
        public PolytopeTri GetTriangle(int idx)
        {
            return triangles[idx];
        }

        public List<PolytopeTri> GetTriangles()
        {
            return triangles;
        }
        #endregion
    }

    public struct PolytopeTri
    {
        //Parameters
        #region
        public Vector3 normal;
        public float distToOrigin;
        public Vector3 centroid;

        public UndirectedEdge edge1;
        public UndirectedEdge edge2;
        public UndirectedEdge edge3;

        private SimplexPt v1;
        private SimplexPt v2;
        private SimplexPt v3;

        #endregion

        public PolytopeTri(SimplexPt a, SimplexPt b, SimplexPt c)
        {

            this.v1 = a;
            this.v2 = b;
            this.v3 = c;

            Vector3 areaVec = Vector3.Cross(v2.pt - v1.pt, v3.pt - v1.pt);

            //if area of triangle is less than a certain threshold, then we know at least 2 points in the triangle are essentially the same
            if (areaVec.sqrMagnitude / 2 < 4e-12f)
            {
                Debug.LogWarning("PolytopeTri is degenerate as points are either duplicated or all collinear!");
            }

            //Compute and Set
            #region
            this.normal = areaVec.normalized;

            //check if norm is pointing AWAY from the origin
            if (Vector3.Dot(v1.pt, normal) < 0) normal *= -1;

            this.centroid = new Vector3(v1.pt.x + v2.pt.x + v3.pt.x, v1.pt.y + v2.pt.y + v3.pt.y, v1.pt.z + v2.pt.z + v3.pt.z);

            //from sage old Mr. G, distance = |pw(vec) dotted with normal| / magnitude of normal
            //we choose norm to be unit vector so this simplifies quite a bit
            this.distToOrigin = Mathf.Abs(-v1.pt.x * normal.x - v1.pt.y * normal.y - v1.pt.z * normal.z);

            this.edge1 = new UndirectedEdge(v1, v2);
            this.edge2 = new UndirectedEdge(v1, v3);
            this.edge3 = new UndirectedEdge(v2, v3);
            #endregion


        }


        //Comparison Type Functions
        #region
        public override bool Equals(object obj)
        {
            if (!(obj is PolytopeTri)) return false;
            PolytopeTri other = (PolytopeTri)obj;

            return (edge1 == other.edge1 || edge1 == other.edge2 || edge1 == other.edge3)
                && (edge2 == other.edge1 || edge2 == other.edge2 || edge2 == other.edge3)
                && (edge3 == other.edge3 || edge3 == other.edge3 || edge3 == other.edge3);
        }

        public bool ContainsEdge(UndirectedEdge e)
        {
            return edge1.Equals(e) || edge2.Equals(e) || edge3.Equals(e);
        }

        public SimplexPt GetVertex(int num)
        {
            switch (num)
            {
                case 0:
                    return v1;
                case 1:
                    return v2;
                case 2:
                    return v3;
                default:
                    throw new Exception("'num' in GetVertex(int num) must be between 0-2 (inclusive on both ends)");
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion
    }

    public struct UndirectedEdge
    {
        //Parameters
        #region
        public SimplexPt v1;
        public SimplexPt v2;
        #endregion
        public UndirectedEdge(SimplexPt v1, SimplexPt v2)
        {
            if (v1 == v2)
            {
                throw new Exception("UndirectedEdge given two non-unique vertices!");
            }
            
            this.v1 = v1;
            this.v2 = v2;
        }

        //Equality

        #region
        public override bool Equals(object obj)
        {
            if (!(obj is UndirectedEdge)) return false;

            UndirectedEdge other = (UndirectedEdge)obj;
            return (this.v1 == other.v1 && this.v2 == other.v2) || (this.v1 == other.v2 && this.v2 == other.v1);
        }

        public static bool operator ==(UndirectedEdge e1, UndirectedEdge e2)
        {
            return e1.Equals(e2);
        }

        public static bool operator !=(UndirectedEdge e1, UndirectedEdge e2)
        {
            return !e1.Equals(e2);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion
    }

    #endregion




    //GJK Helper Functions
    #region



    /*
    public virtual Vector3 GetFurthestPoint(Vector3 dir)
    {

        return Vector3.zero;

    }
    */


    //A support point is given by the first collider's furthest point in a direction subtracted by the second collider's furthest point in the opposite direction
    private static Vector3 Support(IColliderShape col1, IColliderShape col2, Vector3 dir)
    {
        Vector3 sprtPt = col1.GetFurthestPoint(dir) - col2.GetFurthestPoint(-dir);
       // sprtPtToDirection.TryAdd(sprtPt, dir);
        return sprtPt;

    }

    #endregion

    //Main GJK Routine
    #region

    //GJK Algorithm - https://www.youtube.com/watch?v=MDusDn8oTSE (best explanation)


    //the bool is false if no collision, the simplex is the terminating or end simplex, the
    public static (bool, Simplex, List<SimplexPt>) CheckIfCollided(IColliderShape col1, IColliderShape col2)
    {

        //arbitrary direction
        Vector3 supportPoint = Support(col1, col2, Vector3.right);
        List<SimplexPt> simplexPtList = new List<SimplexPt>();


        Simplex simp = new Simplex();

        //0-simplex -- add any corner point

        simp.points = new List<SimplexPt>
        {
            new SimplexPt(supportPoint, Vector3.right)
        };

        simplexPtList.Add(simp.points[0]);

        //-support will point towards the origin (0, 0, 0)
        Vector3 direction = -supportPoint;

        int i = 0;

        while (i < 20)
        {
            //get the supportPoint closest in the direction pointing toward the origin
            supportPoint = Support(col1, col2, direction);

            //if our support point is the maximum we go in this direction, but we still can't reach the origin - then we haven't collided
            if (Vector3.Dot(supportPoint, direction) < 0)
            {
                return (false, simp, simplexPtList);

            }


            //otherwise, add this point to the simplex shape and increase the size
            SimplexPt newSPt = new SimplexPt(supportPoint, direction);

            simp.points.Add(newSPt);
            simplexPtList.Add(newSPt);


            if (OriginContained(ref simp, ref direction)) //check if the origin is contained within the simplex - it will only be true once we get to a tetrahedron
            {
                //terminatingSimplex = simp;
                return (true, simp, simplexPtList);
            }

            i++;

        }

        return (false, simp, simplexPtList);

    }

    //https://www.w3schools.blog/check-object-is-in-layermask-unity
    /*public bool InLayerMask(GameObject other)
    {
        //bitshifting type shit
        return ((ignoreLayers.value & (1 << other.layer)) > 0);

    }*/

    private static bool OriginContained(ref Simplex simp, ref Vector3 dir)
    {
        switch (simp.points.Count)
        {
            //1-simplex
            case 2:
                Lin(ref simp, ref dir);
                return false;

            //2-simplex
            case 3:
                Tri(ref simp, ref dir);
                return false;

            case 4:
                return Tetra(ref simp, ref dir);

        }

        return false;

    }



    //REFER TO MASTER NOTEBOOK FOR ALL DRAWINGS (The Blue one)

    private static void Lin(ref Simplex simp, ref Vector3 dir)
    {
        Vector3 a = simp.points[1].pt;
        Vector3 b = simp.points[0].pt;

        Vector3 toOrigin = -a; //because a is our support point

        //it's less than 0 because I did a - b and not b - a
        if (Vector3.Dot(a - b, toOrigin) < 0) //point is contained within the line bounds
        {

            dir = Vector3.Cross(Vector3.Cross(a - b, toOrigin), a - b);

        }
        else
        {
            //we got the wrong point, reset our direction pointed at the origin
            simp.points.RemoveAt(0);
            dir = toOrigin;
        }

    }

    private static void Tri(ref Simplex simp, ref Vector3 dir)
    {

        Vector3 a = simp.points[2].pt;
        Vector3 b = simp.points[1].pt;
        Vector3 c = simp.points[0].pt;

        Vector3 ac = a - c;
        Vector3 ab = a - b;

        Vector3 toOrigin = -a;

        Vector3 abac = Vector3.Cross(ab, ac);


        //CONFIRM
        if (Vector3.Dot(Vector3.Cross(ac, abac), toOrigin) > 0)//check for C-A Region or A Region
        {
            //CONFIRM
            if (Vector3.Dot(ac, toOrigin) < 0) //Confirm C-A Region
            {

                simp.points.RemoveAt(1);


                dir = Vector3.Cross(ac, abac);
                return;

            }
            else //Confirm A-Region
            {

                simp.points.RemoveRange(0, 2);
                dir = toOrigin;
                return;

            }

        }

        //CONFIRM

        if (Vector3.Dot(Vector3.Cross(abac, ab), toOrigin) > 0) //Check for B-A Region or A Region
        {
            if (Vector3.Dot(ab, toOrigin) < 0) //Confirm B-A Region
            {

                simp.points.RemoveAt(0);

                dir = Vector3.Cross(abac, ab);
                return;

            }
            else //Confirm A Region
            {

                simp.points.RemoveRange(0, 2);

                dir = toOrigin;
                return;

            }


        }

        if (Vector3.Dot(abac, toOrigin) > 0) //Confirm front
        {
            dir = abac;


        }
        else //Confirm back
        {

            dir = -abac;

        }

    }

    private static bool Tetra(ref Simplex simp, ref Vector3 dir)
    {
        Vector3 a = simp.points[3].pt;
        Vector3 b = simp.points[2].pt;
        Vector3 c = simp.points[1].pt;
        Vector3 d = simp.points[0].pt;

        Vector3 toOrigin = -a;
        //confirmed
        Vector3 abc = Vector3.Cross(a - b, a - c);
        Vector3 acd = Vector3.Cross(a - c, a - d);
        Vector3 abd = Vector3.Cross(a - d, a - b);


        //any of these triangle cases that result in return false mean that the created tetrahedron does not surround the origin but three of the vertices could be used

        if (Vector3.Dot(abc, toOrigin) < 0)
        {
            simp.points.RemoveAt(0);

            dir = -abc;
            Tri(ref simp, ref dir);
            return false;

        }


        if (Vector3.Dot(acd, toOrigin) < 0)
        {
            simp.points.RemoveAt(2);

            dir = -acd;
            Tri(ref simp, ref dir);

            return false;

        }

        if (Vector3.Dot(abd, toOrigin) < 0)
        {
            simp.points.RemoveAt(1);

            dir = -abd;
            Tri(ref simp, ref dir);

            return false;
        }

        return true;
    }

    #endregion



    //Getting Penetration Data AFTER GJK
    #region

    //EPA Helper
    #region



    //confirmed 


    //returns index of minNorm
    private static int GetMin(List<PolytopeTri> triangles)
    {

        int minIdx = 0;

        for (int i = 1; triangles.Count > i; i++)
        {
            if (triangles[i].distToOrigin < triangles[minIdx].distToOrigin) minIdx = i;

        }


        return minIdx;
    }


    #endregion

    //EPA Main Routine
    #region


    public static (Vector3, float, Vector3) GetCollisionData(IColliderShape col1, IColliderShape col2, Simplex terminatingSimplex, List<SimplexPt> sPtList)
    {
        //start with terminating simplex after running GJK
        Polytope pt = new Polytope(terminatingSimplex);
        List<PolytopeTri> polytopeTriangles = pt.GetTriangles();

        int minTriIdx;
        Vector3 minNorm;
        Vector3 epaSprtPt;
        float sprtPtDist;

        int m = 0;

        while (m < 30 && pt.GetTriangles().Count >= 4)
        {
            minTriIdx = GetMin(polytopeTriangles);
            minNorm = polytopeTriangles[minTriIdx].normal;
            epaSprtPt = Support(col1, col2, minNorm);

            //Distance from support point to closest plane
            //|pw * n| / ||n||, pw = epaSprtPt - a
            sprtPtDist = Mathf.Abs(Vector3.Dot(epaSprtPt - polytopeTriangles[minTriIdx].GetVertex(0).pt, minNorm));


            if (MinoMath.FApproximately(sprtPtDist, 0.000005f, supportThreshold))
            {
                //penetrationDepth = distance from origin to minTri
                float penetrationDepth = polytopeTriangles[minTriIdx].distToOrigin;
                Vector3 poc = GetPointOfContact(col1, polytopeTriangles[minTriIdx], sPtList);
                return (minNorm, penetrationDepth, poc);
            }

            SimplexPt newSPt = new SimplexPt(epaSprtPt, minNorm);

            pt.AddPoint(newSPt);
            sPtList.Add(newSPt);
            m++;
        }

        return (Vector3.zero, -1, Vector3.negativeInfinity);

    }
    #endregion

    //Extracting Point of Contact AFTER EPA
    #region
    //Check "Contact Manifold" in VRL Notebook
    //tri is closest face after running EPA 
    private static Vector3 GetPointOfContact(IColliderShape col1, PolytopeTri tri, List<SimplexPt> sPtList)
    {
        Vector3 contactPoint;
        Vector3 nPlane = tri.normal;

        if (nPlane == Vector3.zero)
        {
            //Debug.Log("Triangle had zero normal!");
            return Vector3.negativeInfinity;
        }

        Vector3 v1 = tri.GetVertex(0).pt;
        Vector3 v2 = tri.GetVertex(1).pt;
        Vector3 v3 = tri.GetVertex(2).pt;

        Vector3 e1 = v2 - v1;
        Vector3 e2 = v3 - v1;
        Vector3 e3 = v3 - v2;

        // Edge normals should point INWARD (toward opposite vertex)
        Vector3 n1 = Vector3.Cross(e1, nPlane).normalized;
        if (Vector3.Dot(n1, v3 - v1) < 0) n1 *= -1;  // Check against opposite vertex

        Vector3 n2 = Vector3.Cross(e2, nPlane).normalized;
        if (Vector3.Dot(n2, v2 - v1) < 0) n2 *= -1;

        Vector3 n3 = Vector3.Cross(e3, nPlane).normalized;
        if (Vector3.Dot(n3, v1 - v2) < 0) n3 *= -1;

        // Closest point on plane to origin
        Vector3 R = tri.distToOrigin * tri.normal;

        // Vectors from plane point to each vertex
        Vector3 r1 = v1 - R;
        Vector3 r2 = v2 - R;
        Vector3 r3 = v3 - R;

        Vector3 closestPoint = Vector3.zero;

        // Determine which region the closest point is in
        float d1 = Vector3.Dot(r1, n1);

        //float d2 = Vector3.Dot(r2, n2);
        // you can also use r3 here. v2 is on the opposite side of the triangle from edge 2, so r2 gives you a bad distance.
        float d2 = Vector3.Dot(r1, n2);
        
        
        float d3 = Vector3.Dot(r3, n3);

        if (d1 <= 0 && d2 <= 0 && d3 <= 0)
        {
            // Inside triangle
            closestPoint = R;
        }
        //else if (d1 > 0 && Vector3.Dot(r1, e1) <= 0 && Vector3.Dot(r1, -e1 - e2) >= 0)
        else if (d1 > 0 && Vector3.Dot(r1, e1) <= 0 && Vector3.Dot(r2, e1) >= 0)
        {
            // Outside edge e1 (v1-v2)
            float t = Vector3.Dot(R - v1, e1) / Vector3.Dot(e1, e1);
            t = Mathf.Clamp01(t);
            closestPoint = v1 + t * e1;
        }
        //else if (d2 > 0 && Vector3.Dot(r2, e2) <= 0 && Vector3.Dot(r2, -e2 - e1) >= 0)
        else if (d2 > 0 && Vector3.Dot(r1, e2) <= 0 && Vector3.Dot(r3, e2) >= 0)
        {
            // Outside edge e2 (v1-v3)
            float t = Vector3.Dot(R - v1, e2) / Vector3.Dot(e2, e2);
            t = Mathf.Clamp01(t);
            closestPoint = v1 + t * e2;
        }
        //else if (d3 > 0 && Vector3.Dot(r3, e3) <= 0 && Vector3.Dot(r3, -e3 + e1) >= 0)
        else if(d3 > 0 && Vector3.Dot(r2, e3) <= 0 && Vector3.Dot(r3, e3) >= 0)
        {
            // Outside edge e3 (v2-v3)
            float t = Vector3.Dot(R - v2, e3) / Vector3.Dot(e3, e3);
            t = Mathf.Clamp01(t);
            closestPoint = v2 + t * e3;
        }
        else
        {
            // Outside a vertex
            closestPoint = (Vector3.Distance(R, v1) < Vector3.Distance(R, v2)) ?
                          (Vector3.Distance(R, v1) < Vector3.Distance(R, v3) ? v1 : v3) :
                          (Vector3.Distance(R, v2) < Vector3.Distance(R, v3) ? v2 : v3);
        }

        // Compute barycentric coordinates for closestPoint
        float d0 = Vector3.Dot(e1, e1);
        float d1_dot = Vector3.Dot(e1, e2);
        float d2_dot = Vector3.Dot(e2, e2);
        float h0 = Vector3.Dot(closestPoint - v1, e1);
        float h1 = Vector3.Dot(closestPoint - v1, e2);

        float detA = d0 * d2_dot - d1_dot * d1_dot;

        if (Mathf.Abs(detA) < 0.0001f)
        {
            Debug.Log("Degenerate triangle in barycentric calculation!");
            contactPoint = Vector3.negativeInfinity;
            return contactPoint;
        }

        float beta = (h0 * d2_dot - h1 * d1_dot) / detA;
        float gamma = (d0 * h1 - d1_dot * h0) / detA;
        float alpha = 1 - beta - gamma;



        // Get support points for each vertex from the direction dictionary
        Vector3 globalPtV1 = GetDir(v1, sPtList);
        Vector3 globalPtV2 = GetDir(v2, sPtList);
        Vector3 globalPtV3 = GetDir(v3, sPtList);


        if (globalPtV1 == Vector3.negativeInfinity || globalPtV2 == Vector3.negativeInfinity || globalPtV3 == Vector3.negativeInfinity)
        {
            Debug.Log("One or more global points have failed!");
            return Vector3.negativeInfinity;
        }


        globalPtV1 = col1.GetFurthestPoint(globalPtV1);
        globalPtV2 = col1.GetFurthestPoint(globalPtV2);
        globalPtV3 = col1.GetFurthestPoint(globalPtV3);

        // Interpolate contact point using barycentric coordinates
        contactPoint = (alpha * globalPtV1 + beta * globalPtV2 + gamma * globalPtV3);
        return contactPoint;
    }

    public static Vector3 GetDir(Vector3 v, List<SimplexPt> pts)
    {
        foreach(SimplexPt pt in pts)
        {
            if (MinoMath.VApproximately(v, pt.pt, .001f)) return pt.dir;
        }

        return Vector3.negativeInfinity;
    }

    #endregion

    #endregion

}
