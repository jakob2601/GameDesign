using UnityEngine;
using Scripts.Scene;

public class CameraFollow : MonoBehaviour
{
    private Transform target;
    public Vector3 offset = new Vector3(0, 0, -10);
    public float smoothSpeed = 0.125f;

    void Start()
    {
        FindPlayer();
    }

    void LateUpdate()
    {
        if (target == null)
        {
            FindPlayer();
            return;
        }

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }

    void FindPlayer()
    {
        PlayerPersistence player = FindObjectOfType<PlayerPersistence>();
        if (player != null)
        {
            target = player.transform;
            Debug.Log("Camera found player to follow");
        }
    }
}