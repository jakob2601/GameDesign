using UnityEngine;

public class FloatingItem : MonoBehaviour
{
    [SerializeField] private float floatSpeed = 7f; // Geschwindigkeit der Bewegung
    [SerializeField] private float floatHeight = 0.05f; // Höhe der Bewegung

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }
}
