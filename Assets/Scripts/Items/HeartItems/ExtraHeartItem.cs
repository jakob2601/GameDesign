using UnityEngine;
using Scripts.Healths;
using Scripts.Combats.CharacterCombats;

namespace Scripts.Items
{
    public class ExtraHeartItem : Item
    {
        [SerializeField] protected bool healsFullHealth = false; // Flag to indicate if this heart heals full health
        [SerializeField] protected int healAmount = 1; // Amount the heart heals
        [SerializeField] protected int extraHeartAmount = 1; // Amount to increase max health

        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            // Check if the player touched the heart
            PlayerHealth playerHealth = collision.GetComponentInChildren<PlayerHealth>();

            if(playerHealth == null)
            {
                playerHealth = collision.GetComponentInParent<PlayerHealth>();
            }

            if (playerHealth != null) // If it's a player
            {
                if (healsFullHealth)
                {
                    playerHealth.FullHeal();
                    Debug.Log("Player healed to full health.");
                }
                else 
                {
                    playerHealth.Heal(healAmount); // Heal the player by healAmount
                    Debug.Log("Player healed by " + healAmount + " HP.");
                }
                playerHealth.IncreaseMaxHealth(extraHeartAmount); // Increase max health
                Debug.Log("Player's max health increased by " + extraHeartAmount + ".");
                Destroy(gameObject); // Remove the heart after pickup
            }
        }
    }
}