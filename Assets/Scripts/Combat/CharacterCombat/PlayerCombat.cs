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
                    characterAnimation.SetSwordAttackAnimation(true);
                    SoundManager.PlaySound(SoundType.SWING);
                }
            }

            if (Time.time >= lastInputTime + inputTimer)
            {
                // Wait for new Input
                gotInput = false;
            }
        }

        public override void FinishAttack()
        {
            isAttacking = false;
        }

    }

}
