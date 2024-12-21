using System.Collections;
using System.Collections.Generic;
using Scripts.Combats.Weapons;
using Scripts.Movements;
using Scripts.Movements.AI;
using UnityEngine;

namespace Scripts.Combats.CharacterCombats
{
    public class EnemyCombat : Combat
    {
        private Transform player;
        private ContactDamage contactDamage;
        public float startAttackRange = 1f; // Reichweite, in der der Gegner den Spieler versucht anzugreifen 
        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
            contactDamage = GetComponent<ContactDamage>();
        }

        // Update is called once per frame
        public void Update()
        {
            if (player != null)
            {
                float distanceToPlayer = Vector2.Distance(transform.position, player.position);

                if (distanceToPlayer <= startAttackRange)
                {
                    contactDamage.PerformAttack(playerDirection, enemyLayers);
                }
            }
        }

        public void SetPlayer(Transform playerTransform)
        {
            player = playerTransform;
        }

        public override MovementAI getCharacterDirection()
        {
            return GetComponent<EnemyMovementAI>();
        }
    }

}
