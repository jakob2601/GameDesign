using System.Collections;
using UnityEngine;
using Scripts.Combats.CharacterCombats;
using Scripts.Combats.Weapons;
using Scripts.Characters;
using System.Threading;
using UnityEngine.SceneManagement;
using Scripts.UI;

namespace Scripts.Healths
{
    public class PlayerHealth : Health
    {
        [SerializeField] public HealthBarController healthBarController;
        [SerializeField] private int maxPossibleHealth = 10; // Maximum possible health, changeable in inspector

        protected override void Start()
        {
            // Ensure maxHealth doesn't exceed the configurable limit
            maxHealth = Mathf.Min(maxHealth, maxPossibleHealth);

            // Call the base class initialization
            base.Start();

            // Initialize the health bar
            if (healthBarController != null)
            {
                healthBarController.InitializeHearts(maxHealth);
                healthBarController.UpdateHearts(currentHealth, maxHealth);
            }
        }

        public override void TakeDamage(int damage, Vector2 hitDirection, float knockbackForce, float knockbackDuration)
        {
            // Only apply damage and trigger invincibility if we're not already invincible
            base.TakeDamage(damage, hitDirection, knockbackForce, knockbackDuration);
        }

        protected override void initializeHealthBar(int maxHealth)
        {
            // Initialize the health bar
            if (healthBarController != null)
            {
                healthBarController.InitializeHearts(maxHealth);
            }
        }

        protected override void updateHealthBar(int currentHealth, int maxHealth)
        {
            // Add null check to prevent NullReferenceException
            if (healthBarController != null)
            {
                healthBarController.UpdateHearts(currentHealth, maxHealth);
            }
        }

        public override void Heal(int amount)
        {
            base.Heal(amount);
            // Update the health bar
            if (healthBarController != null)
            {
                healthBarController.UpdateHearts(currentHealth, maxHealth);
            }
        }

        public void FullHeal()
        {
            // Heal the player to full health
            currentHealth = maxHealth;
            // Update the health bar
            if (healthBarController != null)
            {
                healthBarController.UpdateHearts(currentHealth, maxHealth);
            }
        }

        public void IncreaseMaxHealth(int amount)
        {
            // Check if we're already at the maximum allowed health
            if (maxHealth >= maxPossibleHealth)
            {
                Debug.Log("Cannot increase max health beyond the limit of " + maxPossibleHealth);
                return;
            }

            // Calculate how much health can be added without exceeding the limit
            int actualIncrease = Mathf.Min(amount, maxPossibleHealth - maxHealth);

            if (actualIncrease <= 0)
            {
                return; // No increase possible
            }

            maxHealth += actualIncrease;
            currentHealth += actualIncrease; // Optionally heal the player by the same amount

            // Re-initialize the health bar to accommodate the new max health
            if (healthBarController != null)
            {
                healthBarController.InitializeHearts(maxHealth);
                healthBarController.UpdateHearts(currentHealth, maxHealth);
            }
            Debug.Log("Max health increased by " + actualIncrease + ". New max health: " + maxHealth);
        }

        protected override void Hurt()
        {
            // Play Hurt Animation
            characterAnimation.SetIsHurt(true);
        }

        protected override void Die()
        {
            Debug.Log("Player has died!");

            Debug.Log("Spieler ist gestorben. Lade GameOver-Szene...");
            SceneManager.LoadScene("GameOverMenu"); // Load the GameOver scene
            Respawn();
        }

        protected void Respawn()
        {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(currentSceneIndex);
        }
    }
}