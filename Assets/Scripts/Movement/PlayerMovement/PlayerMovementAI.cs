using UnityEngine;
using System.Collections;
using Scripts.Movements.Moves;
using MyGame;

namespace Scripts.Movements.AI
{
    public class PlayerMovementAI: MovementAI
    {
        private Dash dash;
        [SerializeField] public SpriteRenderer swordRenderer; // #TODO: exchange this with Sword class 
        [SerializeField] public Transform swordTransform; // #TODO: exchange this with Sword class
        

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

            swordTransform = transform.Find("Sword");
            if (swordTransform != null)
            {
                swordRenderer = swordTransform.Find("SwordGFX").GetComponent<SpriteRenderer>();
            }
            if (swordRenderer == null)
            {
                Debug.LogError("Sword SpriteRenderer component not found on " + gameObject.name);
            }
        }

        protected override void Update()
        {
            base.Update();
            
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
            base.FixedUpdate();
            // Normale Bewegung, wenn nicht gedasht wird und Bewegungseingabe vorhanden ist
            if (!isDashing && walkingInput.magnitude > 0.01f) // Schwellenwert für Bewegung
            {
                rb.MovePosition(walking.getNewPosition(rb.position, walkingInput));
            }
        }

        public override void AnimateWalking()
        {
            base.AnimateWalking();
            UpdateSwordSortingOrder();
        }

        private void UpdateSwordSortingOrder()
        {
            if (lastMoveDirection.y < 0)
            {
                // Spieler schaut nach unten, Schwert vor dem Spieler anzeigen
                swordRenderer.sortingOrder = 1;
                swordTransform.localPosition = new Vector3(-0.13f,-0.4f,-0.18f);
            }
            else if (lastMoveDirection.y > 0)
            {
                // Spieler schaut nach oben, Schwert hinter dem Spieler anzeigen
                swordRenderer.sortingOrder = -1;
                swordTransform.localPosition = new Vector3(0.13f,-0.4f,-0.18f);
                
            }
            else if(lastMoveDirection.x > 0 && lastMoveDirection.y == 0)
            {
                // Spieler schaut nach rechts, Schwert vor dem Spieler anzeigen
                swordRenderer.sortingOrder = 1;
                swordTransform.localPosition = new Vector3(-0.13f,-0.4f,-0.18f);
            }
            else if(lastMoveDirection.x < 0 && lastMoveDirection.y == 0)
            {
                //  Spieler schaut nach rechts, Schwert vor dem Spieler anzeigen
                swordRenderer.sortingOrder = 1;
                swordTransform.localPosition = new Vector3(0.13f,-0.4f,-0.18f);
            }
            else {
                swordRenderer.sortingOrder = 1;
                swordTransform.localPosition = new Vector3(-0.13f,-0.4f,-0.18f);
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
