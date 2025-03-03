using UnityEngine;
using System.Collections;
using Scripts.Movements.Behaviours;
using UnityEngine.UIElements;
using Scripts.Combats.CharacterCombats;
using System.Collections.Generic;

namespace Scripts.Healths
{
    [System.Serializable]
    public class DropReward
    {
        public GameObject rewardPrefab;
        public float dropChance = 0.3f; // 30% chance by default
        public string rewardName; // For easier identification in inspector
    }

    public class EnemyHealth : Health
    {
        [SerializeField] private DropConfiguration dropConfig;
        [SerializeField] private bool useLocalDropSettings = false;
        [SerializeField] private List<DropReward> localPossibleRewards = new List<DropReward>();
        [SerializeField] private float localNothingDropChance = 0.3f;

        private EnemyCombat enemyCombat;

        protected override void Start()
        {
            // Call the base class initialization
            base.Start();
            Debug.Log("Enemy " + gameObject.name + " initialized with health: " + currentHealth);
            enemyCombat = GetComponentInChildren<EnemyCombat>();
            if (enemyCombat == null)
            {
                Debug.LogError("EnemyCombat-Komponente nicht gefunden auf " + gameObject.name);
            }
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
            DropReward();

            Destroy(gameObject, 0.5f);
        }

        protected override void Hurt()
        {
            // Play hurt animation
            if (enemyCombat != null)
            {
                enemyCombat.CancelAttack();
            }
            characterAnimation.SetIsHurt(true);
        }

        public override void TakeDamage(int damage, Vector2 hitDirection, float knockbackForce, float knockbackDuration)
        {
            Debug.Log("Enemy " + gameObject.name + " took damage: " + damage);
            // Reduce health
            base.TakeDamage(damage, hitDirection, knockbackForce, knockbackDuration);
        }

        private void DropReward()
        {
            // Determine which configuration to use
            List<DropReward> rewardList;
            float nothing;

            if (useLocalDropSettings || dropConfig == null)
            {
                rewardList = localPossibleRewards;
                nothing = localNothingDropChance;

                if (dropConfig == null && !useLocalDropSettings)
                {
                    Debug.LogWarning("No drop configuration assigned to enemy: " + gameObject.name + ". Using local settings.");
                }
            }
            else
            {
                rewardList = dropConfig.possibleRewards;
                nothing = dropConfig.nothingDropChance;
            }

            // Check if we should drop anything at all
            if (Random.value < nothing)
            {
                Debug.Log("No reward dropped due to nothingDropChance");
                return;
            }

            if (rewardList.Count == 0)
            {
                Debug.Log("No rewards configured for enemy: " + gameObject.name);
                return;
            }

            // Calculate total weight/chances of all rewards
            float totalChance = 0f;
            foreach (var reward in rewardList)
            {
                if (reward.rewardPrefab != null)
                {
                    totalChance += reward.dropChance;
                }
            }

            if (totalChance <= 0f)
            {
                Debug.LogWarning("Total drop chance is 0 or negative for enemy: " + gameObject.name);
                return;
            }

            // Choose a random value between 0 and the total chance
            float randomValue = Random.Range(0f, totalChance);
            float cumulativeChance = 0f;

            // Find which reward to drop based on the random value
            foreach (var reward in rewardList)
            {
                if (reward.rewardPrefab == null) continue;

                cumulativeChance += reward.dropChance;

                if (randomValue <= cumulativeChance)
                {
                    try
                    {
                        Instantiate(reward.rewardPrefab, transform.position, Quaternion.identity);
                        Debug.Log($"Dropped reward: {reward.rewardName}");
                        return; // Only drop one reward
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError($"Error dropping reward {reward.rewardName}: {e.Message}");
                    }
                    break;
                }
            }
        }
    }
}