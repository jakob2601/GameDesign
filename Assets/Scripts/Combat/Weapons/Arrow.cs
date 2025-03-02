using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame;
using Scripts.Healths;
using Scripts.Combats.CharacterCombats;
using Scripts.Combats.Features;
using Scripts.Characters;

namespace Scripts.Combats.Weapons
{
    public class Arrow : MonoBehaviour
    {
        public enum SpecialArrowType
        {
            None,
            Ricochet,
            Pierce
        }

        [Header("Arrow Properties")]
        [SerializeField] protected GameObject hitEffect;
        [SerializeField] protected float effectTime = 0.8f;
        [SerializeField] protected float timeToLive = 5f;
        [SerializeField] protected float arrowHitTime = 0.2f;
        
        [Header("Special Arrow Properties")]
        [SerializeField] protected SpecialArrowType specialArrowType = SpecialArrowType.None;
        [SerializeField] protected float specialArrowChance = 1f;
        [SerializeField] protected int hitEnemiesThisAttack = 0;
        [SerializeField] protected int maxEnemiesToHit = 3;

        // Properties that were previously inherited from Weapon
        [Header("Attack Properties")]
        [SerializeField] protected LayerMask enemyLayer;
        [SerializeField] protected int attackDamage = 1;
        [SerializeField] protected float knockbackForce = 1f;
        [SerializeField] protected float knockbackDuration = 0.2f;
        [SerializeField] protected bool canDamage = true;

        private Rigidbody2D rb;
        private GameObject playerObject;

        [Header("Hitstop & Screen Shake")]
        [SerializeField] protected Hitstop hitstop; // Reference to the Hitstop component
        [SerializeField] protected float hitstopDuration = 0.1f; // Default hitstop duration
        [SerializeField] protected ScreenShake screenShake; // Reference to the ScreenShake component

        void Start()
        {
            // Start the self-destruct timer
            Destroy(gameObject, timeToLive);

            // Get the rigidbody for direction information
            rb = GetComponent<Rigidbody2D>();

            hitstop = GetComponent<Hitstop>(); // Find the Hitstop component in the scene
            if (hitstop == null)
            {
                Debug.LogError("Hitstop not found for arrow");
            }

            screenShake = FindObjectOfType<ScreenShake>(); // Find the ScreenShake component in the scene
            if (screenShake == null)
            {
                Debug.LogError("ScreenShake not found for arrow");
            }
        }

        // Setter methods
        public void SetEnemyLayer(LayerMask layer)
        {
            enemyLayer = layer;
        }

        public void SetAttackDamage(int damage)
        {
            attackDamage = damage;
        }

        public void SetKnockbackForce(float force)
        {
            knockbackForce = force;
        }

        public void SetKnockbackDuration(float duration)
        {
            knockbackDuration = duration;
        }

        public void SetLifetime(float lifetime)
        {
            timeToLive = lifetime;

            // If Start has already run, update the destroy timer
            if (gameObject.activeInHierarchy)
            {
                CancelInvoke("DestroyArrow");
                Invoke("DestroyArrow", timeToLive);
            }
        }

        public void SetCharacterObject(GameObject player)
        {
            playerObject = player;

            // Ignore ALL colliders on the player and its children
            Collider2D arrowCollider = GetComponent<Collider2D>();
            Collider2D[] playerColliders = player.GetComponentsInChildren<Collider2D>(true); // true = include inactive components

            foreach (Collider2D playerCollider in playerColliders)
            {
                Physics2D.IgnoreCollision(arrowCollider, playerCollider);
            }
        }

        // New setter methods for special abilities
        public void SetSpecialArrowType(SpecialArrowType type)
        {
            specialArrowType = type;
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            HitObject(collision.gameObject, collision.contacts[0].point);
        }

        void OnTriggerEnter2D(Collider2D collider)
        {
            HitObject(collider.gameObject, collider.ClosestPoint(transform.position));
        }

        void HitObject(GameObject target, Vector2 hitPoint)
        {
            // Check if it's the player or child of player (failsafe)
            if (playerObject != null &&
                (target == playerObject || target.transform.IsChildOf(playerObject.transform)))
            {
                return; // Skip collision with player
            }

            Debug.Log("Arrow hit: " + target.name);

            // Check if we hit something in the enemy layer
            if (((1 << target.layer) & enemyLayer) != 0 && canDamage)
            {
                // Apply damage to enemy if it has a health component
                Health health = target.GetComponent<Health>();
                if (health != null)
                {
                    hitEnemiesThisAttack++;
                    // Calculate hit direction from the arrow's velocity
                    Vector2 hitDirection = rb != null && rb.velocity.magnitude > 0.1f ?
                        rb.velocity.normalized :
                        (hitPoint - (Vector2)transform.position).normalized;

                    health.TakeDamage(attackDamage, hitDirection, knockbackForce, knockbackDuration);
                }

                // Apply Hitstop & Screen Shake
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

            // Stop the arrow from moving
            if (rb != null && specialArrowType == SpecialArrowType.None)
            {
                rb.velocity = Vector2.zero;
                rb.isKinematic = true;
                canDamage = false;
                CreateEffectAndDestroy();
            }
            else if (specialArrowType == SpecialArrowType.Ricochet)
            {
                // Ricochet the arrow
                StartCoroutine(RicochetArrow());
            }
            else if (specialArrowType == SpecialArrowType.Pierce)
            {
                // Pierce the arrow
                StartCoroutine(PierceArrow());
            }

            
        }

        protected IEnumerator RicochetArrow()
        {
            // Ricochet the arrow
            yield return new WaitForSeconds(0.1f);

            // Check if the arrow should ricochet
            if (Random.value <= specialArrowChance && hitEnemiesThisAttack < maxEnemiesToHit)
            {
                // Calculate new direction based on the normal of the collision
                Vector2 newDirection = Vector2.Reflect(rb.velocity.normalized, rb.GetRelativeVector(Vector2.up));
                rb.velocity = newDirection * rb.velocity.magnitude;
            }
            else
            {
                // Stop the arrow from moving
                rb.velocity = Vector2.zero;
                rb.isKinematic = true;
                canDamage = false;
                CreateEffectAndDestroy();
            }
        }

        protected IEnumerator PierceArrow()
        {
            // Pierce the arrow
            yield return new WaitForSeconds(0.1f);

            // Check if the arrow should pierce
            if (Random.value <= specialArrowChance && hitEnemiesThisAttack < maxEnemiesToHit)
            {
                // Continue moving the arrow
            }
            else
            {
                // Stop the arrow from moving
                rb.velocity = Vector2.zero;
                rb.isKinematic = true;
                canDamage = false;
                CreateEffectAndDestroy();
            }
        }

        // Common code for both collision types
        void CreateEffectAndDestroy()
        {
            if (hitEffect != null)
            {
                GameObject effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
                Destroy(effect, effectTime);
            }
            Destroy(gameObject, arrowHitTime);
        }

        void DestroyArrow()
        {
            Destroy(gameObject, arrowHitTime);
        }
    }
}
