using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TaskBoard : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI[] rotatingText;
    [SerializeField] TextMeshProUGUI[] sidebarText;

    

    public int currentRotatingIndex = 0;
    public int currentSidebarIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < rotatingText.Length; i++)
        {
            if (currentRotatingIndex == i) rotatingText[currentRotatingIndex].gameObject.SetActive(true);
            else rotatingText[i].gameObject.SetActive(false);


        }

        for(int j = 0; j < sidebarText.Length; j++)
        {

            sidebarText[j].gameObject.SetActive(false);

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void NextText()
    {
        rotatingText[currentRotatingIndex].gameObject.SetActive(false);

        if (currentRotatingIndex < rotatingText.Length - 1) currentRotatingIndex++;
        else currentRotatingIndex = 0;

        rotatingText[currentRotatingIndex].gameObject.SetActive(true);

    }

    public void NextSidebarText()
    {
        sidebarText[currentSidebarIndex].gameObject.SetActive(true);


        if (currentSidebarIndex < sidebarText.Length - 1) currentSidebarIndex++;
        else currentSidebarIndex = 0;

        rotatingText[currentRotatingIndex].gameObject.SetActive(false);
    }
}
