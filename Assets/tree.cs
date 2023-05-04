using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class tree : MonoBehaviour
{
    public float health;
    public GameObject log;
    private Slider hpBar;

    // Start is called before the first frame update
    void Start()
    {
        hpBar = gameObject.GetComponentInChildren<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
        {
            BreakTree();
        }
        hpBar.value = health;
    }

    void BreakTree()
    {
        Instantiate(log, transform.position, transform.rotation);
        Destroy(this.gameObject);
    }
}
