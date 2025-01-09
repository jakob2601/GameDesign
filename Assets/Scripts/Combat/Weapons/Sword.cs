using UnityEngine;
using System.Collections;
using Scripts.Movements.AI;
using Scripts.Healths;
using MyGame;

namespace Scripts.Combats.Weapons
{
    public class Sword : Weapon {
        public override void PerformAttack(MovementAI characterDirection)
        {
            base.Start();
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

            if (characterDirection == null)
            {
                Debug.LogError("Player direction is not assigned.");
                return;
            }

            animator.SetFloat("StayHorizontal", characterDirection.lastMoveDirection.x);
            animator.SetFloat("StayVertical", characterDirection.lastMoveDirection.y);
            // Play an Attack Animation
            animator.SetTrigger("Attack");
            SoundManager.PlaySound(SoundType.SWING);
            // Detect enemies in range
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);

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
                    enemyHealth.TakeDamage(attackDamage, hitDirection, knockbackForce, knockbackDuration);
                }
                else
                {
                    Debug.Log("Enemy script not found");
                }
            }

            Debug.Log("Attacking");
        }
    }
}