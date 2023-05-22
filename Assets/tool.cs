using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tool : MonoBehaviour
{
    public Animator anim;
    public string type;
    public bool isAxe;
    private float damage;
    private float multiplier;
    public bool hasSwung;
    public GameObject soundManager;
    public AudioClip rockSFX, treeSFX;

    private void Start()
    {
        soundManager = GameObject.FindGameObjectWithTag("SFX");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !hasSwung)
        {
            anim.SetTrigger("swing");
            Invoke("ShootRay", 0.2f);
        }
    }

    void ShootRay()
    {
        for (int i = 0; i < 1; i++)
        {
            Transform camTransform = Camera.main.transform;
            RaycastHit hit;

            if (Physics.Raycast(camTransform.position, camTransform.forward, out hit, 2))
            {
                if (hit.collider.tag == ("stone"))
                {
                    if (!isAxe)
                    {
                        if (type == "stone")
                        {
                            multiplier = 1;
                        }
                        if (type == "metal")
                        {
                            multiplier = 2;
                        }
                        if (type == "rock")
                        {
                            multiplier = 0.5f;
                        }
                    }
                    if (isAxe)
                    {
                        multiplier = 0.1f;
                    }

                    soundManager.GetComponent<AudioSource>().clip = rockSFX;
                    soundManager.GetComponent<AudioSource>().Play();

                    damage = Random.Range(5, 15) * multiplier;
                    hit.collider.GetComponent<tree>().health -= damage;
                }
                if (hit.collider.tag == ("wood"))
                {
                    if (isAxe)
                    {
                        if (type == "stone")
                        {
                            multiplier = 1;
                        }
                        if (type == "metal")
                        {
                            multiplier = 2;
                        }
                    }
                    if (!isAxe)
                    {
                        if (type == "rock")
                        {
                            multiplier = 0.5f;
                        }
                        else
                        {
                            multiplier = 0.1f;
                        }
                    }

                    soundManager.GetComponent<AudioSource>().clip = treeSFX;
                    soundManager.GetComponent<AudioSource>().Play();

                    damage = Random.Range(5, 15) * multiplier;
                    hit.collider.GetComponent<tree>().health -= damage;
                }
            }
        }
    }
}
