using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class placeable : MonoBehaviour
{
    public GameObject shilouette;
    public GameObject acutalObj;
    public GameObject player;
    public bool active;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        Transform camTransform = Camera.main.transform;
        RaycastHit hit;

        if (Physics.Raycast(camTransform.position, camTransform.forward, out hit, 6))
        {
            if (hit.collider.tag == ("Ground") && !active)
            {
                var clone = Instantiate(shilouette, hit.point, Quaternion.Euler(-90f, camTransform.eulerAngles.y, 180f));
                clone.transform.parent = this.transform;
            }
        }

        if (this.transform.childCount == 1)
        {
            active = true;
            var obj = this.transform.GetChild(0);
            obj.transform.position = hit.point;
            obj.transform.rotation = Quaternion.Euler(-90, camTransform.eulerAngles.y, 180);

            if (Input.GetMouseButtonDown(0))
            {
                Instantiate(acutalObj, hit.point, Quaternion.Euler(-90f, camTransform.eulerAngles.y, 180f));
                Destroy(obj.gameObject);
                Destroy(this.transform.parent.gameObject);
            }
        }
    }
}
