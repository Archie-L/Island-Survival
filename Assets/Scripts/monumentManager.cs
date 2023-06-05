using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class monumentManager : MonoBehaviour
{
    public List<Transform> spawnPoints;
    public List<GameObject> cratePrefabs;
    public float respawnTime = 180f;
    public bool forEnemy;
    public int amount;
    public List<GameObject> spawnedCrates = new List<GameObject>();
    private bool isRespawning = false;

    private void Start()
    {
        SpawnCrates();
    }

    private void Update()
    {
        if (spawnedCrates.Count == 0 && !isRespawning)
        {
            StartCoroutine(RespawnCrates());
        }
    }

    public void SpawnCrates()
    {
        if (forEnemy)
        {
            for (int i = 0; i < amount; i++)
            {
                int randomSpawnPointIndex = Random.Range(0, spawnPoints.Count);
                Transform spawnPoint = spawnPoints[randomSpawnPointIndex];
                GameObject randomCrate = cratePrefabs[Random.Range(0, cratePrefabs.Count)];
                GameObject spawnedCrate = Instantiate(randomCrate, spawnPoint.position, Quaternion.Euler(-90f, 0f, 0f));
                spawnedCrate.transform.parent = spawnPoint;
                spawnedCrates.Add(spawnedCrate);

                Enemy enemyComponent = spawnedCrate.GetComponent<Enemy>();
                if (enemyComponent != null)
                {
                    enemyComponent.points = spawnPoints.ToArray();
                }
            }
        }
        else
        {
            foreach (Transform spawnPoint in spawnPoints)
            {
                GameObject randomCrate = cratePrefabs[Random.Range(0, cratePrefabs.Count)];
                GameObject spawnedCrate = Instantiate(randomCrate, spawnPoint.position, Quaternion.Euler(-90f, 0f, 0f));
                spawnedCrate.transform.parent = spawnPoint;
                spawnedCrates.Add(spawnedCrate);
            }
        }
    }

    private IEnumerator RespawnCrates()
    {
        isRespawning = true;
        yield return new WaitForSeconds(respawnTime);
        SpawnCrates();
        isRespawning = false;
    }
}