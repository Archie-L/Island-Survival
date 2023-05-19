using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryItem : MonoBehaviour
{
    public Item itemData;
    public TMP_Text amountTxt;
    public GameObject activeObject, emptyObject;
    private GameObject activeEmpty;
    public GameObject output;
    public bool canBeActive;
    public bool isFuel;
    public bool canSmelt;

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

        if (canBeActive)
        {
            var Object = Instantiate(activeObject, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
            Object.transform.parent = activeEmpty.transform;
        }
        else
        {
            var Object = Instantiate(emptyObject, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
            Object.transform.parent = activeEmpty.transform;
        }
    }
}
