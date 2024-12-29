using UnityEngine;
using System.Collections;

namespace Scripts.Movements.AI
{
    public abstract class MovementAI : MonoBehaviour
    {
        [SerializeField] protected Animator animator;  // Animation für Character
        [SerializeField] protected Rigidbody2D rb; // Rigidbody2D-Komponente
        [SerializeField] protected Transform GFX;

        protected bool isFacingRight = false; // der Charakter wendet sich rechte Seite zu
        [SerializeField] protected bool isDashing = false; // Ob der Spieler aktuell dashen kann
        [SerializeField] public Vector2 lastMoveDirection; // letzte Bewegungsrichtung
        [SerializeField] public Vector3 originalScale; // Ursprüngliche Skalierung des Charakters

        public Animator GetAnimator() {
            return animator;
        }

        protected void SetAnimator(Animator animator) {
            this.animator = animator;
        }

        protected Rigidbody2D GetRigidbody2D() {
            return rb;
        }

        protected void SetRigidbody2D(Rigidbody2D rb) {
            this.rb = rb;
        }

        protected Transform GetGFX() {
            return GFX;
        }

        protected void SetGFX(Transform GFX) {
            this.GFX = GFX;
        }

        protected bool GetIsFacingRight() {
            return isFacingRight;
        }

        protected void SetIsFacingRight(bool isFacingRight) {
            this.isFacingRight = isFacingRight;
        }  

        protected bool GetIsDashing() {
            return isDashing;
        }

        protected void SetIsDashing(bool isDashing) {
            this.isDashing = isDashing;
        }

        public Vector2 GetLastMoveDirection() {
            return lastMoveDirection;
        }

        public void SetLastMoveDirection(Vector2 lastMoveDirection) {
            this.lastMoveDirection = lastMoveDirection;
        }

        protected Vector3 GetOriginalScale() {
            return originalScale;
        }

        protected void SetOriginalScale(Vector3 originalScale) {
            this.originalScale = originalScale;
        }

        protected virtual void Start()
        {
            rb = GetComponent<Rigidbody2D>(); // Rigidbody2D zuweisen
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

        public void AnimateWalking(Vector2 moveInput)
        {
            animator.SetFloat("Horizontal", moveInput.x); // Setzen horizontale Bewegung zur Animation
            animator.SetFloat("Vertical", moveInput.y); // Setzen verticale Bewegung zur Animation
            animator.SetFloat("Speed", moveInput.sqrMagnitude); // Bewegungsgeschwindigkeit

            //Blickrichtung für Idle Animation
            animator.SetFloat("StayHorizontal", lastMoveDirection.x);
            animator.SetFloat("StayVertical", lastMoveDirection.y);
        }

        public void UpdateScale(Vector2 force)
        {
            if (force.x >= 0.01f)
            {
                GFX.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
            }
            else if (force.x <= -0.01f)
            {
                GFX.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
            }
        }
    }
}