using MyGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scripts.Combats.CharacterCombats;
using Scripts.Combats.Weapons;
using Scripts.Movements.AI;

namespace Scripts.Characters.CharactersAnimation
{
    class PlayerAnimation : CharacterAnimation
    {
        [SerializeField] protected PlayerCombat playerCombat;
        [SerializeField] protected Sword sword;
        [SerializeField] protected PlayerMovementAI movementAI;
        protected override void Start()
        {
            base.Start();

            playerCombat = transform.root.GetComponent<PlayerCombat>();
            if (playerCombat == null)
            {
                Debug.LogError("PlayerCombat is not assigned.");
            }

            sword = transform.root.GetComponent<Sword>();
            if (sword == null)
            {
                Debug.LogError("Sword is not assigned.");
            }

            movementAI = transform.root.GetComponent<PlayerMovementAI>();
            if (movementAI == null)
            {
                Debug.LogError("PlayerMovementAI is not assigned.");
            }
        }

        public override void SetMovementAnimation(Vector2 direction) 
        {
            base.SetMovementAnimation(direction);
        }

        public override void SetAttackAnimation(bool isAttacking)
        {
            base.SetAttackAnimation(isAttacking);
        }

        public override void SetDeathAnimation()
        {
            base.SetDeathAnimation();
        }

        public override void SetHurtAnimation()
        {
            base.SetHurtAnimation();
        }

        public override void SetIdleAnimation()
        {
            base.SetIdleAnimation();
        }

        public override void SetAttackAnimation()
        {
            base.SetAttackAnimation();
            animator.SetBool("IsFirstAttack", playerCombat.GetIsFirstAttack());
            animator.SetBool("IsAttacking", playerCombat.GetIsAttacking());
            animator.SetFloat("StayHorizontal", movementAI.lastMoveDirection.x);
            animator.SetFloat("StayVertical", movementAI.lastMoveDirection.y);
        }

        public override void CheckAttackHitBox()
        {
            base.CheckAttackHitBox();
            sword.CheckAttackHitBox();
        }

        public override void FinishAttackAnimation()
        {  
            base.FinishAttackAnimation();
            playerCombat.FinishAttack();
            animator.SetBool("IsAttacking", playerCombat.GetIsAttacking());
            animator.SetBool("SwordAttack", false);
        }
    }
}