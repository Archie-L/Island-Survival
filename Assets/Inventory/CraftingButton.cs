using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingButton : MonoBehaviour
{
    public Craft recipe;
    public InventoryManager manager;
    public bool needWorkbench;
    private bool close;
    public GameObject player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<InventoryManager>();
    }

    public void CraftItem()
    {
        if (needWorkbench)
        {
            var cloest = FindClosestBench();
            float distance = Vector3.Distance(player.transform.position, cloest.transform.position);
            if (distance <= 3)
            {
                manager.CraftItem(recipe.IDs, recipe.IDsAmounts, recipe.outCome, recipe.outComeAmount);
            }
        }
        else
        {
            manager.CraftItem(recipe.IDs, recipe.IDsAmounts, recipe.outCome, recipe.outComeAmount);
        }
    }
    public GameObject FindClosestBench()
    {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("workBench");
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (GameObject go in gos)
        {
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
        }
        return closest;
    }
}
