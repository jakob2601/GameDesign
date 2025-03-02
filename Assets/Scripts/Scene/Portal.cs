using UnityEngine;
using UnityEngine.SceneManagement; // For scene management

public class PortalTeleport : MonoBehaviour
{
    // This method is called when another collider enters the portal's trigger zone
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object that collided is the player
        if (other.CompareTag("Player"))
        {
            // Load the next level (you can modify the scene name or build index)
            // This assumes the next level is loaded sequentially in build settings
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
