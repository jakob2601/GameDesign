using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Characters
{
    public class IFrameHandler : MonoBehaviour
    {
        [SerializeField] private float invincibilityDuration = 1.0f;
        [SerializeField] private float flashInterval = 0.1f;
        [SerializeField] private bool flashDuringInvincibility = true;

        private bool isInvincible = false;
        private SpriteRenderer spriteRenderer;

        public bool GetIsInvincible()
        {
            return isInvincible;
        }

        protected void SetIsInvincible(bool isInvincible)
        {
            this.isInvincible = isInvincible;
        }

        protected void Awake()
        {
            // Get the sprite renderer component for visual feedback
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            }
        }

        // Method called by PlayerHealth when taking damage
        public void TriggerInvincibility()
        {
            if (!isInvincible)
            {
                StartCoroutine(InvincibilityCoroutine());
                if (flashDuringInvincibility && spriteRenderer != null)
                {
                    StartCoroutine(FlashCoroutine());
                }
            }
        }
        /*
        // Method called by PlayerHealth when taking damage
        public void TriggerInvincibility()
        {
            if (!isInvincible)
            {
                StartCoroutine(InvincibilityCoroutine());
                if (flashDuringInvincibility && spriteRenderer != null)
                {
                    StartCoroutine(FlashCoroutine());
                }
            }
        }
        */

        private IEnumerator InvincibilityCoroutine()
        {
            isInvincible = true;
            Debug.Log("Invincibility started");

            yield return new WaitForSeconds(invincibilityDuration);

            isInvincible = false;
            Debug.Log("Invincibility ended");
        }
        /*
        private IEnumerator FlashCoroutine()
        {
            Color originalColor = spriteRenderer.color;

            // Flash the sprite until invincibility ends
            while (isInvincible)
            {
                // Toggle visibility
                spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0.5f);
                yield return new WaitForSeconds(flashInterval);
                spriteRenderer.color = originalColor;
                yield return new WaitForSeconds(flashInterval);
            }

            // Ensure sprite is fully visible when invincibility ends
            spriteRenderer.color = originalColor;
        }
        */

        protected IEnumerator FlashCoroutine()
        {
            CharacterGFX characterGFX = GetComponentInChildren<CharacterGFX>();
            if (characterGFX != null)
            {
                Color originalColor = spriteRenderer.color;
                Color flashColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0.5f);
                while (isInvincible)
                {
                    characterGFX.FlashColor(flashColor, flashInterval);
                    WaitForSeconds wait = new WaitForSeconds(flashInterval * 2);
                    yield return wait;
                }
            }
        }
    }
}
