using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TerrainManager : MonoBehaviour
{
    [SerializeField] private int terrainWidth = 100;
    [SerializeField] private int terrainLength = 100;
    [SerializeField] private float terrainHeightScale = 50f;
    [SerializeField] private int terrainSeed = 0;
    [SerializeField] private float treeminheight;
    [SerializeField] private float treemaxheight;
    [SerializeField] private GameObject[] trees;
    [SerializeField] private float treescale;
    [SerializeField] private int lowerdensity;

    [SerializeField] private int perlinOctaves = 3;
    [SerializeField] private float perlinFrequency = 0.1f;
    [SerializeField] private float perlinPersistence = 0.5f;
    [SerializeField] private float perlinLacunarity = 2f;
    [SerializeField] private Vector2 perlinScale = Vector2.one;
    [SerializeField] private float movedown = 1f;
    [SerializeField] private float islandStrength = 0f;

    [SerializeField] private Terrain terrain;
    [SerializeField] private TerrainCollider terrainCollider;

    private void Start()
    {
        if (terrain == null)
        {
            terrain = Terrain.activeTerrain;
        }

        terrainSeed = Random.Range(0, 10000);

        GenerateTerrain();
    }

    private void GenerateTerrain()
    {
        TerrainData terrainData = GenerateTerrainData();

        // Retain the terrain layers
        TerrainLayer[] terrainLayers = terrain.terrainData.terrainLayers;
        terrainData.terrainLayers = terrainLayers;

        // Apply the terrain data to the terrain
        Random.InitState(terrainSeed);
        List<GameObject> tres = new List<GameObject>();
        terrain.terrainData = terrainData;
        terrainCollider.terrainData = terrainData;
        for (int x = 0; x < terrainWidth; x++)
        {
            for (int z = 0; z < terrainLength; z++)
            {
                float random = Random.Range(0f, 1f);
                if (random < Mathf.PerlinNoise((x * treescale) + 5600, (z * treescale) + 5600) && Random.Range(1, lowerdensity) == 1)
                {
                    // Instantiate random tree at the position of current point
                    int randomIndex = Random.Range(0, trees.Length);
                    GameObject tree = Instantiate(trees[randomIndex], new Vector3(x, 100, z), Quaternion.identity);
                    tres.Add(tree);
                    tree.transform.localScale *= Random.Range(0.8f, 1.2f); // Scale the tree randomly
                }

            }
        }
        foreach (GameObject t in tres)
        {
            RaycastHit hit;
            if (Physics.Raycast(t.transform.position, -t.transform.up, out hit, 1000))
            {
                t.transform.position = hit.point;
            }
            if (t.transform.position.y < treeminheight || t.transform.position.y >= treemaxheight)
            {
                Destroy(t);
            }
        }
    }


    private TerrainData GenerateTerrainData()
    {
        TerrainData terrainData = new TerrainData();
        terrainData.heightmapResolution = terrainWidth + 1;
        terrainData.size = new Vector3(terrainWidth, terrainHeightScale, terrainLength);
        terrainData.SetDetailResolution(terrainWidth, 8);

        float[,] heightmap = GenerateHeightmap();
        terrainData.SetHeights(0, 0, heightmap);

        return terrainData;
    }

    private float[,] GenerateHeightmap()
    {

        float[,] heightmap = new float[terrainWidth, terrainLength];

        float centerX = (float)terrainWidth / 2f;
        float centerZ = (float)terrainLength / 2f;

        for (int x = 0; x < terrainWidth; x++)
        {
            for (int z = 0; z < terrainLength; z++)
            {
                float distanceFromCenter = Vector2.Distance(new Vector2(x, z), new Vector2(centerX, centerZ));
                float distanceStrength = Mathf.Clamp01(1f - (distanceFromCenter / centerX));

                float height = 0f;

                // Perlin noise
                for (int i = 0; i < perlinOctaves; i++)
                {
                    float frequency = perlinFrequency * Mathf.Pow(perlinLacunarity, i);
                    float amplitude = Mathf.Pow(perlinPersistence, i);
                    height += amplitude * Mathf.PerlinNoise((x + terrainSeed) * frequency * perlinScale.x, (z + terrainSeed) * frequency * perlinScale.y);
                }

                height += movedown;

                // Apply the distance strength to the height
                height *= distanceStrength;

                heightmap[x, z] = height;
            }
        }



        return heightmap;
    }
}