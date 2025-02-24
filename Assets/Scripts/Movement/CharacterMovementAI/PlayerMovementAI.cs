using UnityEngine;
using System.Collections;
using Scripts.Movements.Moves;
using MyGame;

namespace Scripts.Movements.AI
{
    public class PlayerMovementAI: MovementAI
    {
        private Dash dash;
        

        protected Dash GetDash() {
            return dash;
        }

        protected void SetDash(Dash dash) {
            this.dash = dash;
        }   

        

        protected override void Start()
        {
            base.Start();

            this.SetDash(GetComponent<Dash>());
            if (dash == null)
            {
                Debug.LogError("Dash component not found on " + gameObject.name);
            }
        }

        protected override void Update()
        {
            base.Update();
            /*
            if (walkingInput.x < 0 && !this.isFacingRight || walkingInput.x > 0 && this.isFacingRight)
            {
                this.Flip();
            }
            */

            // Dash-Mechanik prüfen
            if (Input.GetKeyDown(KeyCode.Space) && dash.GetCurrentDashCooldown() <= 0f && !isDashing && walkingInput != Vector2.zero)
            {
                StartCoroutine(this.PerformPlayerDash());
            }
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            // Normale Bewegung, wenn nicht gedasht wird 
            if (!isDashing)
            {
                walking.Walk(this.walkingInput);
            }
        }

        protected override void ProcessInputs()
        {
            float moveX = Input.GetAxisRaw("Horizontal");
            float moveY = Input.GetAxisRaw("Vertical");

            if (Mathf.Abs(moveX) > 0 || Mathf.Abs(moveY) > 0)
            {
                this.SetLastMoveDirection(new Vector2(moveX, moveY).normalized);
                animator.SetBool("IsMoving", true);
                isWalking = true;
            }
            else 
            {
                animator.SetBool("IsMoving", false);
                isWalking = false;
            }   
            this.SetWalkingInput(new Vector2(moveX, moveY).normalized);
        }

        private IEnumerator PerformPlayerDash()
        {
            this.SetIsDashing(true);
            yield return StartCoroutine(dash.PerformDash(rb, walkingInput));
            this.SetIsDashing(false);
        }
    }
}
