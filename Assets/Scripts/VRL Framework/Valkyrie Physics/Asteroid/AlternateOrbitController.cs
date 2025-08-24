using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlternateOrbitController : MonoBehaviour
{
    public ValkyrieRigidbody orbitCenter;
    private ValkyrieRigidbody myRigidbody;

    public float gravitationalConstant = 2;

    private void Awake()
    {
        myRigidbody = GetComponent<ValkyrieRigidbody>();


    }

    private void FixedUpdate()
    {

        float gravitationMagnitude = (gravitationalConstant * myRigidbody.mass * orbitCenter.mass) / Mathf.Pow((orbitCenter.transform.position - this.transform.position).magnitude, 2);

        transform.position += gravitationMagnitude * (orbitCenter.transform.position - this.transform.position) * Time.fixedDeltaTime;

    }
}
