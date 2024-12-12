using UnityEngine;
using System.Collections;
using Scripts.Movements;
using Scripts.Healths;

namespace Scripts.Combats {
    public abstract class Combat : MonoBehaviour
    {
        public Animator animator;
        public LayerMask enemyLayers;

        public Rigidbody2D rb;

        protected PlayerMovement playerDirection; // Referenz auf den PlayerController
        public Transform attackPoint; // Referenzpunkt für den Angriff

        public float attackRange = 0.5f; // Radius des Angriffsbereichs
        public int attackDamage = 40;
        public float attackRate = 2f;
        protected float nextAttackTime = 0f;

        public bool ifDebug = false;
        public GameObject debugAttackRangeVisualizer; // Referenz für das Visualisierungssprite^

        protected virtual void Start()
        {
            rb = GetComponent<Rigidbody2D>();

            if (rb == null)
            {
                Debug.LogError("Rigidbody2D component not found on " + gameObject.name);
            }

            // Get Referenz zum PlayerController
            playerDirection = getCharacterDirection();
            attackPoint = transform.Find("AttackPoint");

            // Initialisiere das Attack-Visualizer-Sprite (falls zugewiesen)
            if (debugAttackRangeVisualizer != null)
            {
                UpdateAttackRangeVisualizer(); // Größe anpassen
                debugAttackRangeVisualizer.SetActive(false); // Zunächst ausblenden
            }
        }

        abstract protected PlayerMovement getCharacterDirection();


        // Function to attack enemies in range
        protected void Attack()
        {
            // Überprüfe, ob attackPoint nicht null ist
            if (attackPoint == null)
            {
                Debug.LogError("Attack point is not assigned.");
                return;
            }

            // Überprüfe, ob animator und playerDirection nicht null sind
            if (animator == null)
            {
                Debug.LogError("Animator is not assigned.");
                return;
            }

            if (playerDirection == null)
            {
                Debug.LogError("Player direction is not assigned.");
                return;
            }

            animator.SetFloat("StayHorizontal", playerDirection.lastMoveDirection.x);
            animator.SetFloat("StayVertical", playerDirection.lastMoveDirection.y);
            // Play an Attack Animation
            animator.SetTrigger("Attack");
            // Detect enemies in range
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

            if (hitEnemies.Length == 0)
            {
                Debug.Log("No enemies in range");
                return;
            }

            // Deal damage to enemies
            foreach (Collider2D enemy in hitEnemies)
            {
                Health enemyHealth = enemy.GetComponent<Health>();
                if (enemyHealth != null)
                {
                    Vector2 hitDirection = enemy.transform.position - transform.position;
                    enemyHealth.TakeDamage(attackDamage, hitDirection);
                }
                else
                {
                    Debug.Log("Enemy script not found");
                }
            }

            Debug.Log("Attacking");
        }

        // Draw a gizmo to show the attack range
        protected void OnDrawGizmosSelected()
        {
            if (attackPoint == null)
                return;

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }

        // Aktualisiert die Größe des Visualisierungs-Sprites basierend auf attackRange
        protected void UpdateAttackRangeVisualizer()
        {
            if (debugAttackRangeVisualizer != null)
            {
                float scale = attackRange * 2; // attackRange repräsentiert den Radius, daher Durchmesser * 2
                debugAttackRangeVisualizer.transform.localScale = new Vector3(scale, scale, 1);
            }
        }
    }
}