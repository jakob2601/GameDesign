using System.Collections;
using System.Collections.Generic;
using Scripts.Movements;
using UnityEngine;

namespace Scripts.Combats
{
    public class EnemyCombat : Combat
    {
        private Transform player;
        public float startAttackRange = 1f; // Reichweite, in der der Gegner den Spieler versucht anzugreifen 
        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
        }

        // Update is called once per frame
        void Update()
        {
            if (player != null)
            {
                float distanceToPlayer = Vector2.Distance(transform.position, player.position);

                if (distanceToPlayer <= attackRange)
                {
                    Attack();
                }
            }
        }

        public void SetPlayer(Transform playerTransform)
        {
            player = playerTransform;
        }

        protected override Movement getCharacterDirection()
        {
            return GetComponent<EnemyMovement>();
        }
    }

}
