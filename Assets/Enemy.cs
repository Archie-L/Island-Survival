using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public NavMeshAgent agent;
    public float range;
    public float health;
    public Transform centrePoint;
    public Animator anim;
    public Transform player;
    public bool patrolling, follow;
    public Transform[] points;
    private int destPoint = 0;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        if (patrolling)
        {
            agent.autoBraking = false;
            patrol();
        }
    }

    void Update()
    {
        if (patrolling && !follow)
        {
            if (!agent.pathPending && agent.remainingDistance < 0.5f)
                patrol();
        }
        else if(follow && !patrolling)
        {
            agent.SetDestination(player.position);

            if (agent.remainingDistance <= 7)
                anim.SetBool("walking", false);
            if (agent.remainingDistance > 7)
                anim.SetBool("walking", true);

        }
        else if (!patrolling && !follow)
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
        }

        if (health <= 1)
        {
            Destroy(this.gameObject);
        }
    }

    void patrol()
    {
        if (points.Length == 0)
            return;

        agent.destination = points[destPoint].position;

        destPoint = (destPoint + 1) % points.Length;
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
