using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class tree : MonoBehaviour
{
    public float health;
    public GameObject log;
    public GameObject soundManager;
    private Slider hpBar;
    public string type;
    public AudioClip rockSFX, treeSFX;

    // Start is called before the first frame update
    void Start()
    {
        hpBar = gameObject.GetComponentInChildren<Slider>();
        soundManager = GameObject.FindGameObjectWithTag("SFX");
    }

    private float lastHp;

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
        {
            BreakTree();
        }
        if(lastHp != health) hpBar.value = health;

        lastHp = health;
    }

    void BreakTree()
    {
        if(type == "rock")
        {
            soundManager.GetComponent<AudioSource>().clip = rockSFX;
        }
        else if (type == "tree")
        {
            soundManager.GetComponent<AudioSource>().clip = treeSFX;
        }

        soundManager.GetComponent<AudioSource>().Play();
        Instantiate(log, transform.position, transform.rotation);
        Destroy(this.gameObject);
    }
}