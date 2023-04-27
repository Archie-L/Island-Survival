using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryItem : MonoBehaviour
{
    public Item itemData;
    public TMP_Text amountTxt;
    public GameObject activeObject;
    private GameObject activeEmpty;

    public int amount = 1;

    private void Start()
    {
        activeEmpty = GameObject.Find("Activated Object");
    }

    private void Update()
    {
        if(amount <= 1)
        {
            amountTxt.gameObject.SetActive(false);
        }
        else
        {
            amountTxt.gameObject.SetActive(true);
        }
        amountTxt.text = amount.ToString();
    }

    public void ActivateObject()
    {
        Destroy(activeEmpty.transform.GetChild(0).gameObject);

        var Object = Instantiate(activeObject, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);

        Object.transform.parent = activeEmpty.transform;
    }
}
