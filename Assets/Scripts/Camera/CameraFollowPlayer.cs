using UnityEngine;

namespace Scripts.Camera
{
    public class CameraFollow : MonoBehaviour
    {
        public Transform player; // Referenz zum Spieler
        public float smoothSpeed = 0.125f; // Geschmeidigkeit der Bewegung
        public Vector3 offset; // Optionaler Offset zwischen Spieler und Kamera

        private void LateUpdate()
        {
            if (player != null)
            {
                // Zielposition der Kamera
                Vector3 targetPosition = player.position + offset;

                // Geschmeidige Kamerabewegung
                Vector3 smoothedPosition = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);

                // Kamera aktualisieren
                transform.position = smoothedPosition;
            }
            else
            {
            }
        }
    }

}
