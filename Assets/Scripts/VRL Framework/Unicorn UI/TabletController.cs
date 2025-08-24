using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabletController : MonoBehaviour
{
    [SerializeField] private GameObject leftControllerPrefab;
    [SerializeField] private GameObject rightControllerPrefab;  

    [SerializeField] private ControllerInput pullUpTablet;

    private Canvas tabletCanvas;

    private void Start()
    {
        
        tabletCanvas = GetComponentInChildren<Canvas>();

    }

    public void ShowControls()
    {

        ControllerSimulator leftController = Instantiate(leftControllerPrefab).GetComponent<ControllerSimulator>();
        ControllerSimulator rightController = Instantiate(rightControllerPrefab).GetComponent<ControllerSimulator>();

    }

}
