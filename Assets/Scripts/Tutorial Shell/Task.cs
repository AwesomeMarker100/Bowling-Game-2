using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Task : MonoBehaviour
{

    [SerializeField] bool isOpen;
    [HideInInspector] public RadixTracker player;

    public virtual void Start()
    {
        
        player = FindObjectOfType<RadixTracker>();

    }

    public void OpenTask()
    {

        isOpen = true;

    }

    public void CloseTask()
    {

        isOpen = false;

    }

    private void Update()
    {

        if (isOpen)
        {

            if (IsTaskDone())
            {

                CloseTask();

            }

        }

    }

    public virtual bool IsTaskDone()
    {

        return false;

    }
}
