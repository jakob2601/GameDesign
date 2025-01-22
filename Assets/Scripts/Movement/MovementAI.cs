using UnityEngine;
using System.Collections;
using Scripts.Movements.Moves;

namespace Scripts.Movements.AI
{
    public abstract class MovementAI : MonoBehaviour
    {
        [SerializeField] public Animator animator;  // Animation für Character
        [SerializeField] protected Rigidbody2D rb; // Rigidbody2D-Komponente
        [SerializeField] protected Transform GFX;

        protected bool isFacingRight = false; // der Charakter wendet sich rechte Seite zu
        [SerializeField] protected bool isDashing = false; // Ob der Spieler aktuell dashen kann
        [SerializeField] public Vector2 lastMoveDirection; // letzte Bewegungsrichtung
        [SerializeField] public Vector3 originalScale; // Ursprüngliche Skalierung des Charakters

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

        }
        protected virtual void Update() {
            this.SetWalkingInput(ProccessInputs());
            this.AnimateWalking();

            if (walkingInput.x < 0 && !this.isFacingRight || walkingInput.x > 0 && this.isFacingRight)
            {
                this.Flip();
            }

        }
        
        protected Vector2 ProccessInputs() {
            float moveX = Input.GetAxisRaw("Horizontal");
            float moveY = Input.GetAxisRaw("Vertical");

            // Toleranz für kleine Eingabewerte
            if (Mathf.Abs(moveX) < 0.1f) moveX = 0;
            if (Mathf.Abs(moveY) < 0.1f) moveY = 0;

            if (moveX != 0 || moveY != 0)
            {
                this.SetLastMoveDirection(new Vector2(moveX, moveY).normalized);
                animator.SetBool("IsMoving", true);
            }
            else {
                animator.SetBool("IsMoving", false);
            }

            return new Vector2(moveX, moveY).normalized;
        }

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