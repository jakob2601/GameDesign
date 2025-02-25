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

        public void CheckAttackHitBox() 
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

        
        /*
        public override void PerformAttack()
        {
            base.PerformAttack();

            if (attackPoint == null)
            {
                Debug.LogError("Attack point is not assigned.");
                return;
            }

            if (animator == null || characterMovement == null)
            {
                Debug.LogError("Animator or player direction is not assigned.");
                return;
            }

            // Set animation parameters
            animator.SetFloat("StayHorizontal", characterMovement.lastMoveDirection.x);
            animator.SetFloat("StayVertical", characterMovement.lastMoveDirection.y);

            // NEU: Berechne die Angriffsposition in Weltkoordinaten (statt lokale Position)
            Vector2 attackPosition = (Vector2)transform.position + (Vector2.up * -0.3f) + characterMovement.lastMoveDirection.normalized * attackRange;

            // Play attack animation
            animator.SetTrigger("Attack");
            SoundManager.PlaySound(SoundType.SWING);

            // NEU: Verwende die Angriffsposition statt attackPoint.position
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPosition, attackRange, enemyLayer);

            if (hitEnemies.Length == 0)
            {
                // Debug.Log("No enemies in range");
                return;
            }

            // Damage enemies only in the direction the character is facing
            foreach (Collider2D enemy in hitEnemies)
            {
                Vector2 hitDirection = (Vector2)(enemy.transform.position - transform.position);

                // NEU: Winkelprüfung mit engerem Schwellenwert (z. B. 45 Grad)
                //if (Vector2.Dot(characterMovement.lastMoveDirection.normalized, hitDirection.normalized) >= angleThreshold)
                {
                    // Should be in EnemyHealth
                    Health enemyHealth = enemy.GetComponent<Health>();
                    if (enemyHealth != null)
                    {
                        enemyHealth.TakeDamage(attackDamage, hitDirection, knockbackForce, knockbackDuration);
                        SoundManager.PlaySound(SoundType.HIT);
                        SoundManager.PlaySound(SoundType.HURT);
                    }
                    else
                    {
                        Debug.Log("Enemy script not found");
                    }
                }
            }

            Debug.Log("Attacking");
        }
        */

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
