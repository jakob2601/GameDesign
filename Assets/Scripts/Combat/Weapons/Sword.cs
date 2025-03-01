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
        [SerializeField] private Hitstop hitstop; // Reference to the Hitstop component
        [SerializeField] private float hitstopDuration = 0.1f; // Default hitstop duration
        [SerializeField] private ScreenShake screenShake; // Reference to the ScreenShake component
        [SerializeField] protected Color damageColor = Color.red; // Farbe, die der Charakter annimmt, wenn er Schaden verteilt
        [SerializeField] protected float damageFlashDuration = 0.1f; // Dauer der Farbänderung

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
                Vector2 hitDirection = (Vector2)(enemy.transform.position - transform.position);
                Health enemyHealth = enemy.GetComponent<Health>();
                if (enemyHealth != null)
                {
                    int initialHealth = enemyHealth.GetCurrentHealth();
                    enemyHealth.TakeDamage(attackDamage, hitDirection, knockbackForce, knockbackDuration);

                    // Apply hitstop and screen shake only if health is reduced
                    if (enemyHealth.GetCurrentHealth() < initialHealth)
                    {
                        if (hitstop != null)
                        {
                            hitstop.SetHitstopDuration(hitstopDuration);
                            StartCoroutine(hitstop.ApplyHitstop());
                        }

                        if (screenShake != null)
                        {
                            StartCoroutine(screenShake.Shake());
                        }
                    }
                }
                else
                {
                    Debug.Log("Enemy Health Component not found");
                }
            }
        }

        private void OnDrawGizmos()
        {
            if (attackPoint == null)
                return;

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