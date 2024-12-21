using UnityEngine;
using System.Collections;
using Scripts.Movements.Moves;

namespace Scripts.Movements.AI
{
    public class PlayerMovementAI: MovementAI
    {
        private Dash dash;
        private Walking walking;
        Vector2 walkingInput;
        protected override void Start()
        {
            base.Start();

            animator = GetComponent<Animator>(); // Animator zuweisen

            dash = GetComponent<Dash>();
            if (dash == null)
            {
                Debug.LogError("Dash component not found on " + gameObject.name);
            }

            walking = GetComponent<Walking>();
            if (walking == null)
            {
                Debug.LogError("Walking component not found on " + gameObject.name);
            }

            walkingInput = Vector2.zero;
        }

        protected override void Update()
        {
            walkingInput = ProccessInputs();
            AnimateWalking(walkingInput);

            if (walkingInput.x < 0 && !isFacingRight || walkingInput.x > 0 && isFacingRight)
            {
                Flip();
            }

            // Dash-Mechanik prüfen
            if (Input.GetKeyDown(KeyCode.Space) && dash.GetCurrentDashCooldown() <= 0f && !isDashing && walkingInput != Vector2.zero)
            {
                StartCoroutine(PerformPlayerDash());
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
                lastMoveDirection = new Vector2(moveX, moveY).normalized;
            }

            return new Vector2(moveX, moveY).normalized;
        }

        private IEnumerator PerformPlayerDash()
        {
            isDashing = true;
            yield return StartCoroutine(dash.PerformDash(rb, walkingInput));
            isDashing = false;
        }
    }
}
