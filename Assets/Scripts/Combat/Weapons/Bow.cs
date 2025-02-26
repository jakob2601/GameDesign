using UnityEngine;
using Scripts.Combats.CharacterCombats;
using Scripts.Movements.AI;
using MyGame; // Add this to access Arrow class

namespace Scripts.Combats.Weapons
{
    public class Bow : Weapon
    {
        [Header("Bow Properties")]
        [SerializeField] protected GameObject arrowPrefab;
        [SerializeField] protected float bulletForce = 20f;
        [SerializeField] protected int arrowDamage = 10;
        [SerializeField] protected float arrowLifetime = 5f;
        
        public override void PerformAttack()
        {
            base.Start();
            CheckBowAttackHitBox();
        }

        public void CheckBowAttackHitBox()
        {
            // Get the last move direction from the character movement AI
            Vector2 shootDirection = characterMovement.GetLastMoveDirection();

            // Calculate rotation angle to point the arrow in the movement direction
            float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;
            Quaternion arrowRotation = Quaternion.AngleAxis(angle, Vector3.forward);

            // Instantiate the arrow at attackPoint with the calculated rotation
            GameObject bullet = Instantiate(arrowPrefab, attackPoint.position, arrowRotation);
            
            // Set arrow properties
            Arrow arrowScript = bullet.GetComponent<Arrow>();
            if (arrowScript != null)
            {
                arrowScript.SetEnemyLayer(enemyLayer);
                arrowScript.SetAttackDamage(arrowDamage);
                arrowScript.SetLifetime(arrowLifetime);
                arrowScript.SetKnockbackForce(knockbackForce);
                arrowScript.SetKnockbackDuration(knockbackDuration);
                
                // Important: Set the player GameObject so arrow can ignore ALL its colliders
                arrowScript.SetCharacterObject(transform.root.gameObject);
            }
            
            // Setup rigidbody and physics
            Rigidbody2D rbBullet = bullet.GetComponent<Rigidbody2D>();
            rbBullet.AddForce(shootDirection * bulletForce, ForceMode2D.Impulse);
            rbBullet.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            // Ignore collision between the arrow and the player who shot it
            Collider2D arrowCollider = bullet.GetComponent<Collider2D>();
            Collider2D characterCollider = transform.root.GetComponent<Collider2D>();
            if (arrowCollider != null && characterCollider != null)
            {
                Physics2D.IgnoreCollision(arrowCollider, characterCollider);
            }
        }
    }
}