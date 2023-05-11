using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class chestScript : MonoBehaviour
{
    [Header("Inventory & Management")]

    public GameObject chest;

    public Transform chestSlotHolder;

    public Transform cursor;
    public Vector3 offset;

    public List<bool> isFull;
    public List<Transform> slots;

    public int currentSlot;

    private void Start()
    {
        InitializeInventory();
        SetSlotsIDs();
        CheckSlots();
    }

    private void Update()
    {
        if (chest.activeSelf == true)
        {
            cursor.position = Input.mousePosition + offset;
        }
        if (cursor.childCount > 0)
        {
            cursor.gameObject.SetActive(true);
        }
        else
        {
            cursor.gameObject.SetActive(false);
        }
    }

    void InitializeInventory()
    {
        for (int i = 0; i < chestSlotHolder.childCount; i++)
        {
            slots.Add(chestSlotHolder.GetChild(i));
            isFull.Add(false);
        }
    }

    void SetSlotsIDs()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].GetComponent<Slot>() != null)
            {
                slots[i].GetComponent<Slot>().ID = i;
            }
        }
    }

    void CheckSlots()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].childCount > 0)
            {
                isFull[i] = true;
            }
            else
            {
                isFull[i] = false;
            }
        }
    }
    public void PickUpDropInventory()
    {
        if (slots[currentSlot].childCount > 0 && cursor.childCount < 1)
        {
            Instantiate(slots[currentSlot].GetChild(0).gameObject, cursor);
            Destroy(slots[currentSlot].GetChild(0).gameObject);
        }
        else if (slots[currentSlot].childCount < 1 && cursor.childCount > 0)
        {
            Instantiate(cursor.GetChild(0).gameObject, slots[currentSlot]);
            Destroy(cursor.GetChild(0).gameObject);
        }
        else if (slots[currentSlot].childCount > 0 && cursor.childCount > 0)
        {
            if (slots[currentSlot].GetChild(0).GetComponent<InventoryItem>().itemData.ID == cursor.GetChild(0).GetComponent<InventoryItem>().itemData.ID)
            {
                if (slots[currentSlot].GetChild(0).GetComponent<InventoryItem>().amount <= cursor.GetChild(0).GetComponent<InventoryItem>().itemData.maxStack - slots[currentSlot].GetChild(0).GetComponent<InventoryItem>().amount)
                {
                    slots[currentSlot].GetChild(0).GetComponent<InventoryItem>().amount += cursor.GetChild(0).GetComponent<InventoryItem>().amount;
                    Destroy(cursor.GetChild(0).gameObject);
                }
            }
        }
        CheckSlots();
    }
}
