using UnityEngine;
using Scripts.Healths; // Stelle sicher, dass das korrekte Namespace importiert ist

public class HeartPickup : MonoBehaviour
{
    [SerializeField] private int healAmount = 1; // Wie viel das Herz heilt

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Prüfe, ob der Spieler das Herz berührt hat
        PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();

        if (playerHealth != null) // Falls es ein Spieler ist
        {
            if (playerHealth.currentHealth < playerHealth.maxHealth) // Spieler ist nicht voll geheilt
            {
                playerHealth.Heal(healAmount); // Heile den Spieler um 1 HP
                Destroy(gameObject); // Entferne das Herz nach dem Aufsammeln
                Debug.Log("Spieler wurde geheilt um " + healAmount + " HP.");
            }
            else
            {
                Debug.Log("Spieler hat bereits volles Leben, Herz bleibt.");
            }
        }
    }
}
