using UnityEngine;
using System.Collections;
using Scripts.UI;
using Scripts.Combats;

namespace Scripts.Healths {
    public abstract class Health : MonoBehaviour {
        public int maxHealth = 10; // Maximale Gesundheit
        public int currentHealth; // Aktuelle Gesundheit

        protected bool isInvincible = false; // Ist der Charakter unverwundbar?

        public float invincibilityTime = 0.5f; // Zeit, in der der Charakter unverwundbar ist
        public float knockbackDuration = 0.2f; // Dauer des Rückstoßes


        public Animator animator; // Referenz auf den Animator
        protected Rigidbody2D rb; // Referenz auf den Rigidbody2D

        public SpriteRenderer spriteRenderer; // Referenz auf den SpriteRenderer
        public GameObject bloodParticlesPrefab; // Referenz zum Blut-Partikel-Prefab

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

        
        public virtual void TakeDamage(int damage, Vector2 hitDirection, float knockbackForce)
        {
            // Reduziere die Gesundheit
            if(!isInvincible)
            {
                
                currentHealth -= damage;
                currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

                // Update die Health Bar
                updateHealthBar(currentHealth, maxHealth);

                // Blutpartikel abspielen
                SpawnBloodParticles();

                //Rot aufleuchten nach Damage
                StartCoroutine(FlashRed());

                // Rückstoß anwenden
                StartCoroutine(ApplyKnockback(hitDirection, knockbackForce));

                // Überprüfe, ob der Spieler tot ist
                if (currentHealth <= 0)
                {
                    Die();
                }

                StartCoroutine(InvincibiltyTimer());
            }
        }

        protected IEnumerator ApplyKnockback(Vector2 hitDirection, float knockbackForce)
        {   
            if (rb != null)
            {
                Debug.Log("Applying knockback");
                float timer = 0;
                while(timer <= knockbackDuration)
                {
                    rb.velocity = hitDirection * (knockbackForce / knockbackDuration) * Time.deltaTime;
                    timer += Time.deltaTime;
                    yield return null;
                }
                rb.velocity = Vector2.zero;
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
            // Hier kannst du eine Logik für den Tod des Spielers einfügen
        }
        
    }
}