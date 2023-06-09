using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Animal : MonoBehaviour
{
    public NavMeshAgent agent;
    public float range;
    public float health;
    public GameObject Neck;
    public GameObject meat;
    public Transform centrePoint;

    void Start()
    {
        float evilCow;
        agent = GetComponent<NavMeshAgent>();

        evilCow = Random.Range(1, 100);

        if(evilCow >= 90)
        {
            Neck.GetComponent<LookAt>().enabled = true;
        }
    }

    void Update()
    {
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            Vector3 point;
            if (RandomPoint(centrePoint.position, range, out point))
            {
                Debug.DrawRay(point, Vector3.up, Color.blue, 1.0f);
                agent.SetDestination(point);
            }
        }

        if(health <= 0)
        {
            Instantiate(meat, transform.position, transform.rotation);
            Destroy(this.gameObject);
        }
    }

    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {

        Vector3 randomPoint = center + Random.insideUnitSphere * range;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
        {
            result = hit.position;
            return true;
        }

        result = Vector3.zero;
        return false;
    }
}