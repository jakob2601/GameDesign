using UnityEngine;
using System.Collections;
using Scripts.Movements;
using Scripts.Healths;
using Scripts.Movements.AI;

namespace Scripts.Combats.CharacterCombats
{
    public abstract class Combat : MonoBehaviour
    {
        [SerializeField] public Animator animator;
        [SerializeField] public LayerMask enemyLayer;

        [SerializeField] public Rigidbody2D rb;

        [SerializeField] protected MovementAI characterMovementAI; 

        [SerializeField] public float attackRate = 2f;
        [SerializeField] protected float inputTimer;
        [SerializeField] protected float lastInputTime = Mathf.NegativeInfinity;

        public bool ifDebug = false;
        public GameObject debugAttackRangeVisualizer; // Referenz für das Visualisierungssprite

        [Header("Combat States")]
        [SerializeField] protected bool combatEnabled;
        [SerializeField] protected bool gotInput, isAttacking, isFirstAttack;

        abstract public MovementAI getCharacterMovement();

        public bool GetCombatEnabled()
        {
            return combatEnabled;
        }

        public void SetCombatEnabled(bool combatEnabled)
        {
            this.combatEnabled = combatEnabled;
        }

        protected virtual void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                Debug.LogError("Rigidbody2D component not found on " + gameObject.name);
            }

            characterMovementAI = getCharacterMovement();
            if (characterMovementAI == null)
            {
                Debug.LogError("Player direction is not assigned.");
            }

            animator = GetComponentInChildren<Animator>();
            if (animator == null)
            {
                Debug.LogError("Animator component not found on " + gameObject.name);
            }
            else 
            {   
                combatEnabled = true;
                gotInput = false;
                isFirstAttack = true;
                isAttacking = false;
                animator.SetBool("CanAttack", combatEnabled);
            }
        }

        protected virtual void Update()
        {
            CheckCombatInput();
            CheckAttacks();
        }

        protected virtual void FixedUpdate()
        {

        }
        protected abstract void CheckCombatInput();

        protected abstract void CheckAttacks();

        protected abstract void FinishAttack();

    }
}