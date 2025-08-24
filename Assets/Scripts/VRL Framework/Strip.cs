using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteAlways]
public class Strip : MonoBehaviour
{

    public LayerMask ignoreLayers;
    public bool commit = false;

    // Start is called before the first frame update
    void Start()
    {
        Transform[] children = GetComponentsInChildren<Transform>();

        foreach (Transform child in children)
        {
            if (child.name.Contains("Asteroid Cluster"))
            {

                // if (child.gameObject.GetComponent<ValkyrieRigidbody>()) DestroyImmediate(child.GetComponent<ValkyrieRigidbody>());
               // if (child.GetComponent<OrbitController>()) DestroyImmediate(child.GetComponent<OrbitController>());

                if (child.GetComponent<OrbitController>() == null)
                {

                    child.gameObject.AddComponent<OrbitController>();
                    OrbitController oc = child.GetComponent<OrbitController>();

                    oc.orbitalPeriod = Random.Range(50f, 800f);
                    oc.center = GameObject.Find("Sun").GetComponent<ValkyrieRigidbody>();

                    oc.eccentricity = Random.Range(0.01f, 0.6f);

                }

                /*ValkyrieRigidbody childRigidbody = child.GetComponent<ValkyrieRigidbody>();
                ValkyrieSphereCollider collider = child.GetComponent<ValkyrieSphereCollider>();

                collider.radius = 120f;
                collider.drawInEditor = true;
                collider.ignoreLayers = ignoreLayers;

                childRigidbody.isKinematic = false;
                childRigidbody.mass = 60000000000f;
                childRigidbody.momentOfInertia = 3;
                childRigidbody.applyGravity = false;*/

            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        Transform[] children = GetComponentsInChildren<Transform>();

        foreach (Transform child in children)
        {
            if (child.name.Contains("Rock"))
            {

               // if (child.gameObject.GetComponent<ValkyrieRigidbody>()) DestroyImmediate(child.GetComponent<ValkyrieRigidbody>());
               // if (child.GetComponent<ValkyrieSphereCollider>()) DestroyImmediate(child.GetComponent<ValkyrieSphereCollider>());

                if(child.GetComponent<OrbitController>() == null)
                {

                    child.gameObject.AddComponent<OrbitController>();
                    OrbitController oc = child.GetComponent<OrbitController>();

                    oc.orbitalPeriod = Random.Range(50f, 800f);
                    oc.center = GameObject.Find("Sun").GetComponent<ValkyrieRigidbody>();

                    oc.eccentricity = Random.Range(0.01f, 0.6f);

                }

                /*ValkyrieRigidbody childRigidbody = child.GetComponent<ValkyrieRigidbody>();
                ValkyrieSphereCollider collider = child.GetComponent<ValkyrieSphereCollider>();

                collider.radius = 120f;
                collider.drawInEditor = true;
                collider.ignoreLayers = ignoreLayers;

                childRigidbody.isKinematic = false;
                childRigidbody.mass = 60000000000f;
                childRigidbody.momentOfInertia = 3;
                childRigidbody.applyGravity = false;*/

            }
        }
    }
}
