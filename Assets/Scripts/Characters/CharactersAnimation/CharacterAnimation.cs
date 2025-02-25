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

        }

        public virtual void SetAttackAnimation(bool isAttacking)
        {

        }

        public virtual void SetDeathAnimation()
        {

        }

        public virtual void SetHurtAnimation()
        {

        }

        public virtual void SetIdleAnimation()
        {

        }

        public virtual void SetAttackAnimation()
        {
            animator.SetBool("SwordAttack", true);
        }

        public virtual void CheckAttackHitBox()
        {

        }

        public virtual void FinishAttackAnimation()
        {
    
        }
    }
}