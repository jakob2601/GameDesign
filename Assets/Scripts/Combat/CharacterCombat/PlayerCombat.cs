using MyGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scripts.Movements.AI;
using Scripts.Combats.Weapons;
using Scripts.Movements;


namespace Scripts.Combats.CharacterCombats
{
    public class PlayerCombat : Combat
    {
        private Sword sword;

        public override MovementAI getCharacterMovement()
        {
            return GetComponent<PlayerMovementAI>();
        }


        protected override void Start()
        {
            base.Start();
            sword = GetComponent<Sword>();
            sword.SetEnemyLayer(enemyLayer);
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
            if (Input.GetKeyDown(KeyCode.R) || Input.GetMouseButtonDown(0))
            {
                if (combatEnabled)
                {
                    gotInput = true;
                    lastInputTime = Time.time;
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
                // Wait for new Input
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
