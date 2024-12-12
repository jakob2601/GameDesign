using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Scripts.Movements
{
    public class EnemyAI : MonoBehaviour
    {
        private EnemyMovement enemyMovement;
        public float detectionRange = 100f; // Reichweite, in der der Gegner den Spieler sehen kann
        private Transform player;
        public LayerMask playerLayer;
        float distanceToPlayer;
        Collider2D playerCollider;

        // Start is called before the first frame update
        void Start()
        {
            enemyMovement = GetComponent<EnemyMovement>();
            if (enemyMovement == null)
            {
                Debug.LogError("EnemyMovement component not found on " + gameObject.name);
            }
        }

        void FixedUpdate()
        {
            // Finde den Spieler basierend auf der Layer
            playerCollider = Physics2D.OverlapCircle(transform.position, detectionRange, playerLayer);
            if (playerCollider != null)
            {
                player = playerCollider.transform;
                distanceToPlayer = Vector2.Distance(transform.position, player.position);

                if (distanceToPlayer <= detectionRange)
                {
                    // Wenn der Spieler in Reichweite ist, bewegt sich der Gegner auf ihn zu
                    enemyMovement.target = player;
                    enemyMovement.enabled = true; // Aktiviert die Bewegung
                }
                else
                {
                    // Wenn der Spieler nicht in Reichweite ist, kann der Gegner andere Aktionen ausführen
                    enemyMovement.target = null; // Oder setzen Sie ein anderes Ziel
                    enemyMovement.enabled = false; // Deaktiviert die Bewegung
                }
            }
            else
            {
                Debug.Log("Player not found in the area.");
                enemyMovement.enabled = false; // Deaktiviert die Bewegung
            }
        }

        void OnDrawGizmosSelected()
        {
            // Zeichne den Erkennungsbereich im Editor
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, detectionRange);
        }
    }
}
