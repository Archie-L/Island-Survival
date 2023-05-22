using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public int maxEnemies = 20;
    public float respawnDelay = 2f;

    private int currentEnemies = 0;
    private Transform terrainTransform;

    private void Start()
    {
        terrainTransform = Terrain.activeTerrain.transform;
        SpawnEnemies(maxEnemies);
    }

    private void Update()
    {
        if (currentEnemies < maxEnemies)
        {
            StartCoroutine(RespawnEnemy(respawnDelay));
        }
    }

    private void SpawnEnemies(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 randomPosition = GenerateRandomPosition();
            GameObject enemy = Instantiate(enemyPrefab, randomPosition, Quaternion.identity);
            enemy.transform.SetParent(transform);
            currentEnemies++;
        }
    }

    private Vector3 GenerateRandomPosition()
    {
        float terrainWidth = terrainTransform.localScale.x * Terrain.activeTerrain.terrainData.size.x;
        float terrainLength = terrainTransform.localScale.z * Terrain.activeTerrain.terrainData.size.z;
        float x = Random.Range(0f, terrainWidth);
        float z = Random.Range(0f, terrainLength);
        float y = Terrain.activeTerrain.SampleHeight(new Vector3(x, 0f, z)) + terrainTransform.position.y;

        return new Vector3(x, y, z);
    }

    private System.Collections.IEnumerator RespawnEnemy(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (currentEnemies < maxEnemies)
        {
            Vector3 randomPosition = GenerateRandomPosition();
            GameObject enemy = Instantiate(enemyPrefab, randomPosition, Quaternion.identity);
            enemy.transform.SetParent(transform);
            currentEnemies++;
        }
    }

    public void EnemyDied()
    {
        currentEnemies--;
    }
}

