using UnityEngine;
using System.Collections.Generic;

public class RoadGenerator : MonoBehaviour
{
    [Header("Yol Ayarları")]
    public GameObject[] roadPrefabs;        // Farklı yol tasarımlarını tutacak liste

    [Tooltip("Pivot noktaları arası mesafe. Hesap: |yolun sonu| + |yolun başı| = 86 + 25 = 111")]
    public float pivotSpacing = 111f;       // Yol pivotları arası mesafe (spawn için)

    [Tooltip("Pivottan yolun ileriye uzadığı mesafe. Yolun Z sonu = 86")]
    public float roadForwardExtent = 86f;   // Pivottan ileriye uzanan mesafe (silme için)

    public int amountOfRoadsOnScreen = 5;   // Ekranda aynı anda bulunacak yol sayısı
    public float safeZone = 40f;            // Oyuncunun ne kadar gerisindeki yollar silinsin

    [Header("Object Pool Ayarları")]
    [Tooltip("Her prefab türü için önceden hazırlanacak yol sayısı. amountOfRoadsOnScreen + 3 civarı yeterli.")]
    public int poolSizePerPrefab = 8;

    [Header("Bağlantılar")]
    public Transform playerTransform;       // Karakterin konumu

    private float spawnZ = 0f;
    private List<GameObject> activeRoads = new List<GameObject>();

    // Her prefab türü için ayrı bir bekleme kuyruğu (pool)
    private Queue<GameObject>[] roadPools;

    // Her objenin hangi prefab'a ait olduğunu tutar (geri döndürme için)
    private Dictionary<GameObject, int> prefabIndexMap = new Dictionary<GameObject, int>();

    // ─────────────────────────────────────────────
    // BAŞLANGIÇ: Pool'ları hazırla, ilk yolları döşe
    // ─────────────────────────────────────────────
    void Start()
    {
        InitializePools();

        // Başlangıçta ekranı yollarla doldur
        for (int i = 0; i < amountOfRoadsOnScreen; i++)
        {
            SpawnRoad(0);
        }
    }

    /// <summary>
    /// Her prefab türü için poolSizePerPrefab adet objeyi önceden oluşturur,
    /// deaktif halde bekletir. Oyun boyunca gereksiz Instantiate çağrısı olmaz.
    /// </summary>
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

    /// <summary>
    /// Verilen prefab indeksinden bir obje oluşturur, deaktif halde pool'a ekler.
    /// </summary>
    void CreateAndStorePoolObject(int prefabIndex)
    {
        GameObject obj = Instantiate(roadPrefabs[prefabIndex]);
        obj.SetActive(false);
        prefabIndexMap[obj] = prefabIndex;   // Hangi prefab'a ait olduğunu kaydet
        roadPools[prefabIndex].Enqueue(obj);
    }

    // ─────────────────────────────────────────────
    // GÜNCELLEME: Oyuncunun önünde HER ZAMAN yeterli yol olsun
    // ─────────────────────────────────────────────
    void Update()
    {
        // Oyuncunun önünde amountOfRoadsOnScreen * pivotSpacing kadar yol olduğu sürece spawn et.
        // while döngüsü sayesinde yüksek hızda bile yol ASLA bitmez.
        while (spawnZ < playerTransform.position.z + pivotSpacing * amountOfRoadsOnScreen)
        {
            SpawnRoad(Random.Range(0, roadPrefabs.Length));
        }

        // Oyuncunun gerisinde kalan eski yolları pool'a geri koy
        ReturnOldRoadsToPool();
    }

    /// <summary>
    /// Pool'dan bir yol alır, spawnZ noktasına koyar, aktifleştirir.
    /// </summary>
    void SpawnRoad(int prefabIndex)
    {
        GameObject road = GetFromPool(prefabIndex);

        // Yolun tüm child objelerini sıfırla (engel, collectible vb. yeniden aktif olsun)
        ResetRoad(road);

        road.transform.position = Vector3.forward * spawnZ;
        road.transform.rotation = Quaternion.identity;
        road.SetActive(true);
        activeRoads.Add(road);
        spawnZ += pivotSpacing;  // Bir sonraki yolun pivot noktası
    }

    /// <summary>
    /// Yol pool'dan alınırken tüm child objelerini (deaktif olanlar dahil) yeniden aktifleştirir.
    /// Engeller ve collectible'lar bir önceki turda SetActive(false) yapıldığı için bu gereklidir.
    /// </summary>
    void ResetRoad(GameObject road)
    {
        // includeInactive = true → deaktif child'ları da getirir
        Transform[] allChildren = road.GetComponentsInChildren<Transform>(true);
        foreach (Transform child in allChildren)
        {
            if (child != road.transform) // Yolun kendisini hariç tut
                child.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Oyuncunun safeZone kadar gerisinde kalan yolları deaktif edip pool'a iade eder.
    /// </summary>
    void ReturnOldRoadsToPool()
    {
        while (activeRoads.Count > 0)
        {
            GameObject oldest = activeRoads[0];

            // Yolun ileri ucu (pivot + roadForwardExtent) oyuncunun safeZone gerisinde mi?
            if (oldest.transform.position.z + roadForwardExtent < playerTransform.position.z - safeZone)
            {
                activeRoads.RemoveAt(0);
                oldest.SetActive(false);

                if (prefabIndexMap.TryGetValue(oldest, out int index))
                    roadPools[index].Enqueue(oldest);
            }
            else
            {
                break; // En eski yol henüz geride değil, dur
            }
        }
    }

    /// <summary>
    /// Pool'dan obje alır. Pool boşsa yeni oluşturur (güvenlik ağı).
    /// </summary>
    GameObject GetFromPool(int prefabIndex)
    {
        if (roadPools[prefabIndex].Count > 0)
        {
            return roadPools[prefabIndex].Dequeue();
        }

        Debug.LogWarning($"[RoadGenerator] Pool bitti! prefabIndex: {prefabIndex} — poolSizePerPrefab değerini artır.");
        GameObject newObj = Instantiate(roadPrefabs[prefabIndex]);
        prefabIndexMap[newObj] = prefabIndex;
        return newObj;
    }
}