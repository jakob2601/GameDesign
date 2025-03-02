using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        public class Wave
        {
            public List<EnemySpawnInfo> enemies;
            public float spawnInterval;
        }

        public Wave[] waves;
        public Transform[] spawnPoints;
        public float timeBetweenWaves = 5f; // Time between waves
        private int currentWaveIndex = 0;
        private bool isSpawning = false;
        private List<int> usedSpawnPoints = new List<int>();
        public LayerMask wallLayerMask; // Layer mask to identify walls

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
                currentWaveIndex++;

                // Wait for the specified time between waves
                yield return new WaitForSeconds(timeBetweenWaves);
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
                Instantiate(enemyPrefab, spawnPosition, spawnPoint.rotation);
            }
            else
            {
                Debug.LogWarning("Spawn position is inside a wall, skipping spawn.");
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
        }
    }
}