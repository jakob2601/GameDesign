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
            GameObject bullet = Instantiate(arrowPrefab, attackPoint.position, attackPoint.rotation);
            Rigidbody2D rbBullet = bullet.GetComponent<Rigidbody2D>();
            rbBullet.AddForce(attackPoint.up * bulletForce, ForceMode2D.Impulse);
        }
    }
}