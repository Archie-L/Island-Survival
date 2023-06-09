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
        Amount = Random.Range(5, 10);
        manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<InventoryManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.transform.parent.tag == "Player")
        {
            Debug.Log("hit");
            AddToInv();
        }
    }

    private void AddToInv()
    {
        manager.AddItem(item, Amount);
        Destroy(this.gameObject);
    }
}