using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DynamicText : MonoBehaviour
{

    [SerializeField] MonoBehaviour trackedComponent;
    [Tooltip("Must refer to a string or something that is printable")][SerializeField] string trackedFieldName;

    private TextMeshProUGUI textMeshPro;


    // Start is called before the first frame update
    void Start()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();  
    }

    // Update is called once per frame
    void Update()
    {

        textMeshPro.text = trackedComponent.GetType().GetField(trackedFieldName).GetValue(trackedComponent).ToString();

    }
}
