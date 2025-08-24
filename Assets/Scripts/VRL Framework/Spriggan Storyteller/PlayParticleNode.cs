using System;
using UnityEngine;

[Serializable]
public class PlayParticleNode : SprigganNode
{

    public enum SystemPlayType
    {
        Transform, Position
    }

    [SerializeField] ParticleSystem particleSystem;
    [SerializeField] float duration;
    [SerializeField] Transform parent;
    //[SerializeField] Vector3 

    public override async Awaitable Execute()
    {

    }
}
