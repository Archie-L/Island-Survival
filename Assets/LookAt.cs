using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour
{
    public Transform target;

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(target);
        transform.rotation *= Quaternion.Euler(0, -90, -90);
    }
}
