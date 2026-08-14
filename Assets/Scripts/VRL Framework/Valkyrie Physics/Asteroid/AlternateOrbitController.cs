using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlternateOrbitController : MonoBehaviour
{
    public ValkyrieRigidbody2 orbitCenter;
    private ValkyrieRigidbody2 myRigidbody;

    public float gravitationalConstant = 2;

    private void Awake()
    {
        myRigidbody = GetComponent<ValkyrieRigidbody2>();


    }

    private void FixedUpdate()
    {

        float gravitationMagnitude = (gravitationalConstant * myRigidbody.GetMass() * orbitCenter.GetMass()) / Mathf.Pow((orbitCenter.transform.position - this.transform.position).magnitude, 2);

        transform.position += gravitationMagnitude * (orbitCenter.transform.position - this.transform.position) * Time.fixedDeltaTime;

    }
}
