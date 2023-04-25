using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itemPickUp : MonoBehaviour
{
    public int Amount;
    public InventoryManager manager;
    public GameObject item;

    private void Start()
    {
        Amount = Random.Range(5, 15);
        manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<InventoryManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            Debug.Log("hit");
            AddToInv();
        }
    }

    private void AddToInv()
    {
        manager.AddItem(item);
        item.GetComponent<InventoryItem>().amount = Amount;
        Destroy(this.gameObject);
    }
}