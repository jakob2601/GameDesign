using MyGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scripts.Combats.CharacterCombats;
using Scripts.Combats.Weapons;
using Scripts.Movements.AI;

namespace Scripts.Characters.CharactersAnimation
{
    class EnemyAnimation : CharacterAnimation
    {
        [SerializeField] protected EnemyCombat enemyCombat;
        [SerializeField] protected Sword sword;
        [SerializeField] protected EnemyMovementAI movementAI;

        protected override void Start()
        {
            base.Start();
            enemyCombat = transform.root.GetComponent<EnemyCombat>();
            if (enemyCombat == null)
            {
                Debug.LogError("EnemyCombat is not assigned.");
            }

            sword = transform.root.GetComponent<Sword>();
            if (sword == null)
            {
                Debug.LogError("Sword is not assigned.");
            }

            movementAI = transform.root.GetComponent<EnemyMovementAI>();
            if (movementAI == null)
            {
                Debug.LogError("EnemyMovementAI is not assigned.");
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
            animator.SetBool("IsFirstAttack", enemyCombat.GetIsFirstAttack());
            animator.SetBool("IsAttacking", enemyCombat.GetIsAttacking());
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
            enemyCombat.FinishAttack();
            animator.SetBool("IsAttacking", enemyCombat.GetIsAttacking());
            animator.SetBool("IsSwordAttack", false);
        }
    }
}