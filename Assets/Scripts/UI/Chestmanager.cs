using UnityEngine;
using Scripts.Healths;
using Scripts.Combats.CharacterCombats;

public class ChestManager : MonoBehaviour
{
    public GameObject heartPrefab;
    public Transform spawnPoint;
    private int enemyCount;
    private bool isTriggered;

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
            SpawnHeart();
        }
    }

    private void SpawnHeart()
    {
        if (isTriggered) return;
        isTriggered = true;

        Debug.Log("All enemies dead - spawning heart");
        if (heartPrefab != null && spawnPoint != null)
        {
            Instantiate(heartPrefab, spawnPoint.position, spawnPoint.rotation);
        }

        Destroy(gameObject);
    }
}