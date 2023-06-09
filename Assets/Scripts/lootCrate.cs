using System.Collections.Generic;
using UnityEngine;

public class lootCrate : MonoBehaviour
{
    public Transform inventorySlotHolder;
    private List<Transform> slots = new List<Transform>();
    private InventoryManager manager;
    private GameObject player;
    private monumentManager monu;
    public bool isLocked;

    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<InventoryManager>();
        player = GameObject.FindGameObjectWithTag("Player");
        monu = GetComponentInParent<monumentManager>();

        for (int i = 0; i < inventorySlotHolder.childCount; i++)
        {
            slots.Add(inventorySlotHolder.GetChild(i));
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<InventoryManager>();
        player = GameObject.FindGameObjectWithTag("Player");
        monu = GetComponentInParent<monumentManager>();

        bool allSlotsEmpty = true;

        for (int i = 0; i < slots.Count; i++)
        {
            Transform slot = slots[i];
            if (slot.childCount > 0)
            {
                allSlotsEmpty = false;
                break;
            }
        }

        if (allSlotsEmpty && player.GetComponent<PlayerMovement>().chestOpen == false)
        {
            if (!isLocked)
            {
                monu.spawnedCrates.Remove(this.gameObject);
            }
            Destroy(gameObject);
        }
    }
}
