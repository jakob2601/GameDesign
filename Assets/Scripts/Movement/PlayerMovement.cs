using UnityEngine;
using System.Collections;

namespace Scripts.Movements
{
    public partial class PlayerMovement: Movement
    {
        
        public float dashSpeed = 10f; // Geschwindigkeit während des Dashes
        public float dashDuration = 0.2f; // Dauer des Dashes in Sekunden
        public float dashCooldown = 1f; // Abklingzeit zwischen Dashes
        
        
        private Vector2 walkingInput; // Bewegungseingabe
        private float dashCooldownTimer = 0f; // Zeit bis zum nächsten Dash
        


        protected override void Start()
        {
            base.Start();
        }

        protected override void Update()
        {
            ProccessInputs();
            AnimateWalking(walkingInput);

            if (walkingInput.x < 0 && !isFacingRight || walkingInput.x > 0 && isFacingRight)
            {
                Flip();
            }

            // Dash-Mechanik prüfen
            if (Input.GetKeyDown(KeyCode.Space) && dashCooldownTimer <= 0f && !isDashing && walkingInput != Vector2.zero)
            {
                StartCoroutine(Dash());
            }

            // Dash-Cooldown herunterzählen
            if (dashCooldownTimer > 0f)
            {
                dashCooldownTimer -= Time.deltaTime;
            }
        }

        protected void ProccessInputs()
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

            walkingInput = new Vector2(moveX, moveY).normalized;
        }



        protected override void FixedUpdate()
        {
            // Normale Bewegung, wenn nicht gedasht wird und Bewegungseingabe vorhanden ist
            if (!isDashing && walkingInput.magnitude > 0.01f) // Schwellenwert für Bewegung
            {
                rb.MovePosition(rb.position + walkingInput * moveSpeed * Time.fixedDeltaTime);
            }
        }

        private IEnumerator Dash()
        {
            isDashing = true;
            Vector2 dashDirection = walkingInput.normalized; // Richtung des Dashes basierend auf der Eingabe

            float dashTime = 0f;
            while (dashTime < dashDuration)
            {
                rb.MovePosition(rb.position + dashDirection * dashSpeed * Time.fixedDeltaTime);
                dashTime += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate(); // Warte auf das nächste Physik-Update
            }

            isDashing = false;

            // Starte den Dash-Cooldown
            dashCooldownTimer = dashCooldown;
        }
    }


}
