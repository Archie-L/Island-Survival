using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gun : MonoBehaviour
{
    public Transform barrel;
    public Animator anim;
    public float dmg;
    private float damage;
    private float multiplier;
    public bool shot = false;
    private bool hasAmmo;
    public int whatAmmoID;
    public int ammoTake;
    private InventoryManager manager;

    private void Start()
    {
        resetDmg();
        manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<InventoryManager>();
    }

    private void Update()
    {
        hasAmmo = manager.HasItem(whatAmmoID);

        if (Input.GetMouseButtonDown(0) && !shot && hasAmmo)
        {
            anim.SetTrigger("shoot");
            ShootGun();
        }
    }

    void ShootGun()
    {
        manager.RemoveItem(whatAmmoID, ammoTake);

        Transform camTransform = Camera.main.transform;
        RaycastHit hit;

        if (Physics.Raycast(camTransform.position, camTransform.forward, out hit, 50))
        {
            bool hitThing = false;

            if (hit.collider.GetComponent<BodyPart>().part == "head")
            {
                multiplier = 2;
                Debug.Log("head");
                hitThing = true;
            }
            else if (hit.collider.GetComponent<BodyPart>().part == "torso")
            {
                multiplier = 1;
                Debug.Log("torso");
                hitThing = true;
            }
            else if (hit.collider.GetComponent<BodyPart>().part == "arm/leg")
            {
                multiplier = 0.5f;
                Debug.Log("arm/leg");
                hitThing = true;
            }

            if (hitThing)
            {
                damage = Random.Range(damage - 5, damage + 5) * multiplier;
                hit.collider.transform.root.GetComponent<Enemy>().health -= damage;
            }

            resetDmg();
        }
    }

    void resetDmg()
    {
        damage = dmg;
    }
}
