using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;

public class TerrainManager : MonoBehaviour
{
    public GameObject player;
    public GameObject playerCam;
    public GameObject loadingCam;
    public PlayerMovement playerMove;

    [Header("Trees")]
    [SerializeField] private int terrainWidth = 100;
    [SerializeField] private int terrainLength = 100;
    [SerializeField] private float terrainHeightScale = 50f;
    [SerializeField] private int terrainSeed = 0;
    [SerializeField] private float treeminheight;
    [SerializeField] private float treemaxheight;
    [SerializeField] private GameObject[] trees;
    [SerializeField] private Transform treeParent;
    [SerializeField] private float treescale;
    [SerializeField] private int lowerdensity;

    [Header("Terrain")]
    [SerializeField] private int perlinOctaves = 3;
    [SerializeField] private float perlinFrequency = 0.1f;
    [SerializeField] private float perlinPersistence = 0.5f;
    [SerializeField] private float perlinLacunarity = 2f;
    [SerializeField] private Vector2 perlinScale = Vector2.one;
    [SerializeField] private float movedown = 1f;
    [SerializeField] private float islandStrength = 0f;

    [SerializeField] private Terrain terrain;
    [SerializeField] private TerrainCollider terrainCollider;

    [Header("Monuments")]
    [SerializeField] private float monumentminheight;
    [SerializeField] private float monumentmaxheight;
    [SerializeField] private GameObject[] monuments;
    [SerializeField] private Transform monumentParent;
    [SerializeField] private float monumentscale;
    [SerializeField] private int monumentlowerdensity;

    private void Start()
    {
        if (terrain == null)
        {
            terrain = Terrain.activeTerrain;
        }

        terrainSeed = Random.Range(0, 10000);

        gameManager.instance.removeEventSys();

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
        terrain.terrainData = terrainData;
        terrainCollider.terrainData = terrainData;

        GenerateMonuments();
        Invoke("GenerateTree", 1);
        Invoke("GenerateNavMesh", 2);
    }

    private IEnumerator spawnPlayer(int waitTime)
    {
        new WaitForSeconds(waitTime);
        gameManager.instance.closeScreen();
        player.gameObject.SetActive(true);
        playerCam.gameObject.SetActive(true);
        loadingCam.gameObject.SetActive(false);
        playerMove.enabled = true;
        yield break;
    }

    public void GenerateMonuments()
    {
        List<GameObject> monus = new List<GameObject>();

        for (int x = 0; x < terrainWidth; x++)
        {
            for (int z = 0; z < terrainLength; z++)
            {
                float random = Random.Range(0f, 1f);
                if (random < Mathf.PerlinNoise((x * monumentscale) + 5600, (z * monumentscale) + 5600) && Random.Range(1, monumentlowerdensity) == 1)
                {
                    // Instantiate random tree at the position of current point
                    int randomIndex = Random.Range(0, monuments.Length);
                    int randRot = Random.Range(0, 359);
                    GameObject monu = Instantiate(monuments[randomIndex], new Vector3(x, 100, z), Quaternion.Euler(0f, randRot, 0f));
                    monus.Add(monu);
                    monu.transform.localScale *= Random.Range(0.8f, 1.2f); // Scale the tree randomly
                }

            }
        }
        foreach (GameObject m in monus)
        {
            RaycastHit hit;
            if (Physics.Raycast(m.transform.position, -m.transform.up, out hit, 1000))
            {
                m.transform.position = hit.point;
                m.transform.parent = monumentParent;
                m.GetComponentInChildren<MatchTerrainToColliders>().BringTerrainToUndersideOfCollider();
            }
            if (m.transform.position.y < monumentminheight || m.transform.position.y >= monumentmaxheight || hit.collider.tag != "Ground")
            {
                Destroy(m);
            }
        }
    }

    public NavMeshSurface surfaces;

    public void GenerateNavMesh()
    {
        surfaces.BuildNavMesh();
        StartCoroutine(spawnPlayer(1));
    }

    public void GenerateTree()
    {
        List<GameObject> tres = new List<GameObject>();

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
                t.transform.parent = treeParent;
            }
            if (t.transform.position.y < treeminheight || t.transform.position.y >= treemaxheight || hit.collider.tag != ("Ground"))
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