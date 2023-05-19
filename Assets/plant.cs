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
        Transform camTransform = Camera.main.transform;
        RaycastHit hit;

        if (Physics.Raycast(camTransform.position, camTransform.forward, out hit, 2))
        {
            if (hit.collider.tag == ("plant"))
            {
                txt.SetActive(true);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    AddToInv();
                }
            }
            else
            {
                txt.SetActive(false);
            }
        }
    }

    private void AddToInv()
    {
        manager.AddItem(item, Amount);
        Destroy(this.gameObject);
    }
}
