using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICanvas : MonoBehaviour
{

    private HittableObject[] hittableObjects;

    private void Start()
    {
        hittableObjects = GetComponentsInChildren<HittableObject>();
    }

    public HittableObject[] GetHittableObjects()
    {

        return hittableObjects; 

    }
}
