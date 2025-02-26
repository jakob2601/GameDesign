// Assets/Scripts/Health/EnemyHealth.cs
using UnityEngine;
using System.Collections;
using Scripts.Movements.Behaviours;
using UnityEngine.UIElements;

namespace Scripts.Healths {
    public class EnemyHealth : Health {

        [SerializeField] private GameObject HeartPickup; // Reference to the heart prefab
        [SerializeField] private float dropChance = 1f; // 30% chance to drop a heart

        protected override void Start()
        {
            // Call the base class initialization
            base.Start();
            Debug.Log("Enemy " + gameObject.name + " initialized with health: " + currentHealth);
        }

        public bool IsAlive()
        {
            Debug.Log("Checking if enemy " + gameObject.name + " is alive with health: " + currentHealth);
            return currentHealth > 0;
        }

        protected override void initializeHealthBar(int maxHealth)
        {
            // Initialize the health bar
        }

        protected override void updateHealthBar(int currentHealth, int maxHealth)
        {
            // Update the health bar
        }
        public System.Action<string> OnThisEnemyDied;
        private bool isDying = false;

        protected override void Die()
        {
        if (isDying) return;
        isDying = true;

        Debug.Log($"Enemy {gameObject.name} died.");

        // Trigger individual enemy death event
        OnThisEnemyDied?.Invoke(gameObject.name);

        // Call base Die() for general death event
        base.Die();

        characterAnimation.SetIsDead(true);
        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;
        DropHeart();

        Destroy(gameObject, 0.5f);
        }

        protected override void Hurt()
        {
            // Play hurt animation
            characterAnimation.SetIsHurt(true);
        }

        public override void TakeDamage(int damage, Vector2 hitDirection, float knockbackForce, float knockbackDuration)
        {
            Debug.Log("Enemy " + gameObject.name + " took damage: " + damage);
            // Reduce health
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
            Debug.Log("Drop Chance: " + randomValue + " (Must be less than " + dropChance + ")");

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