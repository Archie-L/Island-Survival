using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tree : MonoBehaviour
{
    public float health;
    public GameObject log;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
        {
            BreakTree();
        }
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
