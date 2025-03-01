using UnityEngine;
using System.Collections;
using Scripts.Movements.AI;
using Scripts.Healths;
using Scripts.Combats.CharacterCombats;
using MyGame;
using Scripts.Characters;


namespace Scripts.Combats.Weapons
{
    public class Sword : Weapon
    {
        [SerializeField] float angleThreshold = 0.7f; // Entspricht ca. 45 Grad
        [SerializeField] protected Color damageColor = Color.red; // Farbe, die der Charakter annimmt, wenn er Schaden verteilt
        [SerializeField] protected float damageFlashDuration = 0.1f; // Dauer der Farbänderung

        [Header("Hitstop & Screen Shake")]
        [SerializeField] protected Hitstop hitstop; // Reference to the Hitstop component
        [SerializeField] protected float hitstopDuration = 0.1f; // Default hitstop duration
        [SerializeField] protected ScreenShake screenShake; // Reference to the ScreenShake component

        // Add clash-specific properties
        [Header("Sword Clash")]
        [SerializeField] protected float clashKnockbackForce = 3f; // Stronger than normal knockback
        [SerializeField] protected float clashKnockbackDuration = 0.3f; // Clash knockback duration
        [SerializeField] protected float clashCooldown = 0.5f; // Prevent spam clashes
        [SerializeField] protected float clashTimeWindow = 0.1f;

        protected float lastClashTime = -1f;

        protected override void Start()
        {
            base.Start();
            hitstop = FindObjectOfType<Hitstop>(); // Find the Hitstop component in the scene
            screenShake = FindObjectOfType<ScreenShake>(); // Find the ScreenShake component in the scene
        }

        protected override void Update()
        {
            base.Update();
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        public void CheckSwordAttackHitBox()
        {
            if (attackPoint == null)
            {
                Debug.LogError("Attack point or character movement is not assigned.");
                return;
            }

            if (characterMovement == null)
            {
                Debug.LogError("Animator or player direction is not assigned.");
                return;
            }

            Vector2 attackPosition = (Vector2)transform.position + (Vector2.up * -0.3f) + characterMovement.lastMoveDirection.normalized * attackRange;

            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPosition, attackRange, enemyLayer);

            foreach (Collider2D enemy in hitEnemies)
            {
                // Skip if this is ourselves (happens sometimes with child colliders)
                if (enemy.transform.root == transform.root)
                    continue;

                Vector2 hitDirection = (Vector2)(enemy.transform.position - transform.position);

                // Check if the target has a Combat component and is actively attacking with a sword
                Combat targetCombat = enemy.transform.root.GetComponentInChildren<Combat>();
                Combat attackerCombat = transform.root.GetComponentInChildren<Combat>();

                // Check for clash conditions
                bool isClash = false;
                if (targetCombat != null && attackerCombat != null)
                {
                    // Both must be attacking with swords
                    bool bothAttackingWithSwords = targetCombat.GetIsAttacking() && attackerCombat.GetIsAttacking() &&
                                                targetCombat.isSwordAttack && attackerCombat.isSwordAttack;

                    // Attacks must be within the time window to be considered simultaneous
                    bool attacksAreSimultaneous = false;
                    if (bothAttackingWithSwords)
                    {
                        float targetAttackTime = targetCombat.GetLastInputTime();
                        float attackerAttackTime = attackerCombat.GetLastInputTime();

                        // Only clash if attacks started within 0.2 seconds of each other
                        attacksAreSimultaneous = Mathf.Abs(targetAttackTime - attackerAttackTime) <= clashTimeWindow;
                        Debug.Log($"Attack times: Target={targetAttackTime}, Attacker={attackerAttackTime}, Diff={Mathf.Abs(targetAttackTime - attackerAttackTime)}");
                    }

                    isClash = bothAttackingWithSwords && attacksAreSimultaneous &&
                            Time.time > lastClashTime + clashCooldown;
                }

                // Handle based on whether it's a clash or normal attack
                if (isClash)
                {
                    // It's a clash! Handle clash effect
                    lastClashTime = Time.time;

                    // Get health components for both entities
                    Health targetHealth = enemy.transform.root.GetComponentInChildren<Health>();
                    Health attackerHealth = transform.root.GetComponentInChildren<Health>();

                    // Apply clash effect to both entities
                    if (targetHealth != null)
                    {
                        targetHealth.HandleSwordClash(hitDirection.normalized, clashKnockbackForce, clashKnockbackDuration);
                    }

                    if (attackerHealth != null)
                    {
                        attackerHealth.HandleSwordClash(-hitDirection.normalized, clashKnockbackForce, clashKnockbackDuration);
                    }

                    Debug.Log("Sword clash between " + transform.root.name + " and " + enemy.transform.root.name);
                }
                else
                {
                    // Normal attack - apply damage
                    Health enemyHealth = enemy.GetComponent<Health>();
                    if (enemyHealth != null)
                    {
                        enemyHealth.TakeDamage(attackDamage, hitDirection, knockbackForce, knockbackDuration);

                        // Make attacker flash red on successful hit
                        CharacterGFX attackerGFX = transform.root.GetComponentInChildren<CharacterGFX>();
                        if (attackerGFX != null)
                        {
                            attackerGFX.FlashColor(damageColor, damageFlashDuration);
                        }
                    }
                    else
                    {
                        Debug.Log("Enemy Health Component not found");
                    }
                }
                if (hitstop != null)
                {
                    hitstop.SetHitstopDuration(hitstopDuration); // Set the hitstop duration
                    StartCoroutine(hitstop.ApplyHitstop()); // Apply hitstop
                }

                // Apply screen shake when an attack hits
                if (screenShake != null)
                {
                    StartCoroutine(screenShake.Shake()); // Apply screen shake
                }
            }
        }

        private void OnDrawGizmos()
        {
            if (attackPoint == null)
                return;

            // Zeigt den aktuellen Angriffspunkt
            //Gizmos.color = Color.red;
            //Gizmos.DrawWireSphere(attackPoint.position, attackRange);

            // Optional: Weltkoordinaten basierter Angriffspunkt
            if (Application.isPlaying) // Nur zur Laufzeit anzeigen
            {
                Vector2 attackPosition = (Vector2)transform.position + (Vector2.up * -0.3f) + characterMovement.lastMoveDirection.normalized * attackRange;
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(attackPosition, attackRange);
            }
        }
    }
}
