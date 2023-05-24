using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gun : MonoBehaviour
{
    public AudioSource shootSound;
    public AudioClip emptySFX, shootSFX;
    public Animator anim;
    public float dmg;
    private float damage;
    private float multiplier;
    public bool shot = false;
    public bool hasAmmo;
    public int whatAmmoID;
    public int ammoTake;
    private InventoryManager manager;
    public float wait;

    private void Start()
    {
        resetDmg();
        shootSound = GetComponent<AudioSource>();
        manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<InventoryManager>();
    }

    private void Update()
    {
        hasAmmo = manager.HasItem(whatAmmoID);

        if (Input.GetMouseButtonDown(0) && !shot)
        {
            if (hasAmmo)
            {
                anim.SetTrigger("shoot");
                shootSound.clip = shootSFX;
                shootSound.Play(0);
                Invoke("ShootGun", wait);
            }
            if (!hasAmmo)
            {
                anim.SetTrigger("empty");
                shootSound.clip = emptySFX;
                shootSound.Play(0);
            }
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
            bool hitAnimal = false;

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
            else if (hit.collider.GetComponent<BodyPart>().part == "animal")
            {
                multiplier = 1f;
                hitThing = true;
                hitAnimal = true;
            }

            if (hitThing)
            {
                damage = Random.Range(damage - 5, damage + 5) * multiplier;
                if (hitAnimal)
                {
                    hit.collider.transform.root.GetComponent<Animal>().health -= damage;
                }
                else
                {
                    hit.collider.transform.root.GetComponent<Enemy>().health -= damage;
                }
            }

            resetDmg();
        }
    }

    void resetDmg()
    {
        damage = dmg;
    }
}
