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
            

            // Dash-Mechanik prüfen
            if (Input.GetKeyDown(KeyCode.Space) && dash.GetCurrentDashCooldown() <= 0f && !isDashing && walkingInput != Vector2.zero)
            {
                StartCoroutine(this.PerformPlayerDash());
            }
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            // Normale Bewegung, wenn nicht gedasht wird und Bewegungseingabe vorhanden ist
            if (!isDashing && walkingInput.magnitude > 0.01f) // Schwellenwert für Bewegung
            {
                rb.MovePosition(walking.getNewPosition(rb.position, walkingInput));
            }
        }


        private IEnumerator PerformPlayerDash()
        {
            this.SetIsDashing(true);
            yield return StartCoroutine(dash.PerformDash(rb, walkingInput));
            this.SetIsDashing(false);
        }
    }
}
