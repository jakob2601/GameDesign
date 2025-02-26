using UnityEngine;
using Scripts.Combats.CharacterCombats;
using Scripts.Movements.AI;

namespace Scripts.Combats.Weapons
{
    public class Bow : Weapon
    {
        [Header("Bow Properties")]
        [SerializeField] protected GameObject arrowPrefab;
        [SerializeField] protected float bulletForce = 20f;

        public override void PerformAttack()
        {
            base.Start();
            // Füge hier die Logik für den Pfeil hinzu
        }

        public void CheckBowAttackHitBox()
        {
            // Get the last move direction from the character movement AI
            Vector2 shootDirection = characterMovement.GetLastMoveDirection();
            
            // Calculate rotation angle to point the arrow in the movement direction
            // The -90 degrees offset is because Unity's default "up" is facing up,
            // so we rotate to match the shoot direction
            float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg - 90f;
            Quaternion arrowRotation = Quaternion.AngleAxis(angle, Vector3.forward);
            
            // Instantiate the arrow at attackPoint with the calculated rotation
            GameObject bullet = Instantiate(arrowPrefab, attackPoint.position, arrowRotation);
            Rigidbody2D rbBullet = bullet.GetComponent<Rigidbody2D>();
            
            // Apply force in the shoot direction
            rbBullet.AddForce(shootDirection * bulletForce, ForceMode2D.Impulse);
        }
    }
}