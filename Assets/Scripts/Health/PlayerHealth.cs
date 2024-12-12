using System.Collections;
using Scripts.UI;
using UnityEngine;
using Scripts.Combats;

namespace Scripts.Healths
{
    public class PlayerHealth : Health
    {
        public new int maxHealth = 5;
        
        public HealthBarController healthBarController;


        protected override void Start()
        {
            // Rufe die gemeinsame Initialisierung der Basisklasse auf
            base.Start();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            //Initialisieren Collision zu Gegner
            EnemyCombat enemy = collision.GetComponent<EnemyCombat>();

            if (enemy)
            {
                Vector2 hitDirection = (transform.position - enemy.transform.position).normalized;
                TakeDamage(enemy.attackDamage, hitDirection);
            }
        }

        public override void TakeDamage(int damage, Vector2 hitDirection)
        {
            base.TakeDamage(damage, hitDirection);
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

        protected override void Die()
        {
            Debug.Log("Player has died!");
            // Hier kannst du eine Logik für den Tod des Spielers einfügen
        }
    }

}
