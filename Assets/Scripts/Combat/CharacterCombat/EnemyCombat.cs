using System.Collections;
using System.Collections.Generic;
using Scripts.Combats.Weapons;
using Scripts.Movements;
using Scripts.Movements.AI;
using UnityEngine;
using MyGame;

namespace Scripts.Combats.CharacterCombats
{
    public class EnemyCombat : Combat
    {
        [SerializeField] private Transform player;
        [SerializeField] private ContactDamage contactDamage;
        [SerializeField] public float startAttackRange = 1f; // Reichweite, in der der Gegner den Spieler versucht anzugreifen 
        [SerializeField] protected Sword sword;


        public override MovementAI getCharacterMovement()
        {
            return GetComponent<EnemyMovementAI>();
        }

        public void SetPlayer(Transform playerTransform)
        {
            player = playerTransform;
        }

        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
            contactDamage = GetComponent<ContactDamage>();
            if (contactDamage == null)
            {
                Debug.LogError("ContactDamage component not found on " + gameObject.name);
            }
            contactDamage.SetEnemyLayer(enemyLayer);

            sword = GetComponentInChildren<Sword>();
            if (sword == null)
            {
                Debug.LogError("Sword component not found on " + gameObject.name);
            }
        }

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        protected override void CheckCombatInput()
        {
            if (player != null)
            {
                float distanceToPlayer = Vector2.Distance(transform.position, player.position);
                if (distanceToPlayer <= startAttackRange)
                {
                    contactDamage.SetIsEnabled(true);
                    if (combatEnabled)
                    {
                        gotInput = true;
                        lastInputTime = Time.time;
                    }
                }
                else
                {
                    contactDamage.SetIsEnabled(false);
                }
            }
        }

        protected override void CheckAttacks()
        {
            if (gotInput)
            {
                if (!isAttacking)
                {
                    gotInput = false;
                    isAttacking = true;
                    isFirstAttack = !isFirstAttack;
                    animator.SetBool("SwordAttack", true);
                    animator.SetBool("IsFirstAttack", isFirstAttack);
                    animator.SetBool("IsAttacking", isAttacking);
                    animator.SetFloat("StayHorizontal", characterMovementAI.lastMoveDirection.x);
                    animator.SetFloat("StayVertical", characterMovementAI.lastMoveDirection.y);
                    SoundManager.PlaySound(SoundType.SWING);
                }
            }

            if (Time.time >= lastInputTime + inputTimer)
            {
                gotInput = false;
            }
        }

        protected override void FinishAttack()
        {
            isAttacking = false;
            animator.SetBool("IsAttacking", isAttacking);
            animator.SetBool("SwordAttack", false);
        }
    }

}
