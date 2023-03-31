using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public Slot[] slots;
    public GameObject gameUI;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].slotId = i;
            slots[i].GetComponentInChildren<GrabManager>().dragHandler = gameUI;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
