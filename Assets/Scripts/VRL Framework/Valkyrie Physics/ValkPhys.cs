using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//EQUIVALENT OF THE PHYSICS LIBRARY IN UNITY
public class ValkPhys : MonoBehaviour
{

    private List<KDRegion<ValkyrieCollider>> regions;
    Vector3[] axes =
    {

        Vector3.right, //x-axis
        Vector3.up //y-axis

    };

    private static KDTree<ValkyrieCollider> colTree;
    private ValkyrieCollider[] colliders;

    private List<ValkyrieCollision> collisions;
    public int raycastStepSize = 50;

    public int collidersPerRegion = 8;


    private void Start()
    {
        colliders = FindObjectsOfType<ValkyrieCollider>();
        regions = new List<KDRegion<ValkyrieCollider>>();

        CreateColliderKDTree();
    }

    private UICanvas GetCanvas(Vector3 point)
    {
        UICanvas[] canvii = FindObjectsOfType<UICanvas>();
        UICanvas closestCanvas = canvii[0];

        for(int i = 1; canvii.Length > i; i++)
        {
            if(Vector3.Distance(point, closestCanvas.transform.position) > Vector3.Distance(point, canvii[i].transform.position))
            {

                closestCanvas = canvii[i];

            }

        }

        return closestCanvas;

    }

    #region 

    //3D Raycasting
    #region
    public bool Raycast(Vector3 start, Vector3 dir, float distance, LayerMask ignoreLayers, out ValkyrieRaycastHit hit)
    {
        //set a step amount to check for collisions
        hit = null;
        float stepAmount = distance / raycastStepSize;

        Vector3 curPoint = start;
        float distanceTraveled = 0;

        //get the region that the current point falls in and the canvas as well

        KDRegion<ValkyrieCollider> curRegion = GetRegion(curPoint);
        UICanvas curCanvas = GetCanvas(curPoint);

        //while the travel distance is less than the distance specified by the user to check
        while (distanceTraveled <= distance)
        {

            if (curRegion != null)
            {
                //iterate over each collider in the region
                foreach (ValkyrieCollider col in curRegion.GetMembers())
                {
                    if (InLayerMask(ignoreLayers, col.gameObject)) continue;

                    //if the point is in collider bounds, then return true since that means we've hit something
                    if (col.PointInBounds(curPoint))
                    {
                        hit = new ValkyrieRaycastHit(curPoint, col);
                        return true;
                    }
                }

            }

            if (curCanvas != null)
            {
                foreach (HittableObject ho in curCanvas.GetHittableObjects())
                {
                    if(InLayerMask(ignoreLayers, ho.gameObject)) continue;

                    if (ho.PointInBounds(curPoint))
                    {
                        hit = new ValkyrieRaycastHit(curPoint, ho);
                        return true;
                    }

                }
            }

            //add the step amount to the total distanceTraveled and do the same for position
            distanceTraveled += stepAmount;
            curPoint += dir.normalized * stepAmount;
        }

        return false;


    }

    public bool InLayerMask(LayerMask ignoreLayers, GameObject other)
    {

        return ((ignoreLayers.value & (1 << other.layer)) > 0);

    }




    #endregion

    //SPATIAL RECOGNITION CODE BELOW

    //get region functions, using approximate-nearest neighbors
    #region

    public void CreateColliderKDTree()
    {

        Vector3[] positions = new Vector3[colliders.Length];

        for (int i = 0; i < positions.Length; i++)
        {
            positions[i] = colliders[i].transform.position;

        }


        colTree = new KDTree<ValkyrieCollider>(colliders, positions);
        colTree.Build(collidersPerRegion);

        KDRegion<ValkyrieCollider>[] regions = colTree.GetEndRegions();


        print("Total Collider Count: " + colliders.Length);

        for (int i = 0; i < regions.Length; i++)
        {
             

            foreach (ValkyrieCollider vc in regions[i].GetMembers())
            {
                vc.region = regions[i];


            }


        }



    }

    public static KDRegion<ValkyrieCollider> GetRegion(Vector3 point)
    {

        return colTree.GetRegion(point);

    }

    public static void SetNewRegion(ValkyrieCollider col)
    {
        KDRegion<ValkyrieCollider> newRegion = GetRegion(col.transform.position);

        col.region.RemoveMember(col);

        col.region = newRegion;

        newRegion.AddMember(col);

    }


    #endregion

    #endregion

   
}
