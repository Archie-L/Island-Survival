using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Experimental.GraphView.GraphView;

public class furnaceContoller : MonoBehaviour
{
    public Transform fuelSlot;
    public Transform inputSlot;
    public Transform outputSlot;

    private bool fuelActive;
    private bool inputActive;

    public Slider smeltBar;

    private float nextActionTime = 0.0f;
    public float period = 1f;

    private bool wait;

    // Start is called before the first frame update
    void Start()
    {
        wait = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(fuelSlot.GetChild(0).GetComponent<InventoryItem>().isFuel == true)
        {
            fuelActive = true;
        }

        if (inputSlot.GetChild(0).GetComponent<InventoryItem>().canSmelt == true)
        {
            inputActive = true;
        }

        if(inputActive && fuelActive)
        {
            StartSmelt();
        }
        else if (!inputActive || !fuelActive)
        {
            smeltBar.value = 0;
        }
    }

    void StartSmelt()
    {
        if (wait)
        {
            wait = false;
            StartCoroutine(increaseBar());
            if(smeltBar.value >= smeltBar.maxValue)
            {
                RemoveItems();
                smeltBar.value = smeltBar.minValue;
            }
        }
    }
    IEnumerator increaseBar()
    {
        yield return new WaitForSeconds(0.5f);
        smeltBar.value = smeltBar.value + 1;
        wait = true;
    }

    void RemoveItems()
    {
        fuelSlot.GetChild(0).GetComponent<InventoryItem>().amount -= 1;
        inputSlot.GetChild(0).GetComponent<InventoryItem>().amount -= 1;

        var outputItem = inputSlot.GetChild(0).GetComponent<InventoryItem>().output;
        if(outputSlot.childCount == 0)
        {
            Instantiate(outputItem, outputSlot.transform);
        }
        if(outputSlot.childCount > 0 && outputSlot.GetChild(0).GetComponent<InventoryItem>().itemData.ID == outputItem.GetComponent<InventoryItem>().itemData.ID)
        {

            outputSlot.GetChild(0).GetComponent<InventoryItem>().amount += 1;
        }
    }
}
