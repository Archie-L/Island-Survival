using System.Runtime.Serialization;
using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public NavMeshAgent agent;
    public float range;
    public float health;
    public Transform centrePoint;
    public Animator anim;
    public GameObject player;
    public bool patrolling, follow;
    public Transform[] points;
    private int destPoint = 0;
    public bool isBoss;
    private LookAt look;
    public bool shooting;
    public int damage;

    public float delay = 3f; // Delay before the first shot
    public float shootingInterval = 1f; // Interval between shots
    public int maxShots = 5; // Maximum number of shots

    private float nextShotTime;
    private int shotsFired;

    public LayerMask ignore;
    private monumentManager monu;

    private void Start()
    {
        look = GetComponentInChildren<LookAt>();
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
        monu = GetComponentInParent<monumentManager>();
        agent.stoppingDistance = 7;

        if (patrolling)
        {
            agent.autoBraking = false;
            patrol();
        }

        // Set the time for the first shot
        nextShotTime = Time.time + delay;
    }

    private void Update()
    {
        CheckDist();

        if (health <= 0)
        {
            monu.spawnedCrates.Remove(this.gameObject);
            Destroy(this.gameObject);
        }

        if (patrolling && !follow)
        {
            if (!agent.pathPending && agent.remainingDistance < 0.5f)
                patrol();
        }
        else if (follow && !patrolling)
        {
            FollowingPlayer();
        }
        else if (!patrolling && !follow)
        {
            anim.SetBool("walking", true);

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
    }

    public void FollowingPlayer()
    {
        if (agent.remainingDistance <= 7)
            anim.SetBool("walking", false);
        if (agent.remainingDistance > 7)
            anim.SetBool("walking", true);

        if (isBoss)
        {
            look.enabled = true;
        }

        if (Time.time >= nextShotTime)
        {
            if(isBoss)
            {
                anim.SetBool("walking", false);
            }

            if (shotsFired < maxShots)
            {
                if (!isBoss)
                {
                    anim.SetTrigger("shoot");
                    agent.speed = 0f;
                    Invoke("Shoot", 0.5f);
                }
                else if (isBoss)
                {
                    Shoot();
                }
                shotsFired++;

                if (shotsFired >= maxShots)
                {
                    // Reset shotsFired and set the time for the next round of shots
                    shotsFired = 0;
                    nextShotTime = Time.time + delay;
                    agent.speed = 2f;
                }
                else
                {
                    // Set the time for the next shot within the current round
                    nextShotTime = Time.time + shootingInterval;
                }
            }
        }

        agent.SetDestination(player.transform.position);
    }

    private void patrol()
    {
        agent.stoppingDistance = 0;

        if (isBoss)
        {
            look.enabled = false;
        }

        anim.SetBool("walking", true);

        if (points.Length == 0)
            return;

        agent.destination = points[destPoint].position;

        destPoint = (destPoint + 1) % points.Length;
    }

    private bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        if (isBoss)
        {
            look.enabled = false;
        }

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

    private void CheckDist()
    {
        float distance = Vector3.Distance(this.transform.position, player.transform.position);

        if (distance <= 15 && !follow)
        {
            agent.stoppingDistance = 7;

            follow = true;
            patrolling = false;

            if (isBoss)
            {
                look.enabled = true;
            }
        }
        if (distance > 100 && follow)
        {
            agent.stoppingDistance = 0;

            patrolling = true;
            follow = false;

            if (isBoss)
            {
                look.enabled = false;
            }
        }
    }

    public GameObject muzzleFlash;

    private void Shoot()
    {
        Ray ray = new Ray(transform.position, (player.transform.position - transform.position).normalized);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            StartCoroutine(ShowMuzzleFlash());

            if (hit.collider.tag == "Player")
            {
                Debug.Log("Enemy shot at the player!");

                hit.collider.GetComponentInParent<PlayerMovement>().TakeDamage(damage);
            }
        }
    }

    private IEnumerator ShowMuzzleFlash()
    {
        muzzleFlash.SetActive(true);

        yield return new WaitForSeconds(0.05f);

        muzzleFlash.SetActive(false);
    }
}
