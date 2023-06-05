using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BandagesScript : MonoBehaviour
{
    public bool hasAmmo;
    public Animator anim;
    public int whatAmmoID;
    public int ammoTake;
    private InventoryManager manager;
    private PlayerMovement player;
    public bool inUse;

    [Header("Stuff")]
    public bool forHP;
    public int increasehealth;
    public bool forFood;
    public int increasefood;
    public bool forWater;
    public int increasewater;

    public GameObject emptyObject;
    private GameObject activeEmpty;
    private GameObject thisObj;

    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<InventoryManager>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        activeEmpty = GameObject.Find("Activated Object");
        thisObj = this.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        hasAmmo = manager.HasItem(whatAmmoID);

        if (!hasAmmo)
        {
            var Object = Instantiate(emptyObject, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
            Object.transform.parent = activeEmpty.transform;
            Destroy(thisObj);
        }

        if (Input.GetMouseButtonDown(0) && !inUse && hasAmmo)
        {
            anim.SetTrigger("use");
            manager.RemoveItem(whatAmmoID, ammoTake);

            if (forHP)
            {
                player.Health += increasehealth;
            }
            else if (forFood)
            {
                player.Water += increasewater;
                player.Hunger += increasefood;
            }
            else if (forWater)
            {
                player.Water += increasewater;
            }
        }
    }
}
