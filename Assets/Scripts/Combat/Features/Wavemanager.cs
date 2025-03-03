using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
            public string waveName = "Wave";
            public List<EnemySpawnInfo> enemies;
            public float spawnInterval = 1f;
            public List<ItemDropInfo> itemDrops;
            public bool onlyOneItemMax = true;
            public Transform rewardSpawnPoint;
            public bool mustDropReward = false;
            public bool requireAllEnemiesDeadForReward = true;
            public bool waitForAllEnemiesDeadBeforeNextWave = true;
            [HideInInspector]
            public bool rewardDropped = false;
        }

        [Header("Wave Settings")]
        public Wave[] waves;
        public Transform[] spawnPoints;
        public float timeBetweenWaves = 5f;
        public bool autoStart = true;

        [Header("Debug")]
        [SerializeField] private int currentWaveIndex = 0;
        [SerializeField] private int totalEnemiesSpawned = 0;
        [SerializeField] private int totalEnemiesKilled = 0;

        private bool isSpawning = false;
        private List<int> usedSpawnPoints = new List<int>();
        private List<GameObject> activeEnemies = new List<GameObject>();
        public LayerMask wallLayerMask;

        private void OnEnable()
        {
            Health.OnEnemyDied += HandleEnemyDeath;
        }

        private void OnDisable()
        {
            Health.OnEnemyDied -= HandleEnemyDeath;
        }

        private void Start()
        {
            if (autoStart)
            {
                StartNextWave();
            }
        }

private void HandleEnemyDeath()
{
    totalEnemiesKilled++;
    int beforeCount = activeEnemies.Count;

    // Find and remove dead enemies from our active list more aggressively
    for (int i = activeEnemies.Count - 1; i >= 0; i--)
    {
        if (activeEnemies[i] == null ||
            !activeEnemies[i].activeInHierarchy ||
            (activeEnemies[i].GetComponent<Health>() != null &&
             activeEnemies[i].GetComponent<Health>().currentHealth <= 0))
        {
            activeEnemies.RemoveAt(i);
        }
    }

    Debug.Log($"Enemy died! Total killed: {totalEnemiesKilled}, Active before: {beforeCount}, Active after: {activeEnemies.Count}");

    // Only proceed if there are no active enemies left
    if (activeEnemies.Count == 0 && currentWaveIndex > 0)
    {
        int waveIndex = currentWaveIndex - 1;
        if (waveIndex < waves.Length)
        {
            Wave currentWave = waves[waveIndex];

            // Drop rewards if not already dropped
            if (!currentWave.rewardDropped && currentWave.requireAllEnemiesDeadForReward)
            {
                Debug.Log($"All enemies dead for wave {waveIndex}, dropping rewards for {currentWave.waveName}");
                TryDropItems(currentWave);
            }

            // Start next wave if not already spawning
            if (currentWave.waitForAllEnemiesDeadBeforeNextWave && !isSpawning && currentWaveIndex < waves.Length)
            {
                Debug.Log($"Scheduling next wave to start after {timeBetweenWaves} seconds");
                StartCoroutine(DelayNextWave(timeBetweenWaves));
            }
        }
    }
}

 private IEnumerator DelayNextWave(float delay)
 {
     yield return new WaitForSeconds(delay);
     StartNextWave();
 }


       public void StartNextWave()
       {
           if (currentWaveIndex < waves.Length && !isSpawning)
           {
               // Calculate total enemies in this wave
               int totalEnemiesInWave = 0;
               foreach (var enemyInfo in waves[currentWaveIndex].enemies)
               {
                   totalEnemiesInWave += enemyInfo.count;
               }

               Debug.Log($"Starting wave {currentWaveIndex + 1}: {waves[currentWaveIndex].waveName} with {totalEnemiesInWave} enemies");
               StartCoroutine(SpawnWave(waves[currentWaveIndex]));
               currentWaveIndex++;
           }
           else
           {
               Debug.Log("All waves completed!");
           }
       }

       private IEnumerator SpawnWave(Wave wave)
       {
           Debug.Log($"Starting {wave.waveName}");
           isSpawning = true;
           int waveEnemiesSpawned = 0;
           int expectedEnemies = 0;

           foreach (var enemyInfo in wave.enemies)
           {
               expectedEnemies += enemyInfo.count;
           }

           foreach (var enemyInfo in wave.enemies)
           {
               for (int i = 0; i < enemyInfo.count; i++)
               {
                   SpawnEnemy(enemyInfo.enemyPrefab);
                   waveEnemiesSpawned++;
                   totalEnemiesSpawned++;
                   Debug.Log($"Wave progress: {waveEnemiesSpawned}/{expectedEnemies} enemies spawned");
                   yield return new WaitForSeconds(wave.spawnInterval);
               }
           }

           Debug.Log($"Wave complete: All {expectedEnemies} enemies spawned. Total active: {activeEnemies.Count}");
           isSpawning = false;

           // If we don't need to wait for enemies to be dead before dropping rewards
           if (!wave.requireAllEnemiesDeadForReward)
           {
               TryDropItems(wave);
           }

           // If we don't need to wait for all enemies to be dead before starting next wave
           if (!wave.waitForAllEnemiesDeadBeforeNextWave)
           {
               yield return new WaitForSeconds(timeBetweenWaves);
               if (currentWaveIndex < waves.Length)
               {
                   StartNextWave();
               }
           }
       }

        private void SpawnEnemy(GameObject enemyPrefab)
        {
            if (spawnPoints.Length == 0)
            {
                Debug.LogError("No spawn points defined for wave manager!");
                return;
            }

            if (usedSpawnPoints.Count >= spawnPoints.Length)
            {
                usedSpawnPoints.Clear();
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
            if (wallLayerMask == 0 || Physics2D.OverlapCircle(spawnPosition, 0.5f, wallLayerMask) == null)
            {
                GameObject enemy = Instantiate(enemyPrefab, spawnPosition, spawnPoint.rotation);
                activeEnemies.Add(enemy);
                Debug.Log($"Spawned enemy at {spawnPosition}");
            }
            else
            {
                Debug.LogWarning("Spawn position is inside a wall, trying again.");
                SpawnEnemy(enemyPrefab);
            }
        }

        private void TryDropItems(Wave wave)
        {
            if (wave.rewardDropped || wave.itemDrops == null || wave.itemDrops.Count == 0)
            {
                Debug.Log("No rewards to drop or already dropped");
                return;
            }

            Debug.Log("Attempting to drop rewards");
            bool itemDropped = false;

            // Try to drop items based on chance
            foreach (var itemDrop in wave.itemDrops)
            {
                float randomValue = Random.Range(0f, 100f);
                if (randomValue <= itemDrop.dropChance)
                {
                    Transform spawnPoint = GetRewardSpawnPoint(wave);
                    GameObject droppedItem = Instantiate(itemDrop.itemPrefab, spawnPoint.position, spawnPoint.rotation);
                    Debug.Log($"Dropped item: {itemDrop.itemPrefab.name} at {spawnPoint.position}");

                    wave.rewardDropped = true;
                    itemDropped = true;

                    if (wave.onlyOneItemMax)
                    {
                        break;
                    }
                }
            }

            // Force drop if required and nothing dropped yet
            if (wave.mustDropReward && !itemDropped && wave.itemDrops.Count > 0)
            {
                var guaranteedDrop = wave.itemDrops[Random.Range(0, wave.itemDrops.Count)];
                Transform spawnPoint = GetRewardSpawnPoint(wave);
                GameObject droppedItem = Instantiate(guaranteedDrop.itemPrefab, spawnPoint.position, spawnPoint.rotation);
                Debug.Log($"Dropped guaranteed item: {guaranteedDrop.itemPrefab.name} at {spawnPoint.position}");
                wave.rewardDropped = true;
            }
        }

        private Transform GetRewardSpawnPoint(Wave wave)
        {
            if (wave.rewardSpawnPoint != null)
                return wave.rewardSpawnPoint;

            if (spawnPoints != null && spawnPoints.Length > 0)
                return spawnPoints[Random.Range(0, spawnPoints.Length)];

            return transform; // Fallback to the WaveManager's position
        }

        // Draw Gizmos to visualize spawn points in the editor
        private void OnDrawGizmos()
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