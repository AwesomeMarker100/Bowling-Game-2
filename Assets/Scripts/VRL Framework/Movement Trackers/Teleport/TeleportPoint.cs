using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportPoint : MonoBehaviour
{

    [Tooltip("Keeps the player following this point")] public bool parentPlayer;
    public bool openToTeleport = true;

    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (openToTeleport)
        {


        }
    }

    public void Teleport(GameObject go)
    {
        if (openToTeleport)
        {
            player = go;

            if (parentPlayer)
            {
                go.transform.SetParent(transform, true);
            }

            go.transform.position = this.transform.position;
        }

    }

    public void ClearParent()
    {

        if(player != null && parentPlayer)
        {

            player.transform.parent = null;

        }

    }

    private void FixedUpdate()
    {
        
    }
}
