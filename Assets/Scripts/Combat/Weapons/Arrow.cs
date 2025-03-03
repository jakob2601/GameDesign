using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame;
using Scripts.Healths;
using Scripts.Combats.CharacterCombats;
using Scripts.Combats.Features;
using Scripts.Characters;
using Scripts.Items;

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

        [SerializeField] private LayerMask sameTeamLayer; // Layer of characters that arrows should pass through

        private Rigidbody2D rb;
        private GameObject playerObject;
        private HashSet<GameObject> hitEnemies = new HashSet<GameObject>(); // Track hit enemies

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

            // Ignore collisions with other arrows
            IgnoreOtherArrows();

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

        private void IgnoreOtherArrows()
        {
            // Find all existing arrows and ignore collisions with them
            Arrow[] allArrows = FindObjectsOfType<Arrow>();
            Collider2D thisCollider = GetComponent<Collider2D>();

            foreach (Arrow otherArrow in allArrows)
            {
                if (otherArrow != this && otherArrow.gameObject != this.gameObject)
                {
                    Collider2D otherCollider = otherArrow.GetComponent<Collider2D>();
                    if (otherCollider != null && thisCollider != null)
                    {
                        Physics2D.IgnoreCollision(thisCollider, otherCollider, true);
                    }
                }
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

        // Add this method to set the character layer
        public void SetSameTeamLayer(LayerMask layer)
        {
            sameTeamLayer = layer;
            // Debug.Log($"Arrow will pass through layer: {LayerMaskToString(sameTeamLayer)}");
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
            // Skip collision with other arrows
            if (target.GetComponent<Arrow>() != null)
            {
                return;
            }

            // Check if it's the player or child of player (failsafe)
            if (playerObject != null &&
                (target == playerObject || target.transform.IsChildOf(playerObject.transform)))
            {
                return; // Skip collision with player
            }
            
            bool didntHitWall = true;

            // Check if we hit a teammate - if so, IGNORE the collision and continue
            if (((1 << target.layer) & sameTeamLayer) != 0 ||
                (target.GetComponentInChildren<Combat>() != null && ((1 << target.layer) & enemyLayer) == 0))
            {
                Debug.Log($"Arrow passing through {target.name} - same team layer match");
                
                // Ignore collision with this teammate
                Collider2D arrowCollider = GetComponent<Collider2D>();
                Collider2D[] targetColliders = target.GetComponentsInChildren<Collider2D>();
                foreach (Collider2D col in targetColliders)
                {
                    Physics2D.IgnoreCollision(arrowCollider, col, true);
                }
                
                // Important: Don't process anything else, just continue flying
                return;
            }

            // Rest of your collision handling code...
            // Check if we hit something in the enemy layer
            if (((1 << target.layer) & enemyLayer) != 0 && canDamage)
            {
                // Check if the enemy has already been hit
                if (hitEnemies.Contains(target))
                {
                    return; // Skip if already hit
                }

                // Apply damage to enemy if it has a health component
                Health health = target.GetComponent<Health>();
                if (health != null)
                {
                    hitEnemies.Add(target); // Add to hit enemies list
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
            else if (target.GetComponent<Item>() != null)
            {
                didntHitWall = true;
            }
            else
            {
                didntHitWall = false;
            }

            // Stop the arrow from moving
            if (rb != null && !didntHitWall || (specialArrowType == SpecialArrowType.None && hitEnemiesThisAttack > 0) || (hitEnemiesThisAttack >= maxEnemiesToHit))
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
            if (Random.value <= specialArrowChance)
            {
                // Calculate new direction based on the normal of the collision
                Vector2 newDirection = Vector2.Reflect(rb.velocity.normalized, rb.GetRelativeVector(Vector2.up));
                rb.velocity = newDirection * rb.velocity.magnitude;
                rb.rotation = Mathf.Atan2(newDirection.y, newDirection.x) * Mathf.Rad2Deg; // Ensure the arrow points in the new direction
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
            if (Random.value <= specialArrowChance)
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
