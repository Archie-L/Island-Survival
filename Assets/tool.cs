using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tool : MonoBehaviour
{
    public Animator anim;
    public List<string> type;
    public bool isAxe;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
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
                    Debug.Log("poopoo");
                }
                if (hit.collider.tag == ("wood"))
                {
                    Debug.Log("peepee");
                }
            }
        }
    }
}
