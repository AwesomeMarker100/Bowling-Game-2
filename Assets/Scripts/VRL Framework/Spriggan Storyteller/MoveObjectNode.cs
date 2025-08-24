using System;
using UnityEngine;

[Serializable]
public class MoveObjectNode : SprigganNode
{
    public GameObject gameObject;
    public Vector3 position;

    public override async Awaitable Execute()
    {
        gameObject.transform.position = position;
        await Awaitable.EndOfFrameAsync();
    }
}
