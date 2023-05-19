using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class lootCrate : MonoBehaviour
{
    public GameObject parent;

    public Transform inventorySlotHolder;
    public List<Transform> slots;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < inventorySlotHolder.childCount; i++)
        {
            slots.Add(inventorySlotHolder.GetChild(i));
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            Transform slot = slots[i];
            if (slot.childCount > 0)
            {

            }
        }
    }
}
