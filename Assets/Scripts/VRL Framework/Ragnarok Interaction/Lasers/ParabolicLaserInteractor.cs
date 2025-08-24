using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParabolicLaserInteractor : LaserInteractor
{
    [SerializeField] private float parabolaScale = 0.05f;

    [SerializeField] private Vector2 vertex = Vector2.zero;
    private float aValue = 2f;

    private void Awake()
    {

        this.laserType = LaserType.Parabolic;

    }

    //USES TRANSFORM.POSITION AS 0, 0 ORIGIN
    public override void CreateLaser()
    {
        //y = ax^2 + bx + c
        laser.positionCount = (int)(maximumDistance / parabolaScale) + 1;

        int i = 0;
        float x = 0f;

        //if they set the vertex, calculate the a value for the equation
        aValue = -vertex.y/ Mathf.Pow(-vertex.x, 2); 

        while (x <= maximumDistance)
        {
            float y = aValue * Mathf.Pow(x - vertex.x, 2) + vertex.y; 
            Vector3 finalPos = transform.rotation * Quaternion.Euler(rotationOffset) * new Vector3(x, y, 0f);

            laser.SetPosition(i, transform.position + finalPos);
           // if (DidCollide(transform.position + finalPos, i)) break;

            i++;
            x += parabolaScale;
        }

        laser.positionCount = i;
    }
}
