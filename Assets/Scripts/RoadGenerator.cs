using UnityEngine;
using System.Collections.Generic;

public class RoadGenerator : MonoBehaviour
{
    [Header("Yol Ayarları")]
    public GameObject[] roadPrefabs;        

    public float pivotSpacing = 111f;       

    public float roadForwardExtent = 86f;   

    public int amountOfRoadsOnScreen = 5;   
    public float safeZone = 40f;            

    [Header("Object Pool Ayarları")]
    public int poolSizePerPrefab = 8;

    [Header("Bağlantılar")]
    public Transform playerTransform;       

    private float spawnZ = 0f;
    private List<GameObject> activeRoads = new List<GameObject>();

    private Queue<GameObject>[] roadPools;

    private Dictionary<GameObject, int> prefabIndexMap = new Dictionary<GameObject, int>();

    void Start()
    {
        InitializePools();

        for (int i = 0; i < amountOfRoadsOnScreen; i++)
        {
            SpawnRoad(0);
        }
    }

    void InitializePools()
    {
        roadPools = new Queue<GameObject>[roadPrefabs.Length];

        for (int i = 0; i < roadPrefabs.Length; i++)
        {
            roadPools[i] = new Queue<GameObject>();

            for (int j = 0; j < poolSizePerPrefab; j++)
            {
                CreateAndStorePoolObject(i);
            }
        }
    }

    void CreateAndStorePoolObject(int prefabIndex)
    {
        GameObject obj = Instantiate(roadPrefabs[prefabIndex]);
        obj.SetActive(false);
        prefabIndexMap[obj] = prefabIndex;   
        roadPools[prefabIndex].Enqueue(obj);
    }

    void Update()
    {
        while (spawnZ < playerTransform.position.z + pivotSpacing * amountOfRoadsOnScreen)
        {
            SpawnRoad(Random.Range(0, roadPrefabs.Length));
        }

        ReturnOldRoadsToPool();
    }

    void SpawnRoad(int prefabIndex)
    {
        GameObject road = GetFromPool(prefabIndex);

        ResetRoad(road);

        road.transform.position = Vector3.forward * spawnZ;
        road.transform.rotation = Quaternion.identity;
        road.SetActive(true);
        activeRoads.Add(road);
        spawnZ += pivotSpacing;  
    }

    void ResetRoad(GameObject road)
    {
        Transform[] allChildren = road.GetComponentsInChildren<Transform>(true);
        foreach (Transform child in allChildren)
        {
            if (child != road.transform) 
                child.gameObject.SetActive(true);
        }
    }

    void ReturnOldRoadsToPool()
    {
        while (activeRoads.Count > 0)
        {
            GameObject oldest = activeRoads[0];

            if (oldest.transform.position.z + roadForwardExtent < playerTransform.position.z - safeZone)
            {
                activeRoads.RemoveAt(0);
                oldest.SetActive(false);

                if (prefabIndexMap.TryGetValue(oldest, out int index))
                    roadPools[index].Enqueue(oldest);
            }
            else
            {
                break; 
            }
        }
    }

    GameObject GetFromPool(int prefabIndex)
    {
        if (roadPools[prefabIndex].Count > 0)
        {
            return roadPools[prefabIndex].Dequeue();
        }

        GameObject newObj = Instantiate(roadPrefabs[prefabIndex]);
        prefabIndexMap[newObj] = prefabIndex;
        return newObj;
    }
}