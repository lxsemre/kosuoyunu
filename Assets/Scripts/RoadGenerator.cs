using UnityEngine;
using System.Collections.Generic; // Listeler için lazım

public class RoadGenerator : MonoBehaviour
{
    public GameObject roadPrefab;
    public Transform player;
    private List<GameObject> activeRoads = new List<GameObject>(); // Oluşturduğumuz yolları burada tutacağız
    private float spawnZ = 50f;
    private float roadLength = 50f;

    void Update()
    {
        if (player.position.z > spawnZ - 100f)
        {
            SpawnRoad();
        }

        // Arkada kalan yolu silme kontrolü
        if (activeRoads.Count > 0 && activeRoads[0].transform.position.z < player.position.z - 20f)
        {
            Destroy(activeRoads[0]); // Eski yolu yok et
            activeRoads.RemoveAt(0); // Listeden çıkart
        }
    }

    void SpawnRoad()
    {
        GameObject newRoad = Instantiate(roadPrefab, new Vector3(0, 0, spawnZ), Quaternion.identity);
        activeRoads.Add(newRoad); // Yeni oluşturulanı listeye ekle
        spawnZ += roadLength;
    }
}