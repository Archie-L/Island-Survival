using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour
{
    public Transform target;
    public Vector3 customRotation;

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(target);
        transform.rotation *= Quaternion.Euler(customRotation);
    }
}
