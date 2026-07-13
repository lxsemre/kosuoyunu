using UnityEngine;
using System.Collections.Generic;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Engel Prefab'ları")]
    public GameObject smallObstaclePrefab;
    public GameObject bigObstaclePrefab;
    public GameObject duckObstaclePrefab;

    [Header("Item Prefab'ları")]
    public GameObject goldPrefab;
    public GameObject diamondPrefab;

    [Header("Rampa Prefab'ı ve Ayarları")]
    public GameObject rampPrefab;
    [Tooltip("Rampanın yerden yüksekliği (BigObstacle üstüne ulaşana kadar ayarla)")]
    public float rampYPosition = 0.8f;
    [Tooltip("Rampanın BigObstacle'ın önünden ne kadar uzakta olacağı")]
    public float rampZOffset = 2f;
    [Range(0f, 1f)]
    public float rampChance = 0.7f;

    [Header("Şerit Ayarları")]
    public float[] lanePositions = { -2.5f, 0f, 2.5f };

    [Header("Parkur Ayarları")]
    [Tooltip("Engel grupları arası mesafe")]
    public float sectionLength = 5f;
    [Tooltip("Altın/Elmasların yerden yüksekliği")]
    public float itemYPosition = 0.4f;
    public float duckObstacleY = 1.5f;



    [Header("Pool Ayarları")]
    public int smallPoolSize = 30;
    public int bigPoolSize = 15;
    public int duckPoolSize = 10;
    public int rampPoolSize = 10;
    public int goldPoolSize = 120;
    public int diamondPoolSize = 40;

    // Pools
    private Queue<GameObject> smallPool = new Queue<GameObject>();
    private Queue<GameObject> bigPool = new Queue<GameObject>();
    private Queue<GameObject> duckPool = new Queue<GameObject>();
    private Queue<GameObject> rampPool = new Queue<GameObject>();
    private Queue<GameObject> goldPool = new Queue<GameObject>();
    private Queue<GameObject> diamondPool = new Queue<GameObject>();
    private Dictionary<GameObject, Queue<GameObject>> poolMap = new Dictionary<GameObject, Queue<GameObject>>();
    private Dictionary<float, List<GameObject>> activeByRoad = new Dictionary<float, List<GameObject>>();

    

    void Start() { InitPools(); }

    void InitPools()
    {
        FillPool(smallObstaclePrefab, smallPool, smallPoolSize);
        FillPool(bigObstaclePrefab, bigPool, bigPoolSize);
        FillPool(duckObstaclePrefab, duckPool, duckPoolSize);
        FillPool(rampPrefab, rampPool, rampPoolSize);
        FillPool(goldPrefab, goldPool, goldPoolSize);
        FillPool(diamondPrefab, diamondPool, diamondPoolSize);
    }

    void FillPool(GameObject prefab, Queue<GameObject> pool, int size)
    {
        if (prefab == null) return;
        for (int i = 0; i < size; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            poolMap[obj] = pool;
            pool.Enqueue(obj);
        }
    }

    GameObject GetFromPool(Queue<GameObject> pool, GameObject prefab)
    {
        if (prefab == null) return null;
        GameObject obj = (pool.Count > 0) ? pool.Dequeue() : Instantiate(prefab);
        poolMap[obj] = pool;
        return obj;
    }

    void ReturnToPool(GameObject obj)
    {
        obj.SetActive(false);
        if (poolMap.TryGetValue(obj, out Queue<GameObject> pool))
            pool.Enqueue(obj);
        else
            Destroy(obj);
    }

    void PlaceObj(Queue<GameObject> pool, GameObject prefab, float x, float y, float z, List<GameObject> list)
    {
        if (prefab == null) return;
        GameObject obj = GetFromPool(pool, prefab);
        obj.transform.position = new Vector3(x, y, z);
        obj.transform.rotation = prefab.transform.rotation; 
        obj.SetActive(true);
        list.Add(obj);
    }

    

    public void SpawnObstaclesOnRoad(float roadZ)
    {
        List<GameObject> spawned = new List<GameObject>();
        float z = roadZ + 10f;    
        float endZ = roadZ + 105f;

       
        while (z < endZ)
        {
            float roll = Random.value;
            float currentSectionLength = 8f; 

            
            if (roll < 0.40f)
                currentSectionLength = 14f; 
            else if (roll < 0.55f)
                currentSectionLength = 10f; 
            else
                currentSectionLength = 8f;  

            
            List<int> blockedLanes = SpawnObstacle(z, roll, spawned);

            
            List<int> freeLanes = new List<int> { 0, 1, 2 };
            freeLanes.RemoveAll(l => blockedLanes.Contains(l));
            
            
            foreach (int coinLane in freeLanes)
            {
                
                if (Random.value < 0.85f)
                {
                    
                    int maxCoins = Mathf.FloorToInt((currentSectionLength - 2f) / 1.2f); 
                    if (maxCoins > 0)
                    {
                        int coinCount = Random.Range(Mathf.Max(2, maxCoins - 1), maxCoins + 1);
                        for (int i = 0; i < coinCount; i++)
                        {
                            float coinZ = z + 2f + (i * 1.2f); 
                            if (coinZ >= endZ) break;
                            PlaceObj(goldPool, goldPrefab, lanePositions[coinLane], itemYPosition, coinZ, spawned);
                        }
                    }
                }
            }

            
            z += currentSectionLength; 
        }

        activeByRoad[roadZ] = spawned;
    }

    public void ReturnObstaclesFromRoad(float roadZ)
    {
        if (!activeByRoad.ContainsKey(roadZ)) return;
        foreach (GameObject obj in activeByRoad[roadZ])
        {
            if (obj != null) ReturnToPool(obj);
        }
        activeByRoad.Remove(roadZ);
    }

    

    List<int> SpawnObstacle(float z, float roll, List<GameObject> spawned)
    {
        
        int blockedCount = (Random.value < 0.65f) ? 1 : 2;

        
        List<int> lanes = new List<int> { 0, 1, 2 };
        Shuffle(lanes);

        List<int> blockedLanes = new List<int>();

        for (int i = 0; i < blockedCount; i++)
        {
            int lane = lanes[i];
            blockedLanes.Add(lane);

            if (roll < 0.40f)
            {
                
                SpawnBigCombo(z, lane, spawned);
            }
            else if (roll < 0.55f && duckObstaclePrefab != null)
            {
                
                PlaceObj(duckPool, duckObstaclePrefab, lanePositions[lane], duckObstacleY, z, spawned);
            }
            else
            {
                
                if (smallObstaclePrefab != null)
                {
                    float sy = smallObstaclePrefab.transform.localScale.y / 2f;
                    PlaceObj(smallPool, smallObstaclePrefab, lanePositions[lane], sy, z, spawned);
                }
            }
        }

        return blockedLanes;
    }

    

    void SpawnBigCombo(float z, int lane, List<GameObject> spawned)
    {
        if (bigObstaclePrefab == null) return;

        float x = lanePositions[lane];
        float bigY = bigObstaclePrefab.transform.localScale.y / 2f;
        float bigH = bigObstaclePrefab.transform.localScale.y;
        float bigZ = bigObstaclePrefab.transform.localScale.z;

        
        PlaceObj(bigPool, bigObstaclePrefab, x, bigY, z, spawned);

        
        bool hasRamp = rampPrefab != null && Random.value <= rampChance;

        if (hasRamp)
        {
            
            float rampZ = z - bigZ / 2f - rampZOffset;
            PlaceObj(rampPool, rampPrefab, x, rampYPosition, rampZ, spawned);

            
            float topY = bigH + 0.5f;
            int topCount = 3;
            for (int j = 0; j < topCount; j++)
            {
                float iz = z - bigZ / 2f + (bigZ / topCount) * j + (bigZ / topCount) / 2f;
                bool isDiamond = diamondPrefab != null && Random.value < 0.2f;
                if (isDiamond)
                    PlaceObj(diamondPool, diamondPrefab, x, topY, iz, spawned);
                else
                    PlaceObj(goldPool, goldPrefab, x, topY, iz, spawned);
            }

           
            if (Random.value < 0.40f)
            {
                float z2 = z + bigZ + 0.5f;
                PlaceObj(bigPool, bigObstaclePrefab, x, bigY, z2, spawned);
                for (int j = 0; j < 2; j++)
                {
                    float iz = z2 - bigZ / 2f + (bigZ / 2f) * j + bigZ / 4f;
                    PlaceObj(goldPool, goldPrefab, x, topY, iz, spawned);
                }
            }
        }
        
    }



   

    void Shuffle(List<int> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            int t = list[i]; list[i] = list[j]; list[j] = t;
        }
    }
}
