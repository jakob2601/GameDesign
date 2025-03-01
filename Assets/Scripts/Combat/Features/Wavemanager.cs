using System.Collections;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public int enemyCount;
        public float spawnInterval;
    }

    public Wave[] waves;
    public Transform[] spawnPoints;
    public GameObject enemyPrefab;
    private int currentWaveIndex = 0;
    private bool isSpawning = false;

    void Start()
    {
        StartCoroutine(StartNextWave());
    }

    IEnumerator StartNextWave()
    {
        if (currentWaveIndex < waves.Length)
        {
            Wave currentWave = waves[currentWaveIndex];
            isSpawning = true;

            for (int i = 0; i < currentWave.enemyCount; i++)
            {
                SpawnEnemy();
                yield return new WaitForSeconds(currentWave.spawnInterval);
            }

            isSpawning = false;
            currentWaveIndex++;
        }
    }

    void SpawnEnemy()
    {
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
    }

    void Update()
    {
        if (!isSpawning && currentWaveIndex < waves.Length)
        {
            StartCoroutine(StartNextWave());
        }
    }
}