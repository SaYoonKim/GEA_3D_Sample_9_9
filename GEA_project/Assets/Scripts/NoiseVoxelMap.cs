using UnityEngine;
using System.Collections.Generic; 

public class NoiseVoxelMap : MonoBehaviour
{
    public GameObject dirtPrefab;
    public GameObject grassPrefab;
    public GameObject waterPrefab;

    public int width = 20;
    public int depth = 20;
    public int maxHeight = 16; // Y
    [SerializeField] float noiseScale = 20f;

    // Water 기준 높이
    public int waterLevel = 5;
    private HashSet<Vector3> placedBlockPositions = new HashSet<Vector3>();

    void Start()
    {
        // 지형 배치 
        GenerateTerrain();

        // Water 배치
        PlaceWater();
    }

    private void GenerateTerrain()
    {
        float offsetX = Random.Range(-9999f, 9999f);
        float offsetZ = Random.Range(-9999f, 9999f);

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                float nx = (x + offsetX) / noiseScale;
                float nz = (z + offsetZ) / noiseScale;

                float noise = Mathf.PerlinNoise(nx, nz);

                int h = Mathf.FloorToInt(noise * maxHeight);
                if (h <= 0) continue;

             
                for (int y = 0; y < h; y++)
                {
                    // 맨 상단에만 Grass 배치
                    GameObject blockToPlace;
                    if (y == h - 1)
                    {
                        blockToPlace = grassPrefab;
                    }
                    else
                    {
                        blockToPlace = dirtPrefab;
                    }

                    Place(x, y, z, blockToPlace);
                }
            }
        }
    }

    private void PlaceWater()
    {
        // Water 배치 
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                // y 높이 검색
                for (int y = 0; y < waterLevel; y++)
                {
                    Vector3 position = new Vector3(x, y, z);

                    // 블록배치확인
                    if (!placedBlockPositions.Contains(position))
                    {
                        // 블록이 없으면 Water 배치
                        Place(x, y, z, waterPrefab);
                    }
                }
            }
        }
    }

    private void Place(int x, int y, int z, GameObject prefab)
    {
        Vector3 position = new Vector3(x, y, z);

        var go = Instantiate(prefab, position, Quaternion.identity, transform);
        go.name = $"{prefab.name.Replace("(Clone)", "")}_{x}_{y}_{z}"; 

        
        placedBlockPositions.Add(position);
    }
}