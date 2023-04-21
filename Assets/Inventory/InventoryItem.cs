using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryItem : MonoBehaviour
{
    public Item itemData;
    public TMP_Text amountTxt;

    public int amount = 1;

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
}
