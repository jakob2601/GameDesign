using UnityEngine;
using Scripts.Healths;
using Scripts.Combats.CharacterCombats;

public class HeartPickup : MonoBehaviour
{
    [SerializeField] private int healAmount = 1; // Amount the heart heals
    [SerializeField] private int extraHeartAmount = 1; // Amount to increase max health
    [SerializeField] private bool isExtraHealthHeart = false; // Flag to indicate if this is an extra health heart

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the player touched the heart
        PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();

        if (playerHealth != null) // If it's a player
        {
            if (isExtraHealthHeart)
            {
                playerHealth.IncreaseMaxHealth(extraHeartAmount); // Increase max health
                Debug.Log("Player's max health increased by " + extraHeartAmount + ".");
            }
            else
            {
                if (playerHealth.currentHealth < playerHealth.maxHealth) // Player is not fully healed
                {
                    playerHealth.Heal(healAmount); // Heal the player by healAmount
                    Debug.Log("Player healed by " + healAmount + " HP.");
                }
                else
                {
                    Debug.Log("Player already has full health, heart remains.");
                    return; // Do not destroy the heart if the player is already fully healed
                }
            }
            Destroy(gameObject); // Remove the heart after pickup
        }
    }
}