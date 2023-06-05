using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class placeable : MonoBehaviour
{
    public Material[] material;
    Renderer rend;

    public GameObject shilouette;
    public GameObject acutalObj;
    public GameObject player;
    public int ID;
    public bool active;
    private bool canPlace;
    private InventoryManager manager;
    public GameObject emptyObject;
    private GameObject activeEmpty;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        activeEmpty = GameObject.Find("Activated Object");
        manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<InventoryManager>();
        rend = shilouette.GetComponent<Renderer>();
        rend.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        Transform camTransform = Camera.main.transform;
        RaycastHit hit;

        if (Physics.Raycast(camTransform.position, camTransform.forward, out hit, 6))
        {
            if(!active)
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

            if (hit.collider.tag == ("Ground"))
            {
                Debug.Log(hit.collider.tag);
                rend.sharedMaterial = material[0];
                canPlace = true;
            }
            else
            {
                Debug.Log("not ground");
                rend.sharedMaterial = material[1];
                canPlace = false;
            }

            if (Input.GetMouseButtonDown(0) && canPlace)
            {
                Instantiate(acutalObj, hit.point, Quaternion.Euler(-90f, camTransform.eulerAngles.y, 180f));
                Destroy(obj.gameObject);
                Destroy(this.transform.gameObject);
                var Object = Instantiate(emptyObject, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
                Object.transform.parent = activeEmpty.transform;
                manager.RemoveItem(ID, 1);
            }
        }
    }
}
