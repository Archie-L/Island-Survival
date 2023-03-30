using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tree : MonoBehaviour
{
    public float health;
    public GameObject log, treeModel;
    public bool broken = false;

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
        if (!broken)
        {
            Instantiate(log, new Vector3(0, 0, 0), Quaternion.identity);
            Destroy(treeModel);
            broken = true;
        }
    }
}
