using UnityEngine;
using MyGame;
using System.Collections;
using Scripts.UI;
using Scripts.Combats;
using Scripts.Movements.Behaviours;
using Scripts.Characters.CharactersAnimation;

namespace Scripts.Healths {
    public abstract class Health : MonoBehaviour {
        [SerializeField] public int maxHealth = 10; // Maximale Gesundheit
        [SerializeField] public int currentHealth; // Aktuelle Gesundheit

        [SerializeField] protected bool isInvincible = false; // Ist der Charakter unverwundbar?

        [SerializeField] public float invincibilityTime = 1f; // Zeit, in der der Charakter unverwundbar ist
        //[SerializeField] public float knockbackDuration = 0.2f; // Dauer des Rückstoßes


        [SerializeField] public Animator animator; // Referenz auf den Animator
        [SerializeField] public CharacterAnimation characterAnimation; // Referenz auf die CharacterAnimation
        [SerializeField] protected Rigidbody2D rb; // Referenz auf den Rigidbody2D
        [SerializeField] protected Knockback knockback; // Referenz auf den Knockback

        [SerializeField] public SpriteRenderer spriteRenderer; // Referenz auf den SpriteRenderer
        [SerializeField] public GameObject bloodParticlesPrefab; // Referenz zum Blut-Partikel-Prefab

        protected virtual void Start() {
            // Setze die Gesundheit auf das Maximum
            currentHealth = maxHealth;

            initializeHealthBar(maxHealth);
            updateHealthBar(currentHealth, maxHealth);

            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                Debug.LogWarning("SpriteRenderer not found on " + gameObject.name);
            }
            
            rb = GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                Debug.LogError("Rigidbody2D component not found on " + gameObject.name);
            }

            knockback = GetComponent<Knockback>();
            if (knockback == null)
            {
                Debug.LogWarning("Knockback not found on " + gameObject.name);
            }

            animator = GetComponentInChildren<Animator>();
            if (animator == null)
            {
                Debug.LogWarning("Animator not found on " + gameObject.name);
            }

            characterAnimation = GetComponentInChildren<CharacterAnimation>();
            if (characterAnimation == null)
            {
                Debug.LogWarning("CharacterAnimation not found on " + gameObject.name);
            }
        }

        abstract protected void initializeHealthBar(int maxHealth);
        abstract protected void updateHealthBar(int currentHealth, int maxHealth);

        public virtual void Heal(int amount)
        {
            // Erhöhe die Gesundheit
            currentHealth += amount;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

            updateHealthBar(currentHealth, maxHealth);
        }

        
        public virtual void TakeDamage(int damage, Vector2 hitDirection, float knockbackForce, float knockbackDuration)
        {
            Debug.Log(gameObject.name + " took damage: " + damage);
            // Reduziere die Gesundheit
            if(!isInvincible)
            {
                currentHealth -= damage;
                Debug.Log("Current Health: " + currentHealth + "GameObject: " + gameObject.name + "damaged by " + damage);
                currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
                SoundManager.PlaySound(SoundType.HIT);
            
                // Update die Health Bar
                updateHealthBar(currentHealth, maxHealth);

                // Blutpartikel abspielen
                SpawnBloodParticles();
                SoundManager.PlaySound(SoundType.HURT);

                //Rot aufleuchten nach Damage
                StartCoroutine(FlashRed());

                // Überprüfe, ob der Spieler tot ist
                if (currentHealth <= 0)
                {
                    Die();
                    //return;
                }
                else 
                {
                    Hurt();
                }
                // Rückstoß anwenden
                StartCoroutine(knockback.KnockbackCharacter(rb, hitDirection, knockbackForce, knockbackDuration));

                StartCoroutine(InvincibiltyTimer());
            }
            else 
            {
                Debug.Log("Player is invincible");
            }
        }

        protected IEnumerator InvincibiltyTimer() 
        {
            isInvincible = true;
            yield return new WaitForSeconds(invincibilityTime);
            isInvincible = false;
        }

        void SpawnBloodParticles()
        {
            if (bloodParticlesPrefab != null)
            {
                // Erstelle die Partikel an der Position des Gegners
                GameObject tempParticle = Instantiate(bloodParticlesPrefab, transform.position, Quaternion.identity);
                Destroy(tempParticle, 2f); // Zerstöre die Partikel nach 2 Sekunden
            }
            else
            {
                Debug.LogWarning("No blood particle prefab assigned!");
            }
        }

        protected IEnumerator FlashRed()
        {
            // Farbänderung nach Damage 
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = Color.white;
        }

        protected virtual void Die()
        {
            
        }

        protected virtual void Hurt()
        {
           
        }
        
    }
}