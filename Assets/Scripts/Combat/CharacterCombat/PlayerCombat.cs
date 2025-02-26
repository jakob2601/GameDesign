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
        [SerializeField] private Sword sword;

        public override MovementAI getCharacterMovement()
        {
            return transform.root.GetComponentInChildren<PlayerMovementAI>();
        }


        protected override void Start()
        {
            base.Start();
            sword = GetComponentInChildren<Sword>();
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
                    isSwordAttack = true;
                    lastInputTime = Time.time;
                }
            }
            else if (Input.GetMouseButtonDown(1))
            {
                if (combatEnabled)
                {
                    gotInput = true;
                    isBowAttack = true;
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
                    if(isSwordAttack) {
                        characterAnimation.SetSwordAttackAnimation(isSwordAttack);
                        SoundManager.PlaySound(SoundType.SWING);
                    }
                    else if(isBowAttack) {
                        characterAnimation.SetBowAttackAnimation(isBowAttack);
                        //SoundManager.PlaySound(SoundType.BOW);
                    }
                   
                }
            }

            if (Time.time >= lastInputTime + inputTimer)
            {
                // Wait for new Input
                gotInput = false;
            }
        }

        public override void FinishSwordAttack()
        {
            isAttacking = false;
            isSwordAttack = false;
        }

        public override void FinishBowAttack()
        {
            isAttacking = false;
            isBowAttack = false;
        }

    }

}
