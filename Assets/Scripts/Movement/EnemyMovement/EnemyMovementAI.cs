using System.Collections;
using System.Collections.Generic;
using Scripts.Combats.CharacterCombats;
using UnityEngine;
using Scripts.Movements.Moves;
using Scripts.Movements.Behaviours;


namespace Scripts.Movements.AI
{
    public class EnemyMovementAI: MovementAI 
    {
        [SerializeField] private EnemyCombat enemyCombat;
        [SerializeField] private Unstuck unstuck;
        [SerializeField] private FollowTarget followTarget;
        [SerializeField] private Radiate radiate;
        [SerializeField] private Walking walking;

        private Transform player;
        protected LayerMask playerLayer;
        private Collider2D playerCollider;

        protected EnemyCombat GetEnemyCombat() {
            return enemyCombat;
        }

        protected void SetEnemyCombat(EnemyCombat enemyCombat) {
            this.enemyCombat = enemyCombat;
        }

        protected Unstuck GetUnstuck() {
            return unstuck;
        }

        protected void SetUnstuck(Unstuck unstuck) {
            this.unstuck = unstuck;
        }

        protected FollowTarget GetFollowTarget() {
            return followTarget;
        }

        protected void SetFollowTarget(FollowTarget followTarget) {
            this.followTarget = followTarget;
        }

        protected Radiate GetRadiate() {
            return radiate;
        }

        protected void SetRadiate(Radiate radiate) {
            this.radiate = radiate;
        }

        protected Walking GetWalking() {
            return walking;
        }

        protected void SetWalking(Walking walking) {
            this.walking = walking;
        }

        protected Transform GetPlayer() {
            return player;
        }

        protected void SetPlayer(Transform player) {
            this.player = player;
        }

        protected LayerMask GetPlayerLayer() {
            return playerLayer;
        }

        protected void SetPlayerLayer(LayerMask playerLayer) {
            this.playerLayer = playerLayer;
        }

        protected Collider2D GetPlayerCollider() {
            return playerCollider;
        }

        protected void SetPlayerCollider(Collider2D playerCollider) {
            this.playerCollider = playerCollider;
        }


        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();

            Transform animatorTransform = transform.Find("Animator");
            if (animatorTransform != null)
            {
                this.SetAnimator(animatorTransform.GetComponent<Animator>());
            }
            if (animator == null)
            {
                Debug.LogError("Animator component not found on " + gameObject.name);
            }

            this.SetEnemyCombat(GetComponent<EnemyCombat>());
            if (enemyCombat == null)
            {
                Debug.LogError("EnemyCombat component not found on " + gameObject.name);
            }

            this.SetWalking(GetComponent<Walking>());
            if (walking == null)
            {
                Debug.LogError("Walking component not found on " + gameObject.name);
            }

            this.SetUnstuck(GetComponent<Unstuck>());
            if (unstuck == null)
            {
                Debug.LogError("Unstuck component not found on " + gameObject.name);
            }

            this.SetFollowTarget(GetComponent<FollowTarget>());
            if (followTarget == null)
            {
                Debug.LogError("FollowTarget component not found on " + gameObject.name);
                followTarget.SetUnblock(false);
            }
            else 
            {
                followTarget.SetRigidbody(rb);
                followTarget.SetUnblock(true);
            }

            this.SetRadiate(GetComponent<Radiate>());
            if (radiate == null)
            {
                Debug.LogError("Radiate component not found on " + gameObject.name);
            }
        }

        protected override void FixedUpdate()
        {
            // Finde den Spieler basierend auf der Layer
            this.SetPlayerCollider(Physics2D.OverlapCircle(transform.position, followTarget.GetStartRadius(), playerLayer));
            if (playerCollider != null)
            {
                this.SetPlayer(playerCollider.transform);
                followTarget.setTarget(player);
                enemyCombat.SetPlayer(player);

                if(followTarget.GetCurrentDistanceToTarget() <= radiate.GetCircleRadius()) {
                    followTarget.SetUnblock(false); 
                    radiate.RadiateAroundTarget(player, walking.GetMoveSpeed() * 2/3, rb, ref lastMoveDirection);
                    followTarget.SetUnblock(true);
                }
                else if (followTarget.GetEnabled() && followTarget.GetUnblocked())
                {
                    // Wenn der Spieler in Reichweite ist, bewegt sich der Gegner auf ihn zu
                    followTarget.SetUnblock(true); 
                    Debug.Log("Player in range.");
                }
                else if(followTarget.GetCurrentDistanceToTarget() > followTarget.GetStartRadius())
                {
                    // Wenn der Spieler nicht in Reichweite ist, kann der Gegner andere Aktionen ausführen
                    followTarget.SetUnblock(false); 
                }
                if(unstuck.getIsUnstucking())
                {
                    followTarget.SetUnblock(false); 
                }
            }
            else
            {
                Debug.Log("Player not found in the area.");
                followTarget.SetEnabled(false); 
            }
        }

        protected override void Update()
        {
            this.AnimateWalking(lastMoveDirection);
        }

        void OnDrawGizmosSelected()
        {
            if (followTarget != null)
            {
                // Zeichne den Erkennungsbereich im Editor
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, followTarget.GetStartRadius());
            }
            else
            {
                Debug.LogWarning("FollowTarget is not assigned.");
            }
        }
    }
}
