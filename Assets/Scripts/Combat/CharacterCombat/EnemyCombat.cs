using System.Collections;
using System.Collections.Generic;
using Scripts.Combats.Weapons;
using Scripts.Movements;
using Scripts.Movements.AI;
using UnityEngine;
using MyGame;

namespace Scripts.Combats.CharacterCombats
{
    public class EnemyCombat : Combat
    {
        [SerializeField] private Transform player;
        [SerializeField] private ContactDamage contactDamage;
        [SerializeField] public float startAttackRange = 1f; // Reichweite, in der der Gegner den Spieler versucht anzugreifen 
        [SerializeField] protected Sword sword;
        [SerializeField] protected Bow bow;

        [Header("Ranged Combat Settings")]
        [SerializeField] public float minShootRange = 3f;  // Too close, will try to back up
        [SerializeField] public float idealShootRange = 5f; // Perfect shooting distance
        [SerializeField] public float maxShootRange = 7f;  // Too far, will try to move closer
        [SerializeField] protected float shootAngleThreshold = 0.95f; // About 18 degrees of tolerance
        [SerializeField] protected float aimTime = 0.5f;  // Reduced time spent aiming before shooting
        [SerializeField] protected float bowCooldownTime = 2.0f;  // Slightly reduced cooldown between shots
        [SerializeField] protected GameObject aimIndicator; // Optional visual indicator for aiming

        protected bool isAiming = false;
        protected float aimStartTime = 0f;
        protected float lastBowAttackTime = -10f;  // Track the last time we shot an arrow


        public override MovementAI getCharacterMovement()
        {
            return transform.root.GetComponentInChildren<EnemyMovementAI>();
        }

        public void SetPlayer(Transform playerTransform)
        {
            player = playerTransform;
        }

        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();

            // Get contact damage component
            contactDamage = GetComponentInChildren<ContactDamage>();
            if (contactDamage != null)
            {
                contactDamage.SetEnemyLayer(enemyLayer);
            }
            else
            {
                Debug.LogWarning("ContactDamage component not found on " + gameObject.name);
            }

            // Get sword component - now optional
            sword = GetComponentInChildren<Sword>();
            if (sword != null)
            {
                sword.SetEnemyLayer(enemyLayer);
            }

            bow = GetComponentInChildren<Bow>();
            if (bow != null)
            {
                bow.SetEnemyLayer(enemyLayer);
            }
        }

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();

            if (aimIndicator != null)
            {
                aimIndicator.SetActive(isAiming);
            }
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        protected override void CheckCombatInput()
        {
            if (player != null)
            {
                float distanceToPlayer = Vector2.Distance(transform.position, player.position);
                float timeSinceLastBowAttack = Time.time - lastBowAttackTime;
                bool bowCooldownFinished = timeSinceLastBowAttack >= bowCooldownTime;

                if (distanceToPlayer <= startAttackRange)
                {
                    // Enable contact damage if available
                    if (HasWeapon(WeaponTypes.ContactDamage) && contactDamage != null)
                    {
                        contactDamage.SetIsEnabled(true);
                    }

                    // Stelle sicher, dass der Gegner nur dann angreift, wenn er nicht bereits im Angriff ist
                    if (combatEnabled && !isAttacking && !isAiming)
                    {
                        gotInput = true;
                        isSwordAttack = HasWeapon(WeaponTypes.Sword);
                        lastInputTime = Time.time;
                    }
                }
                else if (HasWeapon(WeaponTypes.Bow) && bow != null && combatEnabled && !isAttacking && bowCooldownFinished)
                {
                    // Check if we're in a good position to shoot
                    if (IsInGoodShootingPosition())
                    {
                        // Start aiming if not already aiming
                        if (!isAiming)
                        {
                            isAiming = true;
                            aimStartTime = Time.time;
                            Debug.Log($"{gameObject.name} is taking aim... (Cooldown: {timeSinceLastBowAttack:F1}s)");
                        }
                        // If we've been aiming long enough, shoot
                        else if (Time.time >= aimStartTime + aimTime)
                        {
                            gotInput = true;
                            isBowAttack = HasWeapon(WeaponTypes.Bow);
                            isSwordAttack = false;
                            lastInputTime = Time.time;
                            lastBowAttackTime = Time.time;  // Record when we shot
                            isAiming = false;
                            Debug.Log($"{gameObject.name} is shooting!");
                        }
                    }
                    else
                    {
                        // Reset aiming if we lose the good position
                        isAiming = false;
                    }
                }
                else
                {
                    if (HasWeapon(WeaponTypes.ContactDamage) && contactDamage != null)
                    {
                        contactDamage.SetIsEnabled(false);
                    }

                    // Reset aiming
                    isAiming = false;
                }
            }
        }

        protected override void CheckAttacks()
        {
            if (gotInput && !isAttacking)
            {
                if (HasWeapon(WeaponTypes.Sword) && isSwordAttack)
                {
                    gotInput = false;
                    isAttacking = true;
                    isFirstAttack = !isFirstAttack;
                    characterAnimation.SetSwordAttackAnimation(true);
                    SoundManager.PlaySound(SoundType.SWING);
                }
                else if (HasWeapon(WeaponTypes.Bow) && isBowAttack)
                {
                    gotInput = false;
                    isAttacking = true;
                    characterAnimation.SetBowAttackAnimation(true);
                    // SoundManager.PlaySound(SoundType.SHOOT);
                }
                else
                {
                    gotInput = false;
                }
            }

            if (Time.time >= lastInputTime + inputTimer)
            {
                gotInput = false;
            }
        }

        protected bool IsInGoodShootingPosition()
        {
            if (player == null) return false;
            
            // Check distance
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            bool goodDistance = distanceToPlayer >= minShootRange && distanceToPlayer <= maxShootRange;
            
            // Check line of sight
            bool hasLineOfSight = HasLineOfSightToPlayer();
            
            // Check if we're facing the player properly
            Vector2 directionToPlayer = (player.position - transform.position).normalized;
            float facingAlignment = Vector2.Dot(characterMovementAI.GetLastMoveDirection().normalized, directionToPlayer);
            bool properlyAligned = facingAlignment >= shootAngleThreshold;
            
            // Log information about positioning
            if (!goodDistance || !hasLineOfSight || !properlyAligned)
            {
                Debug.Log($"Shot position check: Distance ok={goodDistance} ({distanceToPlayer:F1}), " +
                          $"LOS={hasLineOfSight}, Aligned={properlyAligned} ({facingAlignment:F2})");
            }
            
            return goodDistance && hasLineOfSight && properlyAligned;
        }

        public bool HasLineOfSightToPlayer()
        {
            if (player == null) return false;
            
            Vector2 direction = player.position - transform.position;
            float distance = direction.magnitude;
            
            RaycastHit2D hit = Physics2D.Raycast(
                transform.position, 
                direction.normalized,
                distance,
                LayerMask.GetMask("Obstacle")
            );
            
            // Draw debug ray
            Debug.DrawRay(transform.position, direction, 
                hit.collider == null ? Color.green : Color.red, 0.1f);
                
            // If we hit nothing, we have line of sight
            return hit.collider == null;
        }

        public override void FinishSwordAttack()
        {
            isAttacking = false;
            isSwordAttack = false;
            StartCoroutine(AttackCooldown(1.0f)); 
        }

        private IEnumerator AttackCooldown(float cooldown)
        {
            combatEnabled = false; // Angriff vorübergehend deaktivieren
            yield return new WaitForSeconds(cooldown);
            combatEnabled = true;  // Nach Ablauf der Zeit wieder aktivieren
        }


        public override void FinishBowAttack()
        {
            isAttacking = false;
            isBowAttack = false;
            StartCoroutine(AttackCooldown(bowCooldownTime)); 
        }

        public void CancelAttack()
        {
            isAttacking = false;
            gotInput = false;
            isSwordAttack = false;
            isBowAttack = false;

            // Animation zurücksetzen
            characterAnimation.SetSwordAttackAnimation(false);
            characterAnimation.SetBowAttackAnimation(false);
        }

    }

}
