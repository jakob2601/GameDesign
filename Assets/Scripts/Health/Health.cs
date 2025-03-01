// Assets/Scripts/Health/Health.cs
using System.Collections;
using UnityEngine;
using Scripts.Combats; // Ensure this using directive is present
using UnityEngine;
using MyGame;
using System.Collections;
using Scripts.UI;
using Scripts.Combats;
using Scripts.Movements.Behaviours;
using Scripts.Characters.CharactersAnimation;
using Scripts.Characters;
using Scripts.Combats.CharacterCombats;
using System;

namespace Scripts.Healths
{
    public abstract class Health : MonoBehaviour
    {
        [SerializeField] protected int maxHealth;
        [SerializeField] protected int currentHealth;
        [SerializeField] protected bool isInvincible;
        [SerializeField] protected float invincibilityTime;
        [SerializeField] protected GameObject bloodParticlesPrefab;
        [SerializeField] protected Combat combat;
        [SerializeField] protected Knockback knockback;
        [SerializeField] protected Animator animator;
        [SerializeField] protected CharacterAnimation characterAnimation;
        [SerializeField] protected Color clashColor;
        [SerializeField] protected float clashColorDuration;
        [SerializeField] protected float combatDisabledTime;
        [SerializeField] protected float clashSoundDuration;

        protected virtual void Awake()
        {
            currentHealth = maxHealth;
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
            currentHealth += amount;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            updateHealthBar(currentHealth, maxHealth);
        }

        public virtual void TakeDamage(int damage, Vector2 hitDirection, float knockbackForce, float knockbackDuration)
        {
            Debug.Log(gameObject.name + " took damage: " + damage);
            if (!isInvincible)
            {
                StartCoroutine(CombatCooldown());
                int initialHealth = currentHealth;
                currentHealth -= damage;
                Debug.Log("Current Health: " + currentHealth + " GameObject: " + gameObject.name + " damaged by " + damage);
                currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
                SoundManager.PlaySound(SoundType.HIT);

                updateHealthBar(currentHealth, maxHealth);

                SpawnBloodParticles();
                SoundManager.PlaySound(SoundType.HURT);

                if (currentHealth < initialHealth)
                {
                    ApplyHitstopAndShake();
                }

                if (currentHealth <= 0)
                {
                    Die();
                }
                else
                {
                    Hurt();
                }

                StartCoroutine(knockback.KnockbackCharacter(rb, hitDirection, knockbackForce, knockbackDuration));
                StartCoroutine(InvincibiltyTimer());
            }
            else
            {
                Debug.Log("Player is invincible");
            }
        }

        public virtual void HandleSwordClash(Vector2 clashDirection, float clashForce, float clashDuration)
        {
            if (knockback == null)
            {
                Debug.LogWarning("Knockback component not found, can't apply clash effect");
                return;
            }

            SoundManager.PlaySound(SoundType.SWING, clashSoundDuration);
            StartCoroutine(knockback.KnockbackCharacter(rb, clashDirection, clashForce, clashDuration));

            CharacterGFX characterGFX = GetComponentInChildren<CharacterGFX>();
            if (characterGFX != null)
            {
                characterGFX.FlashColor(clashColor, clashColorDuration);
            }
        }

        protected IEnumerator InvincibiltyTimer()
        {
            isInvincible = true;
            yield return new WaitForSeconds(invincibilityTime);
            isInvincible = false;
        }

        protected IEnumerator CombatCooldown()
        {
            combat.SetCombatEnabled(false);
            yield return new WaitForSeconds(combatDisabledTime);
            combat.SetCombatEnabled(true);
        }

        public void setInvincibility(bool state)
        {
            isInvincible = state;
        }

        void SpawnBloodParticles()
        {
            if (bloodParticlesPrefab != null)
            {
                GameObject tempParticle = Instantiate(bloodParticlesPrefab, transform.position, Quaternion.identity);
                Destroy(tempParticle, 2f);
            }
            else
            {
                Debug.LogWarning("No blood particle prefab assigned!");
            }
        }

        public int GetCurrentHealth()
        {
            return currentHealth;
        }

        protected virtual void Die()
        {
            OnEnemyDied?.Invoke();
        }

        protected virtual void Hurt()
        {

        }

        private void ApplyHitstopAndShake()
        {
            Hitstop hitstop = FindObjectOfType<Hitstop>();
            ScreenShake screenShake = FindObjectOfType<ScreenShake>();

            if (hitstop != null)
            {
                hitstop.SetHitstopDuration(0.1f);
                StartCoroutine(hitstop.ApplyHitstop());
            }

            if (screenShake != null)
            {
                StartCoroutine(screenShake.Shake());
            }
        }
    }
}