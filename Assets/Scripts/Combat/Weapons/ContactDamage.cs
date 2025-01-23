using UnityEngine;
using System.Collections;
using Scripts.Movements.AI;
using Scripts.Healths;

namespace Scripts.Combats.Weapons
{
    public class ContactDamage : Weapon
    {

        public override void PerformAttack()
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

            if (characterMovement == null)
            {
                Debug.LogError("Player direction is not assigned.");
                return;
            }

            // Detect enemies in range
            Collider2D[] hitEnemies = Physics2D.OverlapCapsuleAll(attackPoint.position, new Vector2(attackRange, attackRange), CapsuleDirection2D.Vertical, 0, enemyLayer);

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

        }
    }
}