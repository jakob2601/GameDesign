using UnityEngine;
using System.Collections;
using Scripts.Movements.AI;
using Scripts.Healths;
using MyGame;

namespace Scripts.Combats.Weapons
{
    public class Sword : Weapon
    {
        [SerializeField] float angleThreshold = 0.7f; // Entspricht ca. 45 Grad

        protected override void Start()
        {
            base.Start();
        }

        protected override void Update()
        {
            base.Update();
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        public void CheckSwordAttackHitBox() 
        {
            if (attackPoint == null)
            {
                Debug.LogError("Attack point is not assigned.");
                return;
            }

            if (characterMovement == null)
            {
                Debug.LogError("Animator or player direction is not assigned.");
                return;
            }

            Vector2 attackPosition = (Vector2)transform.position + (Vector2.up * -0.3f) + characterMovement.lastMoveDirection.normalized * attackRange;

            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPosition, attackRange, enemyLayer);

            foreach (Collider2D enemy in hitEnemies)
            {
                // NEU: Winkelprüfung mit engerem Schwellenwert (z. B. 45 Grad)
                //if (Vector2.Dot(characterMovement.lastMoveDirection.normalized, hitDirection.normalized) >= angleThreshold)
                {
                    Vector2 hitDirection = (Vector2)(enemy.transform.position - transform.position);
                    Health enemyHealth = enemy.GetComponent<Health>();
                    if (enemyHealth != null)
                    {
                        enemyHealth.TakeDamage(attackDamage, hitDirection, knockbackForce, knockbackDuration);
                    }
                    else
                    {
                        Debug.Log("Enemy Health Component not found");
                    }
                    // Instantiate hit particle
                }
            }
        }


        private void OnDrawGizmos()
        {
            if (attackPoint == null)
                return;

            // Zeigt den aktuellen Angriffspunkt
            //Gizmos.color = Color.red;
            //Gizmos.DrawWireSphere(attackPoint.position, attackRange);

            // Optional: Weltkoordinaten basierter Angriffspunkt
            if (Application.isPlaying) // Nur zur Laufzeit anzeigen
            {
                Vector2 attackPosition = (Vector2)transform.position + (Vector2.up * -0.3f) + characterMovement.lastMoveDirection.normalized * attackRange;
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(attackPosition, attackRange);


            }
        }
    }
}
