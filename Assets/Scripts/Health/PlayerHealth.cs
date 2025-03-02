using System.Collections;
using Scripts.UI;
using UnityEngine;
using Scripts.Combats.CharacterCombats;
using Scripts.Combats.Weapons;
using Scripts.Characters;
using System.Threading;
using UnityEngine.SceneManagement;

namespace Scripts.Healths
{
    public class PlayerHealth : Health
    {
        [SerializeField] public HealthBarController healthBarController;

        protected override void Start()
        {
            // Call the base class initialization
            base.Start();
        }
        public override void TakeDamage(int damage, Vector2 hitDirection, float knockbackForce, float knockbackDuration)
        {
            // Only apply damage and trigger invincibility if we're not already invincible
            base.TakeDamage(damage, hitDirection, knockbackForce, knockbackDuration);
        }

        protected override void initializeHealthBar(int maxHealth)
        {
            // Initialize the health bar
            healthBarController.InitializeHearts(maxHealth);
        }

        protected override void updateHealthBar(int currentHealth, int maxHealth)
        {
            // Update the health bar
            healthBarController.UpdateHearts(currentHealth, maxHealth);
        }

        public override void Heal(int amount)
        {
            base.Heal(amount);
            // Update the health bar
            healthBarController.UpdateHearts(currentHealth, maxHealth);
        }

        public void FullHeal()
        {
            // Heal the player to full health
            currentHealth = maxHealth;
            // Update the health bar
            healthBarController.UpdateHearts(currentHealth, maxHealth);
        }

        public void IncreaseMaxHealth(int amount)
        {
            maxHealth += amount;
            currentHealth += amount; // Optionally heal the player by the same amount
            // Re-initialize the health bar to accommodate the new max health
            healthBarController.InitializeHearts(maxHealth);
            healthBarController.UpdateHearts(currentHealth, maxHealth);
            Debug.Log("Max health increased by " + amount + ". New max health: " + maxHealth);
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
        }
    }
}