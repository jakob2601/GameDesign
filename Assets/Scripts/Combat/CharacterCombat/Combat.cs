using UnityEngine;
using System.Collections;
using Scripts.Movements;
using Scripts.Healths;
using Scripts.Movements.AI;
using Scripts.Characters.CharactersAnimation;
using Scripts.Combats.Weapons;

namespace Scripts.Combats.CharacterCombats
{
    public abstract class Combat : MonoBehaviour
    {
        // Create a flags enum for weapon types
        [System.Flags]
        public enum WeaponTypes
        {
            None = 0,
            Sword = 1,
            Bow = 2,
            ContactDamage = 4
        }

        [Header("Combat Properties")]
        [SerializeField] protected bool combatEnabled;
        [SerializeField] protected LayerMask enemyLayer;
        
        // Track available weapons
        [SerializeField] protected WeaponTypes availableWeapons = WeaponTypes.None;

        [SerializeField] public Animator animator;
        [SerializeField] public CharacterAnimation characterAnimation;
        [SerializeField] public Rigidbody2D rb;

        [SerializeField] protected MovementAI characterMovementAI;

        [SerializeField] public float attackRate = 2f;
        [SerializeField] protected float inputTimer;
        [SerializeField] protected float lastInputTime = Mathf.NegativeInfinity;

        public bool ifDebug = false;
        public GameObject debugAttackRangeVisualizer; // Referenz für das Visualisierungssprite

        [Header("Combat States")]
        [SerializeField] protected bool gotInput;
        [SerializeField] protected bool isAttacking;
        [SerializeField] protected bool isFirstAttack;
        [SerializeField] public bool isSwordAttack, isBowAttack; 

        abstract public MovementAI getCharacterMovement();

        public bool GetCombatEnabled()
        {
            return combatEnabled;
        }

        public void SetCombatEnabled(bool combatEnabled)
        {
            characterAnimation.SetCanAttack(combatEnabled);
            this.combatEnabled = combatEnabled;
        }

        protected bool GetGotInput()
        {
            return gotInput;
        }

        protected void SetGotInput(bool gotInput)
        {
            this.gotInput = gotInput;
        }

        public bool GetIsAttacking()
        {
            return isAttacking;
        }

        protected void SetIsAttacking(bool isAttacking)
        {
            this.isAttacking = isAttacking;
        }

        public bool GetIsFirstAttack()
        {
            return isFirstAttack;
        }

        protected void SetIsFirstAttack(bool isFirstAttack)
        {
            this.isFirstAttack = isFirstAttack;
        }

        // Methods to check weapon availability
        public bool HasWeapon(WeaponTypes weaponType)
        {
            return (availableWeapons & weaponType) == weaponType;
        }
        
        public void SetWeaponAvailable(WeaponTypes weaponType, bool available)
        {
            if (available)
                availableWeapons |= weaponType;
            else
                availableWeapons &= ~weaponType;
        }

        public float GetLastInputTime()
        {
            return lastInputTime;
        }

        protected void SetLastInputTime(float lastInputTime)
        {
            this.lastInputTime = lastInputTime;
        }

        protected virtual void Start()
        {
            rb = transform.root.GetComponentInChildren<Rigidbody2D>();
            if (rb == null)
            {
                Debug.LogError("Rigidbody2D component not found on " + gameObject.name);
            }

            characterMovementAI = getCharacterMovement();
            if (characterMovementAI == null)
            {
                Debug.LogError("Player direction is not assigned.");
            }

            characterAnimation = transform.root.GetComponentInChildren<CharacterAnimation>();
            if (characterAnimation == null)
            {
                Debug.LogError("CharacterAnimation is not assigned.");
            }

            combatEnabled = true;
            gotInput = false;
            isFirstAttack = true;
            isAttacking = false;
            isSwordAttack = false;
            isBowAttack = false;
            animator = transform.root.GetComponentInChildren<Animator>();
            if (animator == null)
            {
                Debug.LogError("Animator component not found on " + gameObject.name);
            }
            else
            {
                characterAnimation.SetCanAttack(combatEnabled);
            }

            // Auto-detect available weapons
            DetectAvailableWeapons();
        }

        protected virtual void DetectAvailableWeapons()
        {
            // Reset weapon flags
            availableWeapons = WeaponTypes.None;
            
            // Check for sword
            if (GetComponentInChildren<Sword>() != null)
                SetWeaponAvailable(WeaponTypes.Sword, true);
                
            // Check for bow
            if (GetComponentInChildren<Bow>() != null)
                SetWeaponAvailable(WeaponTypes.Bow, true);
                
            // Check for contact damage
            if (GetComponentInChildren<ContactDamage>() != null)
                SetWeaponAvailable(WeaponTypes.ContactDamage, true);
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

        public abstract void FinishSwordAttack();

        public abstract void FinishBowAttack();

    }
}