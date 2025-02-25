using System.Collections;
using Scripts.UI;
using UnityEngine;
using Scripts.Combats.CharacterCombats;
using Scripts.Combats.Weapons;
using System.Threading;
using UnityEngine.SceneManagement;

namespace Scripts.Healths
{
    public class PlayerHealth : Health
    {   
        [SerializeField] public HealthBarController healthBarController;


        protected override void Start()
        {
            // Rufe die gemeinsame Initialisierung der Basisklasse auf
            base.Start();
        }

        public override void TakeDamage(int damage, Vector2 hitDirection, float knockbackForce, float knockbackDuration)
        {
            base.TakeDamage(damage, hitDirection, knockbackForce, knockbackDuration);
        }
    
        protected override void initializeHealthBar(int maxHealth)
        {
            // Initialisiere die Health Bar
            healthBarController.InitializeHearts(maxHealth);
        }

        protected override void updateHealthBar(int currentHealth, int maxHealth)
        {
            // Update die Health Bar
            healthBarController.UpdateHearts(currentHealth, maxHealth);
        }

        public override void Heal(int amount)
        {
            base.Heal(amount);
            // Update die Health Bar
            healthBarController.UpdateHearts(currentHealth, maxHealth);
          
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
            SceneManager.LoadScene("GameOverMenu"); // Lade die GameOver
        }
    }

}
