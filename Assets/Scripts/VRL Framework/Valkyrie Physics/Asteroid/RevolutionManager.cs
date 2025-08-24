using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevolutionManager : MonoBehaviour
{
    [Header("Axis Settings")]
    [SerializeField] private Vector3 axisOfRevolution;
    [Header("Speed Settings")]
    [SerializeField] private float speed = 20f;
    [SerializeField] private float angleDelta = 0.2f;

    private ValkyrieRigidbody vrb;


    private void Start()
    {

        vrb = GetComponent<ValkyrieRigidbody>();

    }

    private void Update()
    {
        transform.Rotate(axisOfRevolution, angleDelta * speed * Time.deltaTime);



    }
}
