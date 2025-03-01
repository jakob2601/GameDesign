using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

namespace Scripts.Characters
{
    public class CharacterGFX : MonoBehaviour
    {
        [SerializeField] protected SpriteRenderer spriteRenderer;
        [SerializeField] protected bool isFacingRight = false; // der Charakter wendet sich rechte Seite zu
        protected Vector3 originalScale;
        protected bool canFlip = true;
        protected Color originalColor;
         

        protected Vector3 GetOriginalScale()
        {
            return originalScale;
        }

        protected void SetOriginalScale(Vector3 originalScale)
        {
            this.originalScale = originalScale;
        }



        // Start is called before the first frame update
        protected virtual void Start()
        {
            // Speichere die ursprüngliche Skalierung
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                Debug.LogWarning("SpriteRenderer not found on " + gameObject.name);
            }
            else
            {
                originalColor = spriteRenderer.color;
            }

            originalScale = transform.localScale;
        }

        // Update is called once per frame
        protected virtual void Update()
        {

        }

        public void DisableFlip()
        {
            canFlip = false;
        }

        public void EnableFlip()
        {
            canFlip = true;
        }

        protected void Flip()
        {
            if (canFlip)
            {
                // spiegel Sprite sheet, damit Charakter nach links guckt während er nach links läuft 
                Vector3 scale = transform.localScale;
                scale.x *= -1;
                transform.localScale = scale;
                isFacingRight = !isFacingRight;
            }
        }

        public void FlashColor(Color color, float duration)
        {
            StartCoroutine(FlashColorCoroutine(color, duration));
        }

        protected IEnumerator FlashColorCoroutine(Color color, float duration)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.color = color;
                yield return new WaitForSeconds(duration);
                spriteRenderer.color = originalColor;
            }
        }

        public void UpdateScale(Vector2 force)
        {
            if (force.x >= 0.01f)
            {
                transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
            }
            else if (force.x <= -0.01f)
            {
                transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
            }
        }
        
    }
}

