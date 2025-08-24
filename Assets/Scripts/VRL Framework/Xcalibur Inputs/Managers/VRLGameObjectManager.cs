using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HandTracker;

[ExecuteAlways]
public sealed class VRLGameObjectManager : MonoBehaviour
{

    /*
     * GAMEOBJECTS
     */

    [SerializeField] RadixTracker radixTracker;
    [SerializeField] HandTracker leftHandTracker;
    [SerializeField] HandTracker rightHandTracker;
    [SerializeField] ValkChestSimulator chestSimulator;


    private static Camera playerHeadCamera;

    public static VRLGameObjectManager instance = null;
    private static readonly object padlock = new object();


    private VRLGameObjectManager()
    {


    }

    public static VRLGameObjectManager Instance
    {
        get
        {
            lock (padlock)
            {

                instance = FindObjectOfType<VRLGameObjectManager>();
                
            }

            return instance;
        }
        
    }

    private void Awake()
    {
        instance = this;
        playerHeadCamera = radixTracker.headCam;
    }



    public HandTracker GetHandTracker(GameObject gameObject)
    {
        if (gameObject.tag == "Right Hand")
        {
            return TryGetRightHand();
            
        } else if(gameObject.tag == "Left Hand")
        {

            return TryGetLeftHand();

        }

        return null;
    }

    public HandTracker GetHandTracker(HandType handType)
    {

        return handType.Equals(HandType.Left) ? TryGetLeftHand() : TryGetRightHand();


    }

    public RadixTracker TryGetRadixTracker()
    {
        if (radixTracker == null) Debug.LogWarning("No radix tracker found!");
        return radixTracker;

    }

    public static Camera GetPlayerHeadCamera()
    {

        if (playerHeadCamera == null) Debug.LogWarning("Player Head Camera not assigned!");
        return playerHeadCamera;

    }

    public HandTracker TryGetLeftHand()
    {

        if (leftHandTracker == null) Debug.LogWarning("Left hand not assigned!");

        return leftHandTracker;

    }

    public HandTracker TryGetRightHand()
    {
        if (rightHandTracker == null) Debug.LogWarning("Right hand not assigned!");

        return rightHandTracker;

    }

    public ValkChestSimulator TryGetChestSimulator()
    {

        if (chestSimulator == null) Debug.LogWarning("No chest simulator found!");
        return chestSimulator;

    }


   

}
