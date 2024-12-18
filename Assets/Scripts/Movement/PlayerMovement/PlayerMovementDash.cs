using UnityEngine;
using System.Collections;

namespace Scripts.Movements
{
    public partial class PlayerMovement: Movement
    {
        public float dashSpeed = 10f; // Geschwindigkeit während des Dashes
        public float dashDuration = 0.2f; // Dauer des Dashes in Sekunden
        public float dashCooldown = 1f; // Abklingzeit zwischen Dashes
        
        private float dashCooldownTimer = 0f; // Zeit bis zum nächsten Dash

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