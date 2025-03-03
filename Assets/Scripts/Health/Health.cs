using UnityEngine;
using UnityEngine.SceneManagement;
using MyGame;
using System.Collections;
using Scripts.Combats;
using Scripts.Movements.Behaviours;
using Scripts.Characters.CharactersAnimation;
using Scripts.Characters;
using Scripts.Combats.CharacterCombats;
using System;
using Scripts.UI;


namespace Scripts.Healths
{
    public abstract class Health : MonoBehaviour
    {
        public static event Action OnEnemyDied;

        [SerializeField] public int maxHealth = 10; // Maximale Gesundheit
        [SerializeField] public int currentHealth; // Aktuelle Gesundheit

        [SerializeField] protected bool isInvincible = false; // Ist der Charakter unverwundbar?

        [SerializeField] public float invincibilityTime = 0.3f; // Zeit, in der der Charakter unverwundbar ist
        [SerializeField] public float combatDisabledTime = 0.5f; // Stärke des Rückstoßes
        [SerializeField] protected Combat combat;
        [SerializeField] protected Animator animator; // Referenz auf den Animator
        [SerializeField] protected CharacterAnimation characterAnimation; // Referenz auf die CharacterAnimation
        [SerializeField] protected Rigidbody2D rb; // Referenz auf den Rigidbody2D
        [SerializeField] protected Knockback knockback; // Referenz auf den Knockback

        [SerializeField] protected SpriteRenderer spriteRenderer; // Referenz auf den SpriteRenderer
        [SerializeField] public GameObject bloodParticlesPrefab; // Referenz zum Blut-Partikel-Prefab
        [SerializeField] protected IFrameHandler iframeHandler;

        [Header("Clash Properties")]
        [SerializeField] protected float clashSoundDuration = 0.7f;
        [SerializeField] protected Color clashColor = new Color(1f, 0.9f, 0.2f); // Golden yellow
        [SerializeField] protected float clashColorDuration = 0.1f;


        public IFrameHandler GetIFrameHandler()
        {
            return iframeHandler;
        }

        public bool GetIsInvincible()
        {
            return iframeHandler.GetIsInvincible();
        }

        public void SetIsInvincible(bool isInvincible)
        {
            iframeHandler.SetIsInvincible(isInvincible);
        }

        protected virtual void Start()
        {
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

            combat = GetComponentInChildren<Combat>();
            if (combat == null)
            {
                Debug.LogWarning("Combat not found on " + gameObject.name);
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

            iframeHandler = transform.root.GetComponentInChildren<IFrameHandler>();
            if (iframeHandler == null)
            {
                Debug.LogWarning("IFrameHandler not found on " + gameObject.name);
            }
        }

        abstract protected void initializeHealthBar(int maxHealth);
        abstract protected void updateHealthBar(int currentHealth, int maxHealth);

        public virtual void Heal(int amount)
        {
            // Erhöhe die Gesundheit
            if (currentHealth + amount > maxHealth)
            {
                amount = maxHealth - currentHealth;
            }
            currentHealth += amount;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

            updateHealthBar(currentHealth, maxHealth);
        }

        public virtual void TakeDamage(int damage, Vector2 hitDirection, float knockbackForce, float knockbackDuration)
        {
            if (iframeHandler == null || (iframeHandler != null && !iframeHandler.GetIsInvincible()))
            {
                Debug.Log(gameObject.name + " took damage: " + damage);
                StartCoroutine(CombatCooldown());
                currentHealth -= damage;
                Debug.Log("Current Health: " + currentHealth + "GameObject: " + gameObject.name + "damaged by " + damage);
                currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
                SoundManager.PlaySound(SoundType.HIT);

                // Update die Health Bar
                updateHealthBar(currentHealth, maxHealth);

                // Blutpartikel abspielen
                SpawnBloodParticles();
                SoundManager.PlaySound(SoundType.HURT);

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

                //StartCoroutine(InvincibiltyTimer());
            }

            if (iframeHandler != null)
            {
                iframeHandler.TriggerInvincibility();
                Debug.Log("Triggering invincibility frames from PlayerHealth"); // Debug log
            }
            else
            {
                Debug.LogWarning("IFrameHandler not found on " + gameObject.name);
            }
        }

        public virtual void HandleSwordClash(Vector2 clashDirection, float clashForce, float clashDuration)
        {
            if (knockback == null)
            {
                Debug.LogWarning("Knockback component not found, can't apply clash effect");
                return;
            }

            // Play clash effect sound
            SoundManager.PlaySound(SoundType.SWING, clashSoundDuration);

            // Apply stronger knockback for the clash
            StartCoroutine(knockback.KnockbackCharacter(rb, clashDirection, clashForce, clashDuration));

            // Optionally flash the sprite a different color for clash
            // Flash gold color for clash
            CharacterGFX characterGFX = GetComponentInChildren<CharacterGFX>();
            if (characterGFX != null)
            {
                characterGFX.FlashColor(clashColor, clashColorDuration);
            }
        }

        protected IEnumerator CombatCooldown()
        {
            combat.SetCombatEnabled(false);
            yield return new WaitForSeconds(combatDisabledTime);
            combat.SetCombatEnabled(true);
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

        protected virtual void Die()
        {
            OnEnemyDied?.Invoke();
            
        }

        protected virtual void Hurt()
        {

        }
    }
}