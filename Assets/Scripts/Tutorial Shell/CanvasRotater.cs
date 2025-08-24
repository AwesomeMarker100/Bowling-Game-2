using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasRotater : MonoBehaviour
{

    public Transform target;
    public float rotateFactor = 2f;

    // Update is called once per frame
    private void Update()
    {
        Vector3 targetDirectino = target.position - this.transform.position;
        Vector3 rotateBy = Vector3.RotateTowards(transform.forward, -targetDirectino, Time.deltaTime * rotateFactor, 0f);

        transform.rotation = Quaternion.LookRotation(rotateBy);

    }
}
