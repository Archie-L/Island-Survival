using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [Header("Inventory & Management")]

    public GameObject inventory;

    public Transform inventorySlotHolder;
    public Transform inventoryHotbarSlotHolder;

    public Transform cursor;
    public Vector3 offset;

    public List<bool> isFull;
    public List<Transform> slots;
    public List<Transform> slotsHotbar;

    public int currentSlot;

    public GameObject emptyObject;
    private GameObject activeEmpty;


    private void Start()
    {
        InitializeInventory();
        SetSlotsIDs();
        CheckSlots();

        activeEmpty = GameObject.Find("Activated Object");
    }

    private void Update()
    {
        if(inventory.activeSelf == true)
        {
            cursor.position = Input.mousePosition + offset;
        }
        if(cursor.childCount > 0)
        {
            cursor.gameObject.SetActive(true);
        }
        else
        {
            cursor.gameObject.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (slotsHotbar[0].transform.childCount > 0)
            {
                slotsHotbar[0].GetChild(0).GetComponent<InventoryItem>().ActivateObject();
            }
            if (slotsHotbar[0].transform.childCount == 0)
            {
                NoObject();
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (slotsHotbar[1].transform.childCount > 0)
            {
                slotsHotbar[1].GetChild(0).GetComponent<InventoryItem>().ActivateObject();
            }
            if (slotsHotbar[1].transform.childCount == 0)
            {
                NoObject();
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (slotsHotbar[2].transform.childCount > 0)
            {
                slotsHotbar[2].GetChild(0).GetComponent<InventoryItem>().ActivateObject();
            }
            if (slotsHotbar[2].transform.childCount == 0)
            {
                NoObject();
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (slotsHotbar[3].transform.childCount > 0)
            {
                slotsHotbar[3].GetChild(0).GetComponent<InventoryItem>().ActivateObject();
            }
            if (slotsHotbar[3].transform.childCount == 0)
            {
                NoObject();
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            if (slotsHotbar[4].transform.childCount > 0)
            {
                slotsHotbar[4].GetChild(0).GetComponent<InventoryItem>().ActivateObject();
            }
            if (slotsHotbar[4].transform.childCount == 0)
            {
                NoObject();
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            if (slotsHotbar[5].transform.childCount > 0)
            {
                slotsHotbar[5].GetChild(0).GetComponent<InventoryItem>().ActivateObject();
            }
            if (slotsHotbar[5].transform.childCount == 0)
            {
                NoObject();
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            if (slotsHotbar[6].transform.childCount > 0)
            {
                slotsHotbar[6].GetChild(0).GetComponent<InventoryItem>().ActivateObject();
            }
            if (slotsHotbar[6].transform.childCount == 0)
            {
                NoObject();
            }
        }
    }
    public void NoObject()
    {
        Destroy(activeEmpty.transform.GetChild(0).gameObject);

        var Object = Instantiate(emptyObject, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);

        Object.transform.parent = activeEmpty.transform;
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

    public void CraftItem(int[] IDs, int[] IDsAmounts, GameObject outCome, int outComeAmount)
    {
        bool[] collected = new bool[IDs.Length];
        Transform[] collectedSlots = new Transform[IDs.Length];

        for (int x = 0; x < IDs.Length; x++)
        {
            for (int i = 0; i < slots.Count; i++)
            {
                if (isFull[i] == true)
                {
                    if (slots[i].GetChild(0).GetComponent<InventoryItem>().itemData.ID == IDs[x] && slots[i].GetChild(0).GetComponent<InventoryItem>().amount >= IDsAmounts[x])
                    {
                        collected[x] = true;
                        collectedSlots[x] = slots[i].GetChild(0);
                    }
                }
            }
        }
        for (int i = 0; i < collected.Length; i++)
        {
            if (collected[i] == false)
            {
                return;
            }
        }
        for (int i = 0; i < collectedSlots.Length; i++)
        {
            collectedSlots[i].GetComponent<InventoryItem>().amount -= IDsAmounts[i];
        }
        for (int i = 0; i < outComeAmount; i++)
        {
            AddItem(outCome);
        }
    }

    public void AddItem(GameObject item)
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
