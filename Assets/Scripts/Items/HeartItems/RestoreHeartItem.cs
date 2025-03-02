using UnityEngine;
using Scripts.Healths;
using Scripts.Combats.CharacterCombats;

namespace Scripts.Items
{
    public class RestoreHeartItem : Item
    {
        [SerializeField] private int healAmount = 1; // Amount the heart heals

        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            // Check if the player touched the heart
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();

            if (playerHealth != null) // If it's a player
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
                Destroy(gameObject); // Remove the heart after pickup
            }
        }
    }
}