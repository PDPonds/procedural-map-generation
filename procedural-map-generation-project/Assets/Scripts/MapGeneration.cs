using System;
using Unity.Mathematics;
using UnityEngine;

public class MapGeneration : MonoBehaviour
{
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
    [SerializeField] GameObject grass_Prefab;
    [SerializeField] GameObject sand_Prefab;
    [Header("----- Decorate -----")]
    [SerializeField] float decorateNoiseScale;
    [SerializeField] Decorate[] decoratePrefab;
    [SerializeField] float decorate_density;


    private void Start()
    {
        GenerateMap();
    }

    void GenerateMap()
    {
        GenerateCell();
        GenerateMapVisual();
        GenerateDecoration();
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
            }
        }

        for (int y = 0; y < mapSize; y++)
        {
            for (int x = 0; x < mapSize; x++)
            {
                float noiseValue = Mathf.PerlinNoise(x * noiseMap_Scale + xOffset, y * noiseMap_Scale + yOffset);
                noiseMap[x, y] = noiseValue - falloffMap[x, y];
            }
        }

        for (int y = 0; y < mapSize; y++)
        {
            for (int x = 0; x < mapSize; x++)
            {
                bool isWater = noiseMap[x, y] <= water_Thrashold;
                grid[x, y] = new Cell(isWater);
            }
        }
    }

    void GenerateMapVisual()
    {
        for (int y = 0; y < mapSize; y++)
        {
            for (int x = 0; x < mapSize; x++)
            {
                Vector3 cellPos = new Vector3(x, 0, y);
                Cell cell = grid[x, y];
                if (!cell.isWater)
                {
                    GameObject prefab = SelectMapVisualPrefab(noiseMap[x, y]);
                    GameObject go = Instantiate(prefab, cellPos, Quaternion.identity);
                    go.transform.SetParent(transform, true);
                }
            }
        }
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
                            Vector3 pos = new Vector3(x, 0, y);
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

    GameObject SelectMapVisualPrefab(float value)
    {
        if (value < water_Thrashold)
        {
            return null;
        }
        else
        {
            if (value > water_Thrashold && value <= sand_Thrashold)
            {
                return sand_Prefab;
            }
            else
            {
                return grass_Prefab;
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
    public Cell(bool isWater)
    {
        this.isWater = isWater;
    }
}
