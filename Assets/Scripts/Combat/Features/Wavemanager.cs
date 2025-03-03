using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scripts.Combats.Features;
using Scripts.Healths;

namespace Scripts.Combats.Features
{
    public class WaveManager : MonoBehaviour
    {
        [System.Serializable]
        public class EnemySpawnInfo
        {
            public GameObject enemyPrefab;
            public int count;
        }

        [System.Serializable]
        public class ItemDropInfo
        {
            public GameObject itemPrefab;
            [Range(0, 100)]
            public float dropChance; // Percentage chance to drop the item
        }

        [System.Serializable]
        public class Wave
        {
            public List<EnemySpawnInfo> enemies;
            public float spawnInterval;
            public List<ItemDropInfo> itemDrops; // List of item drops for this wave
            public bool onlyOneItemMax; // Toggle to control if only one item can spawn
            public Transform rewardSpawnPoint; // Specific point where the reward spawns
            public bool mustDropReward; // Toggle to control if a reward must be dropped
            public bool spawnRewardAfterAllEnemiesDead; // Toggle to control if reward spawns after all enemies are dead
            [HideInInspector]
            public bool rewardDropped; // Flag to track if a reward has been dropped
        }

        public Wave[] waves;
        public Transform[] spawnPoints;
        public float timeBetweenWaves = 5f; // Time between waves
        private int currentWaveIndex = 0;
        private bool isSpawning = false;
        private List<int> usedSpawnPoints = new List<int>();
        public LayerMask wallLayerMask; // Layer mask to identify walls
        private List<GameObject> activeEnemies = new List<GameObject>(); // List to keep track of active enemies

        void Start()
        {
            StartCoroutine(StartNextWave());
        }

        IEnumerator StartNextWave()
        {
            while (currentWaveIndex < waves.Length)
            {
                Wave currentWave = waves[currentWaveIndex];
                isSpawning = true;

                foreach (var enemyInfo in currentWave.enemies)
                {
                    for (int i = 0; i < enemyInfo.count; i++)
                    {
                        SpawnEnemy(enemyInfo.enemyPrefab);
                        yield return new WaitForSeconds(currentWave.spawnInterval);
                    }
                }

                isSpawning = false;

                // Wait for the specified time between waves
                yield return new WaitForSeconds(timeBetweenWaves);

                // Drop items after wave
                if (currentWave.spawnRewardAfterAllEnemiesDead)
                {
                    yield return new WaitUntil(() => activeEnemies.Count == 0); // Wait until all enemies are defeated
                }
                TryDropItems(currentWave);

                currentWaveIndex++;
            }
        }

        void SpawnEnemy(GameObject enemyPrefab)
        {
            if (usedSpawnPoints.Count >= spawnPoints.Length)
            {
                usedSpawnPoints.Clear(); // Reset the used spawn points if all have been used
            }

            int spawnIndex;
            do
            {
                spawnIndex = Random.Range(0, spawnPoints.Length);
            } while (usedSpawnPoints.Contains(spawnIndex));

            usedSpawnPoints.Add(spawnIndex);

            Transform spawnPoint = spawnPoints[spawnIndex];

            // Calculate random offset within a circle of radius 2 units
            Vector2 randomOffset = Random.insideUnitCircle * 2f;
            Vector3 spawnPosition = spawnPoint.position + new Vector3(randomOffset.x, randomOffset.y, 0);

            // Check if the spawn position is inside a wall
            if (Physics2D.OverlapCircle(spawnPosition, 0.5f, wallLayerMask) == null)
            {
                GameObject enemy = Instantiate(enemyPrefab, spawnPosition, spawnPoint.rotation);
                activeEnemies.Add(enemy); // Add enemy to the active enemies list
                Health.OnEnemyDied += () => RemoveEnemy(enemy); // Subscribe to the enemy's death event
            }
            else
            {
                Debug.LogWarning("Spawn position is inside a wall, skipping spawn.");
            }
        }

        void RemoveEnemy(GameObject enemy)
        {
            activeEnemies.Remove(enemy); // Remove enemy from the active enemies list
        }

void TryDropItems(Wave wave)
{
    if (wave.rewardDropped)
    {
        return; // Do not drop a reward if one has already been dropped
    }

    // Check if all enemies on the map are dead
    if (activeEnemies.Count > 0)
    {
        Debug.Log("Cannot drop items, enemies are still alive.");
        return;
    }

    bool itemDropped = false;

    foreach (var itemDrop in wave.itemDrops)
    {
        float randomValue = Random.Range(0f, 100f);
        if (randomValue <= itemDrop.dropChance)
        {
            // Drop the item at the specified reward spawn point
            Transform spawnPoint = wave.rewardSpawnPoint != null ? wave.rewardSpawnPoint : spawnPoints[Random.Range(0, spawnPoints.Length)];
            Instantiate(itemDrop.itemPrefab, spawnPoint.position, spawnPoint.rotation);

            Debug.Log($"Dropped item: {itemDrop.itemPrefab.name} at {spawnPoint.position}");

            wave.rewardDropped = true; // Mark that a reward has been dropped

            if (wave.onlyOneItemMax)
            {
                itemDropped = true;
                break;
            }
        }
    }

    if (wave.mustDropReward && !itemDropped)
    {
        // Ensure at least one item is dropped
        var guaranteedDrop = wave.itemDrops[Random.Range(0, wave.itemDrops.Count)];
        Transform spawnPoint = wave.rewardSpawnPoint != null ? wave.rewardSpawnPoint : spawnPoints[Random.Range(0, spawnPoints.Length)];
        Instantiate(guaranteedDrop.itemPrefab, spawnPoint.position, spawnPoint.rotation);

        Debug.Log($"Dropped guaranteed item: {guaranteedDrop.itemPrefab.name} at {spawnPoint.position}");

        wave.rewardDropped = true; // Mark that a reward has been dropped
    }
}
        void Update()
        {
            if (!isSpawning && currentWaveIndex < waves.Length)
            {
                StartCoroutine(StartNextWave());
            }
        }

        // Draw Gizmos to visualize spawn points in the editor
        void OnDrawGizmos()
        {
            if (spawnPoints == null) return;

            Gizmos.color = Color.red;
            foreach (Transform spawnPoint in spawnPoints)
            {
                if (spawnPoint != null)
                {
                    Gizmos.DrawSphere(spawnPoint.position, 0.5f);
                }
            }

            // Draw Gizmos for reward spawn points
            Gizmos.color = Color.blue;
            foreach (Wave wave in waves)
            {
                if (wave.rewardSpawnPoint != null)
                {
                    Gizmos.DrawSphere(wave.rewardSpawnPoint.position, 0.5f);
                }
            }
        }
    }
}