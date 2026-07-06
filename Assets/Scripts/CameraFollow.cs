using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // Buraya karakterini (Cube) sürükleyeceğiz
    public Vector3 offset;   // Kamera ile karakter arasındaki mesafe

    void Start()
    {
        // Oyun başladığında aradaki mesafeyi otomatik hesapla
        offset = transform.position - player.position;
    }

    void LateUpdate() // Kameranın hareketinden sonra çalışması için
    {
        // Kamerayı karakterin pozisyonuna + mesafeye taşı
        transform.position = player.position + offset;
    }
}