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
        [SerializeField] protected Bow bow;
        [SerializeField] protected PlayerMovementAI movementAI;
        protected override void Start()
        {
            base.Start();

            playerCombat = transform.root.GetComponentInChildren<PlayerCombat>();
            if (playerCombat == null)
            {
                Debug.LogError("PlayerCombat is not assigned.");
            }

            sword = transform.root.GetComponentInChildren<Sword>();
            if (sword == null)
            {
                Debug.LogError("Sword is not assigned.");
            }

            bow = transform.root.GetComponentInChildren<Bow>();
            if (bow == null)
            {
                Debug.LogError("Bow is not assigned.");
            }

            movementAI = transform.root.GetComponentInChildren<PlayerMovementAI>();
            if (movementAI == null)
            {
                Debug.LogError("PlayerMovementAI is not assigned.");
            }
        }

        public override void SetMovementAnimation(Vector2 direction)
        {
            base.SetMovementAnimation(direction);
            animator.SetFloat("StayHorizontal", movementAI.lastMoveDirection.x);
            animator.SetFloat("StayVertical", movementAI.lastMoveDirection.y);
        }

        public override void SetSwordAttackAnimation(bool isSwordAttack)
        {
            base.SetSwordAttackAnimation(isSwordAttack);
            animator.SetBool("IsFirstAttack", playerCombat.GetIsFirstAttack());
            animator.SetBool("IsAttacking", playerCombat.GetIsAttacking());
            animator.SetFloat("StayHorizontal", movementAI.lastMoveDirection.x);
            animator.SetFloat("StayVertical", movementAI.lastMoveDirection.y);
        }

        public override void SetBowAttackAnimation(bool isBowAttack)
        {
            base.SetBowAttackAnimation(isBowAttack);
            animator.SetBool("IsFirstAttack", playerCombat.GetIsFirstAttack());
            animator.SetBool("IsAttacking", playerCombat.GetIsAttacking());
            animator.SetFloat("StayHorizontal", movementAI.lastMoveDirection.x);
            animator.SetFloat("StayVertical", movementAI.lastMoveDirection.y);
        }

        public override void CheckSwordAttackHitBox()
        {
            base.CheckSwordAttackHitBox();
            sword.CheckSwordAttackHitBox();
        }

        public override void CheckBowAttackHitBox()
        {
            base.CheckBowAttackHitBox();
            bow.CheckBowAttackHitBox();
        }

        public override void FinishSwordAttackAnimation()
        {
            base.FinishSwordAttackAnimation();
            playerCombat.FinishSwordAttack();
            animator.SetBool("IsAttacking", playerCombat.GetIsAttacking());
            animator.SetBool("IsSwordAttack", false);
        }

        public override void FinishBowAttackAnimation()
        {
            base.FinishBowAttackAnimation();
            playerCombat.FinishBowAttack();
            animator.SetBool("IsAttacking", playerCombat.GetIsAttacking());
            animator.SetBool("IsBowAttack", false);
        }
    }
}