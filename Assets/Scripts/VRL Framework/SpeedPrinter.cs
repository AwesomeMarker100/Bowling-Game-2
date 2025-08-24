using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpeedPrinter : MonoBehaviour
{

    [SerializeField] ValkyrieRigidbody vrb;
    [SerializeField] PlayMusicNode musicNode;
    [SerializeField] TextMeshProUGUI speedText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        speedText.text = "Speed: " + vrb.velocity.magnitude;
    }
}
