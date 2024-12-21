using UnityEngine;
using System.Collections;

namespace Scripts.Movements.Moves
{
    public class Dash: Move
    {
        [SerializeField] private float dashSpeed = 18f; // Geschwindigkeit während des Dashes
        [SerializeField] private float dashDuration = 0.2f; // Dauer des Dashes in Sekunden
        [SerializeField] private float dashCooldown = 1f; // Abklingzeit zwischen Dashes
        private float dashCooldownTimer = 0f; // Zeit bis zum nächsten Dash

        public void Update()
        {
            // Dash-Cooldown herunterzählen
            if (dashCooldownTimer > 0f)
            {
                dashCooldownTimer -= Time.deltaTime;
            }
        }

        public bool CanDash()
        {
            return dashCooldownTimer <= 0f;
        }

        public void ResetDashCooldown()
        {
            dashCooldownTimer = 0f;
        }

        public void SetDashCooldownTimer(float cooldown)
        {
            dashCooldownTimer = cooldown;
        }

        public void SetDashSpeed(float speed)
        {
            dashSpeed = speed;
        }

        public float GetDashSpeed()
        {
            return dashSpeed;
        }

        public void SetDashDuration(float duration)
        {
            dashDuration = duration;
        }

        public float GetDashDuration()
        {
            return dashDuration;
        }

        public float GetCurrentDashCooldown()
        {
            return dashCooldownTimer;
        }

        public void SetCurrentDashCooldown(float deltaTime)
        {
            dashCooldownTimer -= deltaTime;
        }

        public float GetDashCooldown()
        {
            return dashCooldown;
        }

        public void SetDashCooldown(float cooldown)
        {
            dashCooldown = cooldown;
        }

        public IEnumerator PerformDash(Rigidbody2D rb, Vector2 walkingInput)
        {
            Vector2 dashDirection = walkingInput.normalized; // Richtung des Dashes basierend auf der Eingabe

            float dashTime = 0f;
            while (dashTime < dashDuration)
            {
                rb.MovePosition(rb.position + dashDirection * dashSpeed * Time.fixedDeltaTime);
                dashTime += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate(); // Warte auf das nächste Physik-Update
            }

            // Starte den Dash-Cooldown
            dashCooldownTimer = dashCooldown;
        }
    }
}