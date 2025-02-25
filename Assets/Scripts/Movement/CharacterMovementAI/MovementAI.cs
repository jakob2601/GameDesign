using UnityEngine;
using System.Collections;
using Scripts.Movements.Moves;
using Scripts.Characters.CharactersAnimation;

namespace Scripts.Movements.AI
{
    public abstract class MovementAI : MonoBehaviour
    {
        [SerializeField] protected Animator animator;  
        [SerializeField] protected CharacterAnimation characterAnimation; 
        [SerializeField] protected Rigidbody2D rb; 
        [SerializeField] protected Walking walking;
        [SerializeField] protected Vector2 walkingInput;

        [SerializeField] protected bool isWalking = false; // Ob der Charakter sich bewegt
        [SerializeField] protected bool isDashing = false; // Ob der Spieler aktuell dashen kann

        [SerializeField] public Vector2 lastMoveDirection; // letzte Bewegungsrichtung


        public Animator GetAnimator()
        {
            return animator;
        }

        protected void SetAnimator(Animator animator)
        {
            this.animator = animator;
        }

        protected Rigidbody2D GetRigidbody2D()
        {
            return rb;
        }

        protected void SetRigidbody2D(Rigidbody2D rb)
        {
            this.rb = rb;
        }

        protected bool GetIsDashing()
        {
            return isDashing;
        }

        protected void SetIsDashing(bool isDashing)
        {
            this.isDashing = isDashing;
        }

        protected Walking GetWalking()
        {
            return walking;
        }

        protected void SetWalking(Walking walking)
        {
            this.walking = walking;
        }

        public Vector2 GetWalkingInput()
        {
            return walkingInput;
        }

        public void SetWalkingInput(Vector2 walkingInput)
        {
            this.walkingInput = walkingInput;
        }

        public Vector2 GetLastMoveDirection()
        {
            return lastMoveDirection;
        }

        public void SetLastMoveDirection(Vector2 lastMoveDirection)
        {
            this.lastMoveDirection = lastMoveDirection;
        }

        protected virtual void Start()
        {
            rb = GetComponent<Rigidbody2D>(); // Rigidbody2D zuweisen
    
            Transform animatorTransform = transform.Find("Animator");
            if (animatorTransform != null)
            {
                this.SetAnimator(animatorTransform.GetComponent<Animator>());
                characterAnimation = animatorTransform.GetComponent<CharacterAnimation>();
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



        protected virtual void FixedUpdate()
        {
            this.ProcessInputs();
            this.AnimateWalking();
        }
        protected virtual void Update()
        {

        }

        protected abstract void ProcessInputs();

        

        public void AnimateWalking()
        {
            Vector2 moveInput = GetWalkingInput();
            characterAnimation.SetMovementAnimation(moveInput);
        }

        
    }
}