using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidDustHandler : MonoBehaviour
{

    private new ValkyrieCollider collider;


    [SerializeField] ParticleSystem[] particleRange;
    [SerializeField] public float[] startRangeVal;

    // Start is called before the first frame update
    void Start()
    {

        collider = this.GetComponent<ValkyrieCollider>();
        collider.SubscribeToCollisionAwake(MoveDust);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveDust(ValkyrieCollision collision)
    {



    }
    
}
