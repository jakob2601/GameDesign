using UnityEngine;
using Scripts.Healths;
using Scripts.Combats.CharacterCombats;
using Scripts.Scene; // Add this for PlayerPersistence

namespace Scripts.Items
{
    public class ExtraHeartItem : Item
    {
        [SerializeField] protected bool healsFullHealth = false;
        [SerializeField] protected int healAmount = 1;
        [SerializeField] protected int extraHeartAmount = 1;

        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            PlayerHealth playerHealth = collision.GetComponentInChildren<PlayerHealth>();

            if(playerHealth == null)
            {
                playerHealth = collision.GetComponentInParent<PlayerHealth>();
            }

            if (playerHealth != null)
            {
                if (healsFullHealth)
                {
                    playerHealth.FullHeal();
                    Debug.Log("Player healed to full health.");
                }
                else
                {
                    playerHealth.Heal(healAmount);
                    Debug.Log("Player healed by " + healAmount + " HP.");
                }

                // Fix variable name in persistence call
                PlayerPersistence persistence = collision.GetComponent<PlayerPersistence>();
                if (persistence != null)
                {
                    persistence.AddExtraHeart(extraHeartAmount); // Fixed variable name
                }
                Destroy(gameObject);
            }
        }
    }
}