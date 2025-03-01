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

        // Hitbox-Fixierung
        private Vector2 fixedAttackPosition;
        private bool isAttackActive = false; // Überprüft, ob der Angriff gerade läuft

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
            if (attackPoint == null || characterMovement == null)
            {
                Debug.LogError("Attack point or character movement is not assigned.");
                return;
            }

            // Fixiere die Position zum Zeitpunkt des Angriffs
            fixedAttackPosition = (Vector2)transform.position + (Vector2.up * -0.3f) + characterMovement.lastMoveDirection.normalized * attackRange;
            isAttackActive = true;

            StartCoroutine(FixHitboxForDuration(0.1f)); // Hitbox bleibt für 0.1 Sekunden aktiv
        }

        private IEnumerator FixHitboxForDuration(float duration)
        {
            float timer = duration;
            while (timer > 0)
            {
                CheckHitboxAtFixedPosition(); // Kollision an fixer Position prüfen
                timer -= Time.deltaTime;
                yield return null;
            }
            isAttackActive = false; // Nach der Zeit deaktivieren
        }

        private void CheckHitboxAtFixedPosition()
        {
            if (!isAttackActive) return; // Falls Angriff schon beendet, nichts tun

            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(fixedAttackPosition, attackRange, enemyLayer);

            foreach (Collider2D enemy in hitEnemies)
            {
                Vector2 hitDirection = (Vector2)(enemy.transform.position - transform.position);
                Health enemyHealth = enemy.GetComponent<Health>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(attackDamage, hitDirection, knockbackForce, knockbackDuration);
                }
                else
                {
                    Debug.Log("Enemy Health Component not found");
                }
            }
        }


        private void OnDrawGizmos()
        {
            if (attackPoint == null) return;

            // Zeigt die **fixierte** Angriffshitze an, falls der Angriff läuft
            if (Application.isPlaying && isAttackActive)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(fixedAttackPosition, attackRange);
            }
            else if (Application.isPlaying)
            {
                Vector2 attackPosition = (Vector2)transform.position + (Vector2.up * -0.3f) + characterMovement.lastMoveDirection.normalized * attackRange;
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(attackPosition, attackRange);
            }
        }
    }
}