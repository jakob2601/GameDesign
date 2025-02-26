using UnityEngine;
using System.Collections;
using Scripts.Movements.Behaviours;
using UnityEngine.UIElements;

namespace Scripts.Healths {
    public class EnemyHealth : Health {

        [SerializeField] private GameObject HeartPickup; // Referenz zum Herz-Prefab
        [SerializeField] private float dropChance = 1f; // 30% Chance, ein Herz zu droppen

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
            characterAnimation.SetIsDead(true);
            // Disable the enemy
            GetComponent<Collider2D>().enabled = false;
            this.enabled = false;

            // Versuche ein Herz zu droppen
            DropHeart();

            Destroy(gameObject, 0.5f);
        }

        protected override void Hurt()
        {
            // Play Hurt Animation
            characterAnimation.SetIsHurt(true);
            
        }

        public override void TakeDamage(int damage, Vector2 hitDirection, float knockbackForce, float knockbackDuration)
        {
            // Reduziere die Gesundheit
            // Update die Health Bar
            // Hier kannst du die Health Bar des Gegners aktualisieren
            base.TakeDamage(damage, hitDirection, knockbackForce, knockbackDuration);
        }

        private void DropHeart()
        {
            // Improved null check
            if (HeartPickup == null)
            {
                Debug.LogError("HeartPickup ist nicht zugewiesen für Enemy: " + gameObject.name);
                return; // This should prevent the error
            }

            float randomValue = Random.value;
            Debug.Log("Drop Chance: " + randomValue + " (Muss kleiner als " + dropChance + " sein)");

            if (randomValue <= dropChance)
            {
                try {
                    Instantiate(HeartPickup, transform.position, Quaternion.identity);
                    Debug.Log("Heart dropped!");
                }
                catch (System.Exception e) {
                    Debug.LogError("Error dropping heart: " + e.Message);
                }
            }
        }
    } 

}
