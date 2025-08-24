using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//SCRIPT SHOULD EVENTUALLY BE TRASHED AND INCORPORATED INTO ASPECT CONTROLLER
public class LaserCycler : MonoBehaviour
{
    [SerializeField] private LaserInteractor[] cycledInteractors;
    [SerializeField] private ControllerInput cycleInput;
    
    
    
    private LaserInteractor currentInteractor;
    private int currentIndex = 0;

    private void Start()
    {
        OculusInputManager.SubscribeToEvent(cycleInput, Cycle);

        foreach (LaserInteractor interactor in cycledInteractors)
        {
            interactor.enabled = false;
        }

        if (cycledInteractors.Length > 0) { currentInteractor = cycledInteractors[currentIndex]; currentInteractor.enabled = true; }
    }


    private void Cycle(ControllerInputInfo inputInfo)
    {
        currentInteractor.enabled = false;

        if (currentIndex == cycledInteractors.Length - 1) currentIndex = 0;
        else currentIndex++;

        currentInteractor = cycledInteractors[currentIndex];
        currentInteractor.enabled = true;
    }

}
