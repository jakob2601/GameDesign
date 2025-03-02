using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame;
using Scripts.Healths;
using Scripts.Combats.CharacterCombats;
using Scripts.Combats.Features;

namespace Scripts.Combats.Weapons
{
    public class Arrow : MonoBehaviour
    {
        [Header("Arrow Properties")]
        [SerializeField] protected GameObject hitEffect;
        [SerializeField] protected float effectTime = 0.8f;
        [SerializeField] protected float timeToLive = 5f;
        [SerializeField] protected float arrowHitTime = 0.2f;
        
        [Header("Special Arrow Properties")]
        [SerializeField] protected bool ricochet = false;
        [SerializeField] protected float ricochetChance = 0f;
        [SerializeField] protected bool canPierce = false;
        [SerializeField] protected float pierceChance = 0f;

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
            if (rb != null && !ricochet && !canPierce)
            {
                rb.velocity = Vector2.zero;
                rb.isKinematic = true;
                canDamage = false;
            }

            CreateEffectAndDestroy();
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
