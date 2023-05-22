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

    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<InventoryManager>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        hasAmmo = manager.HasItem(whatAmmoID);

        if (Input.GetMouseButtonDown(0) && !inUse)
        {
            if (hasAmmo)
            {
                anim.SetTrigger("use");
                manager.RemoveItem(whatAmmoID, ammoTake);

                if (forHP)
                {
                    player.Health += increasehealth;
                }
                else if(forFood)
                {
                    player.Water += increasewater;
                    player.Hunger += increasefood;
                }
                else if(forWater)
                {
                    player.Water += increasewater;
                }
            }
        }
    }
}
