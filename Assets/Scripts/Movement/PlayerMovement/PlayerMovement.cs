using UnityEngine;
using System.Collections;

namespace Scripts.Movements
{
    public partial class PlayerMovement: Movement
    {
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

        protected override void FixedUpdate()
        {
            // Normale Bewegung, wenn nicht gedasht wird und Bewegungseingabe vorhanden ist
            if (!isDashing && walkingInput.magnitude > 0.01f) // Schwellenwert für Bewegung
            {
                rb.MovePosition(rb.position + walkingInput * moveSpeed * Time.fixedDeltaTime);
            }
        }

    }
}
