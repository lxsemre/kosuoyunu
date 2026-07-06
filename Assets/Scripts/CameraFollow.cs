using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // Küpü buraya sürükleyeceğiz
    public Vector3 offset = new Vector3(0, 100, -250);// Kamera mesafesi

    void Update()
    {
        if (target != null)
        {
            transform.position = target.position + offset;
        }
    }
}