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
        [SerializeField] private Transform player;
        [SerializeField] private ContactDamage contactDamage;
        [SerializeField] public float startAttackRange = 1f; // Reichweite, in der der Gegner den Spieler versucht anzugreifen 
        [SerializeField] protected Sword sword;
        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
            contactDamage = GetComponent<ContactDamage>();
            if(contactDamage == null)
            {
                Debug.LogError("ContactDamage component not found on " + gameObject.name);
            }
            contactDamage.SetEnemyLayer(enemyLayer);

            sword = GetComponentInChildren<Sword>();
            if(sword == null)
            {
                Debug.LogError("Sword component not found on " + gameObject.name);
            }
        }

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();
            if (player != null)
            {
                float distanceToPlayer = Vector2.Distance(transform.position, player.position);
                if (distanceToPlayer <= startAttackRange)
                {
                    contactDamage.SetIsEnabled(true);
                    if (Time.time >= nextAttackTime)
                    {
                        sword.PerformAttack();
                        nextAttackTime = Time.time + 1f / attackRate;
                    }
                }
                else
                {
                    contactDamage.SetIsEnabled(false);
                    sword.GetAnimator().ResetTrigger("Attack"); // Reset attack animation
                }
            }
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
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
