using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


[ExecuteAlways]

[RequireComponent(typeof(ValkyrieRigidbody2))]
public class ValkyrieTester : MonoBehaviour
{
    private ValkyrieRigidbody2 m_Rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        m_Rigidbody = GetComponent<ValkyrieRigidbody2>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.pKey.isPressed)
        {
            m_Rigidbody.ApplyForce(transform.forward * 4f);
        }

    }
}
