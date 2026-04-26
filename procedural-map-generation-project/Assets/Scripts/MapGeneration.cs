using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class MapGeneration : MonoBehaviour
{
    [SerializeField] NavMeshSurface navSurface;

    Cell[,] grid;
    float[,] noiseMap;
    [Header("----- Map -----")]
    [Range(1, 128)]
    [SerializeField] int mapSize;
    [SerializeField] float noiseMap_Scale;
    [Range(0f, 1f)]
    [SerializeField] float water_Thrashold;
    [Range(0f, 1f)]
    [SerializeField] float sand_Thrashold;
    [SerializeField] Material grass_Mat;
    [SerializeField] Material sand_Mat;
    [Header("----- Decorate -----")]
    [SerializeField] float decorateNoiseScale;
    [SerializeField] Decorate[] decoratePrefab;
    [SerializeField] float decorate_density;


    private void Start()
    {
        StartCoroutine(TestGenerateMapWithNav());
    }

    IEnumerator TestGenerateMapWithNav()
    {
        Debug.Log("Generate Nav");
        navSurface.BuildNavMesh();
        yield return new WaitForSeconds(5);
        GenerateMap();
        Debug.Log("Generate Map Visual");
        navSurface.UpdateNavMesh(navSurface.navMeshData);
    }

    public void GenerateMap()
    {
        ClearMap();
        GenerateCell();
        GenerateMapVisual();
        GenerateDecoration();
    }

    void ClearMap()
    {
        if (transform.childCount > 0)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform cell = transform.GetChild(i);
                Destroy(cell.gameObject);
            }
        }
    }

    void GenerateCell()
    {
        grid = new Cell[mapSize, mapSize];
        noiseMap = new float[mapSize, mapSize];
        float[,] falloffMap = new float[mapSize, mapSize];
        float xOffset = UnityEngine.Random.Range(-10000f, 10000f);
        float yOffset = UnityEngine.Random.Range(-10000f, 10000f);

        for (int y = 0; y < mapSize; y++)
        {
            for (int x = 0; x < mapSize; x++)
            {
                float xv = x / (float)mapSize * 2 - 1;
                float yv = y / (float)mapSize * 2 - 1;
                float value = Mathf.Sqrt((xv * xv) + (yv * yv));
                float v = Mathf.Min(value, 1);
                falloffMap[x, y] = Mathf.Pow(v, 3f) / (Mathf.Pow(v, 3f) + Mathf.Pow(2.2f - 2.2f * v, 3f));

                float noiseValue = Mathf.PerlinNoise(x * noiseMap_Scale + xOffset, y * noiseMap_Scale + yOffset);
                float noisefallOff = noiseValue - falloffMap[x, y];
                noiseMap[x, y] = noisefallOff;

                bool isWater = noiseMap[x, y] <= water_Thrashold;
                grid[x, y] = new Cell(isWater);
                grid[x, y].noiseValue = noiseMap[x, y];
            }
        }
    }

    void GenerateMapVisual()
    {
        Mesh sandMesh = new Mesh();
        List<Vector3> sandVertices = new List<Vector3>();
        List<int> sandTriangles = new List<int>();

        Mesh grassMesh = new Mesh();
        List<Vector3> grassVertices = new List<Vector3>();
        List<int> grassTriangles = new List<int>();

        Mesh edgeMesh = new Mesh();
        List<Vector3> edgeVertices = new List<Vector3>();
        List<int> edgeTriangles = new List<int>();

        for (int y = 0; y < mapSize; y++)
        {
            for (int x = 0; x < mapSize; x++)
            {
                Cell cell = grid[x, y];
                int yPosInt = Mathf.FloorToInt((cell.noiseValue - water_Thrashold) * 10);
                Vector3 cellPos = new Vector3(x, yPosInt, y);
                if (!cell.isWater)
                {
                    if (noiseMap[x, y] <= sand_Thrashold)
                    {
                        Vector3 a = cellPos + new Vector3(-0.5f, 0, 0.5f);
                        Vector3 b = cellPos + new Vector3(0.5f, 0, 0.5f);
                        Vector3 c = cellPos + new Vector3(-0.5f, 0, -0.5f);
                        Vector3 d = cellPos + new Vector3(0.5f, 0, -0.5f);
                        Vector3[] v = new Vector3[] { a, b, c, c, b, d };
                        for (int k = 0; k < 6; k++)
                        {
                            sandVertices.Add(v[k]);
                            sandTriangles.Add(sandTriangles.Count);
                        }
                    }
                    else
                    {
                        Vector3 a = cellPos + new Vector3(-0.5f, 0, 0.5f);
                        Vector3 b = cellPos + new Vector3(0.5f, 0, 0.5f);
                        Vector3 c = cellPos + new Vector3(-0.5f, 0, -0.5f);
                        Vector3 d = cellPos + new Vector3(0.5f, 0, -0.5f);
                        Vector3[] v = new Vector3[] { a, b, c, c, b, d };
                        for (int k = 0; k < 6; k++)
                        {
                            grassVertices.Add(v[k]);
                            grassTriangles.Add(grassTriangles.Count);
                        }
                    }

                    if (x > 0)
                    {
                        Cell left = grid[x - 1, y];
                        int leftYPosInt = Mathf.FloorToInt((left.noiseValue - water_Thrashold) * 10);
                        if (left.isWater || leftYPosInt < yPosInt)
                        {
                            Vector3 a = cellPos + new Vector3(-0.5f, 0, 0.5f);
                            Vector3 b = cellPos + new Vector3(-0.5f, 0, -0.5f);
                            Vector3 c = cellPos + new Vector3(-0.5f, -10, 0.5f);
                            Vector3 d = cellPos + new Vector3(-0.5f, -10, -0.5f);
                            Vector3[] v = new Vector3[] { a, b, c, c, b, d };
                            for (int k = 0; k < 6; k++)
                            {
                                edgeVertices.Add(v[k]);
                                edgeTriangles.Add(edgeTriangles.Count);
                            }
                        }
                    }

                    if (x < mapSize - 1)
                    {
                        Cell right = grid[x + 1, y];
                        int rightYPosInt = Mathf.FloorToInt((right.noiseValue - water_Thrashold) * 10);
                        if (right.isWater || rightYPosInt < yPosInt)
                        {
                            Vector3 a = cellPos + new Vector3(0.5f, 0, 0.5f);
                            Vector3 b = cellPos + new Vector3(0.5f, 0, -0.5f);
                            Vector3 c = cellPos + new Vector3(0.5f, -10, 0.5f);
                            Vector3 d = cellPos + new Vector3(0.5f, -10, -0.5f);
                            Vector3[] v = new Vector3[] { a, b, c, c, b, d };
                            for (int k = 0; k < 6; k++)
                            {
                                edgeVertices.Add(v[k]);
                                edgeTriangles.Add(edgeTriangles.Count);
                            }
                        }
                    }

                    if (y > 0)
                    {
                        Cell down = grid[x, y - 1];
                        int downYPosInt = Mathf.FloorToInt((down.noiseValue - water_Thrashold) * 10);
                        if (down.isWater || downYPosInt < yPosInt)
                        {
                            Vector3 a = cellPos + new Vector3(-0.5f, 0, -0.5f);
                            Vector3 b = cellPos + new Vector3(0.5f, 0, -0.5f);
                            Vector3 c = cellPos + new Vector3(-0.5f, -10, -0.5f);
                            Vector3 d = cellPos + new Vector3(0.5f, -10, -0.5f);
                            Vector3[] v = new Vector3[] { a, b, c, c, b, d };
                            for (int k = 0; k < 6; k++)
                            {
                                edgeVertices.Add(v[k]);
                                edgeTriangles.Add(edgeTriangles.Count);
                            }
                        }
                    }

                    if (y < mapSize - 1)
                    {
                        Cell up = grid[x, y + 1];
                        int upYPosInt = Mathf.FloorToInt((up.noiseValue - water_Thrashold) * 10);
                        if (up.isWater || upYPosInt < yPosInt)
                        {
                            Vector3 a = cellPos + new Vector3(-0.5f, 0, 0.5f);
                            Vector3 b = cellPos + new Vector3(0.5f, 0, 0.5f);
                            Vector3 c = cellPos + new Vector3(-0.5f, -10, 0.5f);
                            Vector3 d = cellPos + new Vector3(0.5f, -10, 0.5f);
                            Vector3[] v = new Vector3[] { a, b, c, c, b, d };
                            for (int k = 0; k < 6; k++)
                            {
                                edgeVertices.Add(v[k]);
                                edgeTriangles.Add(edgeTriangles.Count);
                            }
                        }
                    }

                }
            }
        }

        sandMesh.vertices = sandVertices.ToArray();
        sandMesh.triangles = sandTriangles.ToArray();
        sandMesh.RecalculateNormals();

        grassMesh.vertices = grassVertices.ToArray();
        grassMesh.triangles = grassTriangles.ToArray();
        grassMesh.RecalculateNormals();

        edgeMesh.vertices = edgeVertices.ToArray();
        edgeMesh.triangles = edgeTriangles.ToArray();
        edgeMesh.RecalculateNormals();

        GameObject sand = new GameObject("Sand");
        sand.transform.parent = transform;
        MeshFilter sandMeshFilter = sand.AddComponent<MeshFilter>();
        sandMeshFilter.mesh = sandMesh;
        MeshRenderer sandMeshRen = sand.AddComponent<MeshRenderer>();
        sandMeshRen.sharedMaterial = sand_Mat;
        MeshCollider sandCol = sand.AddComponent<MeshCollider>();
        sandCol.sharedMesh = sandMesh;
        sand.layer = 6;

        GameObject grass = new GameObject("Grass");
        grass.transform.parent = transform;
        MeshFilter grassMeshFilter = grass.AddComponent<MeshFilter>();
        grassMeshFilter.mesh = grassMesh;
        MeshRenderer grassMeshRenderer = grass.AddComponent<MeshRenderer>();
        grassMeshRenderer.sharedMaterial = grass_Mat;
        MeshCollider grassCol = grass.AddComponent<MeshCollider>();
        grassCol.sharedMesh = grassMesh;
        grass.layer = 6;

        GameObject edge = new GameObject("Edge");
        edge.transform.parent = transform;
        MeshFilter edgeMeshFilter = edge.AddComponent<MeshFilter>();
        edgeMeshFilter.mesh = edgeMesh;
        MeshRenderer edgeRen = edge.AddComponent<MeshRenderer>();
        edgeRen.sharedMaterial = sand_Mat;
        MeshCollider edgeCol = edge.AddComponent<MeshCollider>();
        edgeCol.sharedMesh = edgeMesh;
        edge.layer = 6;
    }

    void GenerateDecoration()
    {
        float[,] decorateNoiseMap = new float[mapSize, mapSize];
        float xOffset = UnityEngine.Random.Range(-10000f, 10000f);
        float yOffset = UnityEngine.Random.Range(-10000f, 10000f);

        for (int y = 0; y < mapSize; y++)
        {
            for (int x = 0; x < mapSize; x++)
            {
                float noiseValue = Mathf.PerlinNoise(x * decorateNoiseScale + xOffset, y * decorateNoiseScale + yOffset);
                decorateNoiseMap[x, y] = noiseValue;
            }
        }

        for (int y = 0; y < mapSize; y++)
        {
            for (int x = 0; x < mapSize; x++)
            {
                Cell cell = grid[x, y];
                if (!cell.isWater)
                {
                    float v = UnityEngine.Random.Range(0, decorate_density);
                    if (decorateNoiseMap[x, y] < v)
                    {
                        GameObject prefab = GetPrefabFormDecorate();
                        if (prefab != null)
                        {
                            int yPosInt = Mathf.FloorToInt((cell.noiseValue - water_Thrashold) * 10);
                            Vector3 pos = new Vector3(x, yPosInt, y);
                            GameObject go = Instantiate(prefab, pos, Quaternion.identity);
                            go.transform.SetParent(transform, true);
                            go.transform.rotation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 360f), 0);
                            go.transform.localScale = Vector3.one * UnityEngine.Random.Range(0.8f, 1.2f);
                        }
                    }
                }
            }
        }

    }

    GameObject GetPrefabFormDecorate()
    {
        float chanceSum = 0;
        for (int i = 0; i < decoratePrefab.Length; i++)
        {
            chanceSum += decoratePrefab[i].dropRate;
        }

        float rand = UnityEngine.Random.Range(0, chanceSum);
        float runningRate = 0;
        for (int i = 0; i < decoratePrefab.Length; i++)
        {
            runningRate += decoratePrefab[i].dropRate;
            if (rand < runningRate)
            {
                return decoratePrefab[i].prefab;
            }
        }
        return null;
    }

}

[Serializable]
public class Decorate
{
    public GameObject prefab;
    public float dropRate;
}

[Serializable]
public class Cell
{
    public bool isWater;

    public float noiseValue;

    public Cell(bool isWater)
    {
        this.isWater = isWater;
    }
}
