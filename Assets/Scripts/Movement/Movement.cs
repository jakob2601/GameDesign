using UnityEngine;
using System.Collections;

namespace Scripts.Movements
{
    public abstract class Movement : MonoBehaviour
    {
        public Animator animator;  // Animation für Character
        protected Rigidbody2D rb; // Rigidbody2D-Komponente

        protected bool isFacingRight = false; // der Charakter wendet sich rechte Seite zu
        protected bool isDashing = false; // Ob der Spieler aktuell dashen kann
        public float moveSpeed = 150f; 
        public Vector2 lastMoveDirection; // letzte Bewegungsrichtung

        protected virtual void Start()
        {
            rb = GetComponent<Rigidbody2D>(); // Rigidbody2D zuweisen
            animator = GetComponent<Animator>(); // Animator zuweisen
        }

        protected abstract void FixedUpdate();
        protected abstract void Update();

        protected void Flip()
        {
            // spiegel Sprite sheet, damit Charakter nach links guckt während er nach links läuft 
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
            isFacingRight = !isFacingRight;
        }

        protected void AnimateWalking(Vector2 moveInput)
        {
            animator.SetFloat("Horizontal", moveInput.x); // Setzen horizontale Bewegung zur Animation
            animator.SetFloat("Vertical", moveInput.y); // Setzen verticale Bewegung zur Animation
            animator.SetFloat("Speed", moveInput.sqrMagnitude); // Bewegungsgeschwindigkeit

            //Blickrichtung für Idle Animation
            animator.SetFloat("StayHorizontal", lastMoveDirection.x);
            animator.SetFloat("StayVertical", lastMoveDirection.y);
        }
    }
}