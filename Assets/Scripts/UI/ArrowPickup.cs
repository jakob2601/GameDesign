using UnityEngine;
using Scripts.Combats.CharacterCombats;

public class ArrowPickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the player or its children/parent have the PlayerCombat component
        PlayerCombat playerCombat = collision.GetComponentInChildren<PlayerCombat>();
        if (playerCombat == null)
        {
            playerCombat = collision.GetComponentInParent<PlayerCombat>();
        }

        if (playerCombat != null) // If it's a player
        {
            playerCombat.EnableArrowShooting(); // Enable arrow shooting
            Debug.Log("Player can now shoot arrows.");
            Destroy(gameObject); // Remove the arrow after pickup
        }
    }
}