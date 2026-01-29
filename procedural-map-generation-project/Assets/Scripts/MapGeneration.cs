using System;
using Unity.Mathematics;
using UnityEngine;

public class MapGeneration : MonoBehaviour
{
    Cell[,] grid;
    float[,] noiseMap;

    [Range(1, 128)]
    [SerializeField] int mapSize;

    [SerializeField] float noiseMap_Scale;

    [Range(0f, 1f)]
    [SerializeField] float water_Thrashold;

    [Range(0f, 1f)]
    [SerializeField] float sand_Thrashold;

    [SerializeField] GameObject grass_Prefab;
    [SerializeField] GameObject sand_Prefab;
    private void Start()
    {
        GenerateMap();
        GenerateMapVisual();
    }

    void GenerateMap()
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

    GameObject SelectMapVisualPrefab(float value)
    {
        if(value < water_Thrashold)
        {
            return null;
        }
        else
        {
            if(value > water_Thrashold && value <= sand_Thrashold)
            {
                return sand_Prefab;
            }
            else
            {
                return grass_Prefab;
            }
        }
    }
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
