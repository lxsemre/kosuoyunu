using UnityEngine;
using System.Collections.Generic;

public class RoadGenerator : MonoBehaviour
{
    [Header("Yol Ayarları")]
    public GameObject[] roadPrefabs; // Farklı yol tasarımlarını tutacak liste
    public float roadLength = 50f; // BİR yol parçasının Z eksenindeki uzunluğu (Bunu kendi yoluna göre değiştir!)
    public int amountOfRoadsOnScreen = 5; // Ekranda aynı anda bulunacak yol sayısı
    public float safeZone = 40f; // Karakter yolu geçtikten ne kadar sonra eski yol silinsin

    [Header("Bağlantılar")]
    public Transform playerTransform; // Karakterin konumu

    private float spawnZ = 0f; // Yeni yolun doğacağı Z noktası
    private List<GameObject> activeRoads = new List<GameObject>();

    void Start()
    {
        // Oyun başlarken ekrana ilk yol parçalarını diz
        for (int i = 0; i < amountOfRoadsOnScreen; i++)
        {
            SpawnRoad(0);
        }
    }

    void Update()
    {
        // Karakterimiz ilerledikçe yeni yol üret ve arkada kalanı sil
        if (playerTransform.position.z - safeZone > (spawnZ - amountOfRoadsOnScreen * roadLength))
        {
            // Eğer birden fazla yol prefabın varsa rastgele birini seçer
            SpawnRoad(Random.Range(0, roadPrefabs.Length));
            DeleteRoad();
        }
    }

    void SpawnRoad(int prefabIndex)
    {
        // Yolu Z ekseninde spawnZ noktasına kopyala
        GameObject go = Instantiate(roadPrefabs[prefabIndex], Vector3.forward * spawnZ, Quaternion.identity);
        activeRoads.Add(go);
        spawnZ += roadLength; // Bir sonraki yolun doğacağı yeri, yol uzunluğu kadar ileri at
    }

    void DeleteRoad()
    {
        // Arkada kalan en eski yolu (listenin 0. elemanını) sil
        Destroy(activeRoads[0]);
        activeRoads.RemoveAt(0);
    }
}