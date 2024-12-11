using UnityEngine;
using System.Collections;

namespace Scripts.Health {
    public class EnemyHealth : Health {

        protected override void Start()
        {
            // Rufe die gemeinsame Initialisierung der Basisklasse auf
            base.Start();
        }

        protected override void initializeHealthBar(int maxHealth)
        {
            // Initialisiere die Health Bar
            // Hier kannst du die Health Bar des Gegners initialisieren
        }

        protected override void updateHealthBar(int currentHealth, int maxHealth)
        {
            // Update die Health Bar
            // Hier kannst du die Health Bar des Gegners aktualisieren
        }

        protected override void Die()
        {
             Debug.Log("Enemy died.");

            // Play Death Animation
            animator.SetBool("IsDead", true);

            // Disable the enemy
            GetComponent<Collider2D>().enabled = false;
            this.enabled = false;

            Destroy(gameObject, 3f);
        }

        public override void TakeDamage(int damage, Vector2 hitDirection)
        {
            base.TakeDamage(damage, hitDirection);
            // Update die Health Bar
            // Hier kannst du die Health Bar des Gegners aktualisieren
        }
    } 

}
