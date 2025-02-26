using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame;
using Scripts.Healths;
using Scripts.Combats.CharacterCombats;

namespace Scripts.Combats.Weapons
{
    public class Arrow : MonoBehaviour
    {
        [Header("Arrow Properties")]
        [SerializeField] protected GameObject hitEffect;
        [SerializeField] protected float effectTime = 0.8f;
        [SerializeField] protected float timeToLive = 5f;
        
        // Properties that were previously inherited from Weapon
        [Header("Attack Properties")]
        [SerializeField] protected LayerMask enemyLayer;
        [SerializeField] protected int attackDamage = 10;
        [SerializeField] protected float knockbackForce = 1f;
        [SerializeField] protected float knockbackDuration = 0.2f;
        
        private Rigidbody2D rb;
        
        void Start()
        {
            // Start the self-destruct timer
            Destroy(gameObject, timeToLive);
            
            // Get the rigidbody for direction information
            rb = GetComponent<Rigidbody2D>();
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
        
        void OnCollisionEnter2D(Collision2D collision)
        {
            HitObject(collision);
        }
        
        void OnTriggerEnter2D(Collider2D collider)
        {
            HitObject(collider);
        }

        // For collisions
        void HitObject(Collision2D collision)
        {
            Debug.Log("Arrow collided with: " + collision.gameObject.name);
            
            // Check if we hit something in the enemy layer
            if (((1 << collision.gameObject.layer) & enemyLayer) != 0)
            {
                // Apply damage to enemy if it has a health component
                Health health = collision.gameObject.GetComponent<Health>();
                if (health != null)
                {
                    // Calculate hit direction from the arrow's velocity
                    Vector2 hitDirection = rb != null && rb.velocity.magnitude > 0.1f ? 
                        rb.velocity.normalized : 
                        (collision.transform.position - transform.position).normalized;
                    
                    health.TakeDamage(attackDamage, hitDirection, knockbackForce, knockbackDuration);
                }
            }
            
            CreateEffectAndDestroy();
        }
        
        // For triggers
        void HitObject(Collider2D collider)
        {
            Debug.Log("Arrow triggered with: " + collider.gameObject.name);
            
            // Check if we hit something in the enemy layer
            if (((1 << collider.gameObject.layer) & enemyLayer) != 0)
            {
                // Apply damage to enemy if it has a health component
                Health health = collider.gameObject.GetComponent<Health>();
                if (health != null)
                {
                    // Calculate hit direction from the arrow's velocity
                    Vector2 hitDirection = rb != null && rb.velocity.magnitude > 0.1f ? 
                        rb.velocity.normalized : 
                        (collider.transform.position - transform.position).normalized;
                    
                    health.TakeDamage(attackDamage, hitDirection, knockbackForce, knockbackDuration);
                }
            }
            
            CreateEffectAndDestroy();
        }
        
        // Common code for both collision types
        void CreateEffectAndDestroy()
        {
            if(hitEffect != null)
            {
                GameObject effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
                Destroy(effect, effectTime);
            }
            Destroy(gameObject);
        }
        
        void DestroyArrow()
        {
            Destroy(gameObject);
        }
    }
}
