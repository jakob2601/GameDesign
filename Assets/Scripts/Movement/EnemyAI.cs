using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Scripts.Movements
{
    public class EnemyAI : MonoBehaviour
    {
        private EnemyMovement enemyMovement;
        public float detectionRange = 10f; // Reichweite, in der der Gegner den Spieler sehen kann
        private Transform player;

        // Start is called before the first frame update
        void Start()
        {
            enemyMovement = GetComponent<EnemyMovement>();
            if (enemyMovement == null)
            {
                Debug.LogError("EnemyMovement component not found on " + gameObject.name);
            }
            player = GameObject.FindGameObjectWithTag("Player").transform;
            if (player == null)
            {
                Debug.LogError("Player not found in the scene.");
            }
        }

        void FixedUpdate()
        {
            // Hier können Sie die Logik für die Entscheidungen des Gegners implementieren
            // Zum Beispiel: Wenn der Gegner den Spieler sieht, bewegt er sich auf ihn zu
            if (player != null && enemyMovement != null)
            {
                float distanceToPlayer = Vector2.Distance(transform.position, player.position);

                if (distanceToPlayer <= detectionRange)
                {
                    // Wenn der Spieler in Reichweite ist, bewegt sich der Gegner auf ihn zu
                    enemyMovement.target = player;
                }
                else
                {
                    // Wenn der Spieler nicht in Reichweite ist, kann der Gegner andere Aktionen ausführen
                    enemyMovement.target = null; // Oder setzen Sie ein anderes Ziel
                }
            }
        }

    }
}

