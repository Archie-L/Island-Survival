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
        if(this.gameObject.tag == ("stone"))
        {
            Instantiate(log, new Vector3(10, 1, 0), Quaternion.identity);
        }
        if (this.gameObject.tag == ("wood"))
        {
            Instantiate(log, new Vector3(0, 5, 0), Quaternion.identity);
        }
        Destroy(this.gameObject);
    }
}
