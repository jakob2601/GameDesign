using UnityEngine;
using Scripts.Healths;
using Scripts.Combats.CharacterCombats;

public class ChestManager : MonoBehaviour
{
    public GameObject heartPrefab;
    public GameObject bowPrefab;
    public Transform spawnPoint;
    private int enemyCount;
    private bool isTriggered;
    private bool arrowSpawned;

    void Start()
    {
        // Count initial enemies
        enemyCount = FindObjectsOfType<EnemyCombat>().Length;
        Debug.Log($"Found {enemyCount} enemies at start");

        // Subscribe to the enemy death event
        Health.OnEnemyDied += HandleEnemyDeath;
    }

    void OnDestroy()
    {
        Health.OnEnemyDied -= HandleEnemyDeath;
    }

    private void HandleEnemyDeath()
    {
        if (isTriggered) return;

        enemyCount--;
        Debug.Log($"Enemy died. Remaining: {enemyCount}");

        if (enemyCount <= 0)
        {
            SpawnReward();
        }
    }

    private void SpawnReward()
    {
        if (isTriggered) return;
        isTriggered = true;

        Debug.Log("All enemies dead - spawning reward");

        if (spawnPoint != null)
        {
            if (!arrowSpawned)
            {
                if (bowPrefab != null)
                {
                    Instantiate(bowPrefab, spawnPoint.position, spawnPoint.rotation);
                    arrowSpawned = true;
                }
            }
            else
            {
                if (heartPrefab != null)
                {
                    Instantiate(heartPrefab, spawnPoint.position, spawnPoint.rotation);
                }
            }
        }

        Destroy(gameObject);
    }
}