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
        [Header("Weapons")]
        [SerializeField] protected Sword sword;
        [SerializeField] protected Bow bow;

        // Variables for attack queueing
        [SerializeField] protected bool queuedSwordAttack = false;
        [SerializeField] protected bool queuedBowAttack = false;
        [SerializeField] protected float queuedAttackWindow = 0.5f; // Time window to queue attacks
        [SerializeField] protected float minQueueTime = 0.25f; // Minimum time before next attack can be queued
        [SerializeField] protected float queuedAttackTime;

        // Debug variables
        [SerializeField] protected bool debugAttackState = false;

        // Flag to track arrow shooting ability
        [SerializeField] private bool canShootArrow = false;

        public override MovementAI getCharacterMovement()
        {
            return transform.root.GetComponentInChildren<PlayerMovementAI>();
        }

        protected override void Start()
        {
            base.Start();
            sword = GetComponentInChildren<Sword>();
            if(sword == null)
            {
                Debug.LogError("No sword found for player combat");
            }
            else
            {
                sword.SetEnemyLayer(enemyLayer);
            }

            bow = GetComponentInChildren<Bow>();
            if(bow == null)
            {
                Debug.LogError("No bow found for player combat");
            }
            else
            {
                bow.SetEnemyLayer(enemyLayer);
            }
        }

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
            // Sword attack input
            if (Input.GetKeyDown(KeyCode.R) || Input.GetMouseButtonDown(0))
            {
                if (combatEnabled)
                {
                    if (!isAttacking && !gotInput && Time.time >= lastInputTime + sword.GetNextAttackTime())
                    {
                        // If not attacking, start attack immediately
                        gotInput = true;
                        isSwordAttack = true;
                        lastInputTime = Time.time;
                        if (debugAttackState) Debug.Log("Direct sword attack");
                    }
                    else if (!queuedSwordAttack && gotInput && Time.time >= lastInputTime + minQueueTime)
                    {
                        // If already attacking, queue this attack
                        queuedSwordAttack = true;
                        queuedAttackTime = Time.time;
                        queuedBowAttack = false; // Prioritize last input
                        if (debugAttackState) Debug.Log("Queued sword attack");
                    }
                }
            }
            // Bow attack input
            else if (Input.GetMouseButtonDown(1) && canShootArrow)
            {
                if (combatEnabled)
                {
                    if (!isAttacking && !gotInput && Time.time >= lastInputTime + bow.GetNextAttackTime())
                    {
                        // If not attacking, start attack immediately
                        gotInput = true;
                        isBowAttack = true;
                        lastInputTime = Time.time;
                        if (debugAttackState) Debug.Log("Direct bow attack");
                    }
                    else if(!queuedBowAttack && gotInput && Time.time >= lastInputTime + minQueueTime)
                    {
                        // If already attacking, queue this attack
                        queuedBowAttack = true;
                        queuedAttackTime = Time.time;
                        queuedSwordAttack = false; // Prioritize last input
                        if (debugAttackState) Debug.Log("Queued bow attack");
                    }
                }
            }
        }

        protected override void CheckAttacks()
        {
            if (gotInput && !isAttacking)
            {
                gotInput = false;
                isAttacking = true;
                isFirstAttack = !isFirstAttack;

                if (isSwordAttack)
                {
                    characterAnimation.SetSwordAttackAnimation(true);
                    SoundManager.PlaySound(SoundType.SWING);
                    if (debugAttackState) Debug.Log("Starting sword attack");
                }
                else if (isBowAttack)
                {
                    characterAnimation.SetBowAttackAnimation(true);
                    // SoundManager.PlaySound(SoundType.BOW);
                    if (debugAttackState) Debug.Log("Starting bow attack");
                }
            }

            // Reset gotInput if too much time passes
            if (Time.time >= lastInputTime + inputTimer)
            {
                gotInput = false;
            }

            // Reset queued attacks if they expire
            if ((queuedSwordAttack || queuedBowAttack) &&
                Time.time >= queuedAttackTime + queuedAttackWindow)
            {
                queuedSwordAttack = false;
                queuedBowAttack = false;
                if (debugAttackState) Debug.Log("Queued attacks expired");
            }
        }

        public override void FinishSwordAttack()
        {
            if (debugAttackState) Debug.Log("Finishing sword attack");
            isSwordAttack = false;
            isAttacking = false;  // Set this first to prevent race conditions

            // Use a coroutine to delay queued attack processing by one frame
            StartCoroutine(ProcessQueuedAttacksNextFrame());
        }

        public override void FinishBowAttack()
        {
            if (debugAttackState) Debug.Log("Finishing bow attack");
            isBowAttack = false;
            isAttacking = false;  // Set this first to prevent race conditions

            // Use a coroutine to delay queued attack processing by one frame
            StartCoroutine(ProcessQueuedAttacksNextFrame());
        }

        private IEnumerator ProcessQueuedAttacksNextFrame()
        {
            // Wait for the end of the frame
            yield return null;

            // Now check for queued attacks
            if (Time.time < queuedAttackTime + queuedAttackWindow)
            {
                if (queuedSwordAttack)
                {
                    if (debugAttackState) Debug.Log("Processing queued sword attack");
                    queuedSwordAttack = false;
                    // Start sword attack
                    gotInput = true;
                    isSwordAttack = true;
                    lastInputTime = Time.time;
                }
                else if (queuedBowAttack)
                {
                    if (debugAttackState) Debug.Log("Processing queued bow attack");
                    queuedBowAttack = false;
                    // Start bow attack
                    gotInput = true;
                    isBowAttack = true;
                    lastInputTime = Time.time;
                }
            }
        }

        // Method to enable arrow shooting
        public void EnableArrowShooting()
        {
            canShootArrow = true;
            Debug.Log("Arrow shooting enabled.");
        }
    }
}