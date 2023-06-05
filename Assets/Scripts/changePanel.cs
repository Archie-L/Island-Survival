using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class changePanel : MonoBehaviour
{
    public GameObject thisPanel;
    public GameObject currentPanel;

    // Update is called once per frame
    void Update()
    {
        currentPanel = GameObject.FindGameObjectWithTag("panel");
    }

    public void OpenPanel()
    {
        currentPanel.SetActive(false);
        thisPanel.SetActive(true);
    }
}
