using System;
using UnityEngine;

using Object = UnityEngine.Object;

public class SpawnObjectNode : SprigganNode
{
    [SerializeField] public GameObject[] objectsToSpawn;

    public override async Awaitable Execute()
    {
        foreach(GameObject go in objectsToSpawn)
        {
            if (go == null) continue;
            Object.Instantiate(go, Vector3.zero, Quaternion.identity);
            await Awaitable.NextFrameAsync();
        }
    }

}