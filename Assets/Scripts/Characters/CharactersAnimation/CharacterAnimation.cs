using MyGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Characters.CharactersAnimation
{
    public abstract class CharacterAnimation : MonoBehaviour
    {
        [SerializeField] protected Animator animator;

        protected virtual void Start()
        {
            animator = GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError("Animator is not assigned.");
            }
        }

        public virtual void SetMovementAnimation(Vector2 direction)
        {
            animator.SetFloat("Horizontal", direction.x);
            animator.SetFloat("Vertical", direction.y);
            animator.SetFloat("Speed", direction.sqrMagnitude);
        }

        public void SetIsMoving(bool isMoving)
        {
            animator.SetBool("IsMoving", isMoving);
        }

        public void SetCanAttack(bool canAttack)
        {
            animator.SetBool("CanAttack", canAttack);
        }

        public void SetIsHurt(bool isHurt)
        {
            if (isHurt)
            {
                animator.SetTrigger("IsHurt");
            }
            else
            {
                animator.ResetTrigger("IsHurt");
            }
        }

        public void SetIsDead(bool isDead)
        {
            if (isDead)
            {
                animator.SetTrigger("IsDead");
            }
            else
            {
                animator.ResetTrigger("IsDead");
            }
        }

        public void SetDirection(Vector2 direction)
        {
            animator.SetFloat("StayHorizontal", direction.x);
            animator.SetFloat("StayVertictal", direction.y);
        }

        public virtual void SetSwordAttackAnimation(bool isSwordAttack)
        {
            animator.SetBool("IsSwordAttack", isSwordAttack);
        }

        public virtual void SetBowAttackAnimation(bool isBowAttack)
        {
            animator.SetBool("IsBowAttack", isBowAttack);
        }

        public virtual void CheckSwordAttackHitBox()
        {

        }

        public virtual void CheckBowAttackHitBox()
        {

        }

        public virtual void FinishSwordAttackAnimation()
        {

        }

        public virtual void FinishBowAttackAnimation()
        {

        }
    }
}