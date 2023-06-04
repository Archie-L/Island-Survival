using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardFX : MonoBehaviour
{
    public Transform camTransform;
    private GameObject player;
    public GameObject panel;

    void Update()
    {
        camTransform = Camera.main.transform;
        player = GameObject.FindGameObjectWithTag("Player");

        float distance;
        distance = Vector3.Distance(player.transform.position, transform.position);

        if (distance <= 5)
        {
            panel.SetActive(true);
        }
        else
        {
            panel.SetActive(false);
        }

        transform.LookAt(camTransform);
    }
}