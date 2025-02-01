using UnityEngine;
using System.Collections;
using Scripts.Movements.Moves;

namespace Scripts.Movements.AI
{
    public abstract class MovementAI : MonoBehaviour
    {
        [SerializeField] protected Animator animator;  // Animation für Character
        [SerializeField] protected Rigidbody2D rb; // Rigidbody2D-Komponente
        [SerializeField] protected Transform GFX;

        [SerializeField] protected Walking walking;
        [SerializeField] protected Vector2 walkingInput;

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

        protected Walking GetWalking() {
            return walking;
        }

        protected void SetWalking(Walking walking) {
            this.walking = walking;
        }

        public Vector2 GetWalkingInput() {
            return walkingInput;
        }

        public void SetWalkingInput(Vector2 walkingInput) {
            this.walkingInput = walkingInput;
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

            Transform animatorTransform = transform.Find("Animator");
            if (animatorTransform != null)
            {
                this.SetAnimator(animatorTransform.GetComponent<Animator>());
            }
            if (this.GetAnimator() == null)
            {
                Debug.LogError("Animator component not found on " + gameObject.name);
            }

            this.SetWalking(GetComponent<Walking>());
            if (walking == null)
            {
                Debug.LogError("Walking component not found on " + gameObject.name);
            }

            this.SetWalkingInput(Vector2.zero);
        }

        

        protected virtual void FixedUpdate() {
            this.ProcessInputs();
            this.AnimateWalking();
        }
        protected virtual void Update() {
            
        }
        
        protected abstract void ProcessInputs();

        protected void Flip()
        {
            // spiegel Sprite sheet, damit Charakter nach links guckt während er nach links läuft 
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
            isFacingRight = !isFacingRight;
        }

        public void AnimateWalking()
        {   
            Vector2 moveInput = GetWalkingInput();
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