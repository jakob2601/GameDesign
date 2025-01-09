using UnityEngine;
using System.Collections;
using Scripts.Movements.Moves;
using MyGame;

namespace Scripts.Movements.AI
{
    public class PlayerMovementAI: MovementAI
    {
        private Dash dash;
        private Walking walking;
        Vector2 walkingInput;

        protected Dash GetDash() {
            return dash;
        }

        protected void SetDash(Dash dash) {
            this.dash = dash;
        }   

        protected Walking GetWalking() {
            return walking;
        }

        protected void SetWalking(Walking walking) {
            this.walking = walking;
        }

        protected Vector2 GetWalkingInput() {
            return walkingInput;
        }

        protected void SetWalkingInput(Vector2 walkingInput) {
            this.walkingInput = walkingInput;
        }

        protected override void Start()
        {
            base.Start();

            Transform animatorTransform = transform.Find("Animator");
            if (animatorTransform != null)
            {
                this.SetAnimator(animatorTransform.GetComponent<Animator>());
            }
            if (this.GetAnimator() == null)
            {
                Debug.LogError("Animator component not found on " + gameObject.name);
            }

            this.SetDash(GetComponent<Dash>());
            if (dash == null)
            {
                Debug.LogError("Dash component not found on " + gameObject.name);
            }

            this.SetWalking(GetComponent<Walking>());
            if (walking == null)
            {
                Debug.LogError("Walking component not found on " + gameObject.name);
            }

            this.SetWalkingInput(Vector2.zero);
        }

        protected override void Update()
        {
            this.SetWalkingInput(ProccessInputs());
            this.AnimateWalking(walkingInput);

            if (walkingInput.x < 0 && !this.isFacingRight || walkingInput.x > 0 && this.isFacingRight)
            {
                this.Flip();
            }

            // Dash-Mechanik prüfen
            if (Input.GetKeyDown(KeyCode.Space) && dash.GetCurrentDashCooldown() <= 0f && !isDashing && walkingInput != Vector2.zero)
            {
                StartCoroutine(this.PerformPlayerDash());
            }
        }

        protected override void FixedUpdate()
        {
            // Normale Bewegung, wenn nicht gedasht wird und Bewegungseingabe vorhanden ist
            if (!isDashing && walkingInput.magnitude > 0.01f) // Schwellenwert für Bewegung
            {
                rb.MovePosition(walking.getNewPosition(rb.position, walkingInput));
            }
        }

        protected Vector2 ProccessInputs()
        {
            float moveX = Input.GetAxisRaw("Horizontal");
            float moveY = Input.GetAxisRaw("Vertical");

            // Toleranz für kleine Eingabewerte
            if (Mathf.Abs(moveX) < 0.1f) moveX = 0;
            if (Mathf.Abs(moveY) < 0.1f) moveY = 0;

            if (moveX != 0 || moveY != 0)
            {
                this.SetLastMoveDirection(new Vector2(moveX, moveY).normalized);
            }

            return new Vector2(moveX, moveY).normalized;
        }

        private IEnumerator PerformPlayerDash()
        {
            this.SetIsDashing(true);
            yield return StartCoroutine(dash.PerformDash(rb, walkingInput));
            this.SetIsDashing(false);
        }
    }
}
