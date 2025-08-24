using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


[ExecuteAlways]
public class ValkyrieTester : MonoBehaviour
{
    private ValkyrieRigidbody m_Rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        m_Rigidbody = GetComponent<ValkyrieRigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.pKey.isPressed)
        {
            m_Rigidbody.Accelerate(transform.forward * 0.1f);
        }

    }
}
