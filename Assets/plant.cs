using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class plant : MonoBehaviour
{
    public int Amount;
    public InventoryManager manager;
    public GameObject item;
    public GameObject txt;

    private void Start()
    {
        Amount = Random.Range(1, 3);
        manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<InventoryManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            txt.SetActive(true);

            if (Input.GetKeyDown(KeyCode.E))
            {
                AddToInv();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            txt.SetActive(false);
        }
    }
    private void AddToInv()
    {
        manager.AddItem(item);
        item.GetComponent<InventoryItem>().amount = Amount;
        Destroy(this.gameObject);
    }
}
