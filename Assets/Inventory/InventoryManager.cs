using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public GameObject inventory;

    public Transform inventorySlotHolder;
    public Transform inventoryHotbarSlotHolder;

    public Transform cursor;
    public Vector3 offset;

    public List<bool> isFull;
    public List<Transform> slots;
    public List<Transform> slotsHotbar;

    public int currentSlot;

    private void Start()
    {
        InitializeInventory();
        SetSlotsIDs();
        CheckSlots();
    }

    private void Update()
    {
        if(inventory.activeSelf == true)
        {
            cursor.position = Input.mousePosition + offset;
        }
        if(cursor.childCount > 0)
        {
            cursor.gameObject.SetActive(false);
        }
    }

    void InitializeInventory()
    {
        for (int i = 0; i < inventorySlotHolder.childCount; i++)
        {
            slots.Add(inventorySlotHolder.GetChild(i));
            isFull.Add(false);
        }
        for (int i = 0; i < inventoryHotbarSlotHolder.childCount; i++)
        {
            slots.Add(inventoryHotbarSlotHolder.GetChild(i));
            slotsHotbar.Add(inventoryHotbarSlotHolder.GetChild(i));
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

    void AddItem(GameObject item, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            for (int x = 0; x < slots.Count; x++)
            {
                if (isFull[x] == false)
                {
                    Instantiate(item, slots[x]);
                    CheckSlots();
                    return;
                }
                else
                {
                    Debug.Log("Slot full");
                }
            }
        }
        Debug.Log("All slots full");
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
    }
}
