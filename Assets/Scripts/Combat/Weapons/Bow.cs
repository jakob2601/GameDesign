using UnityEngine;
using Scripts.Combats.CharacterCombats;
using Scripts.Movements.AI;
using MyGame; // Add this to access Arrow class

namespace Scripts.Combats.Weapons
{
    public class Bow : Weapon
    {
        // Add an enum for arrow types
        public enum SpecialArrowType
        {
            None,
            Ricochet,
            Pierce
        }

        [Header("Bow Properties")]
        [SerializeField] protected GameObject arrowPrefab;
        [SerializeField] protected float bulletForce = 20f;
        [SerializeField] protected float arrowLifetime = 5f;

        [Header("Special Arrow Properties")]
        public SpecialArrowType currentSpecialArrowType = SpecialArrowType.None;
        [SerializeField] protected float specialArrowChance = 1f;

        // Replace individual flags with a unified system

        protected override void Start()
        {
            base.Start();
            SetPierce(false);
            SetRicochet(false);
        }

        public void SetRicochet(bool enable)
        {
            if (enable)
            {
                // If we're enabling ricochet, disable pierce and set arrow type
                currentSpecialArrowType = SpecialArrowType.Ricochet;
                Debug.Log("Special Arrows: Ricochet enabled" +
                         (currentSpecialArrowType == SpecialArrowType.Pierce ? " (Pierce disabled)" : ""));
            }
            else if (currentSpecialArrowType == SpecialArrowType.Ricochet)
            {
                // Only disable if it was ricochet
                currentSpecialArrowType = SpecialArrowType.None;
                Debug.Log("Special Arrows: Ricochet disabled");
            }
        }

        public void SetPierce(bool enable)
        {
            if (enable)
            {
                // If we're enabling pierce, disable ricochet and set arrow type
                currentSpecialArrowType = SpecialArrowType.Pierce;
                Debug.Log("Special Arrows: Pierce enabled" +
                         (currentSpecialArrowType == SpecialArrowType.Ricochet ? " (Ricochet disabled)" : ""));
            }
            else if (currentSpecialArrowType == SpecialArrowType.Pierce)
            {
                // Only disable if it was pierce
                currentSpecialArrowType = SpecialArrowType.None;
                Debug.Log("Special Arrows: Pierce disabled");
            }
        }

        public override void PerformAttack()
        {
            base.PerformAttack();
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
                arrowScript.SetAttackDamage(attackDamage);
                arrowScript.SetLifetime(arrowLifetime);
                arrowScript.SetKnockbackForce(knockbackForce);
                arrowScript.SetKnockbackDuration(knockbackDuration);

                // Apply special abilities based on current arrow type
                arrowScript.SetSpecialArrowType((Arrow.SpecialArrowType)currentSpecialArrowType);

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