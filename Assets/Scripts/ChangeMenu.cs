using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMenu : MonoBehaviour
{
    public GameObject thisMenu;
    public GameObject nextMenu;

    public void SwitchMenu()
    {
        thisMenu.SetActive(false);
        nextMenu.SetActive(true);
    }
}
