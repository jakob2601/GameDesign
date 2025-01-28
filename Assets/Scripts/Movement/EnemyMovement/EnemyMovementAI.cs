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
        [SerializeField] private Knockback knockback;

        [SerializeField] private Transform player;
        [SerializeField] protected LayerMask playerLayer;
        [SerializeField] private Collider2D playerCollider;
        
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


        protected Knockback GetKnockback() {
            return knockback;
        }

        protected void SetKnockback(Knockback knockback) {
            this.knockback = knockback;
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

            this.SetKnockback(GetComponent<Knockback>());
            if (knockback == null)
            {
                Debug.LogError("Knockback component not found on " + gameObject.name);
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
            base.FixedUpdate();
            // Finde den Spieler basierend auf der Layer
            this.SetPlayerCollider(Physics2D.OverlapCircle(transform.position, followTarget.GetStartRadius(), this.GetPlayerLayer()));
            if (playerCollider != null)
            {
                this.SetPlayer(playerCollider.transform);
                followTarget.SetTarget(player);
                enemyCombat.SetPlayer(player);
                radiate.SetTarget(player);

                if(knockback.GetKnockbackActive()) {
                    // Debug.Log("Knockback active.");
                    followTarget.SetUnblock(false); 
                    radiate.SetIsUnblocked(false);
                    return;
                }
                if(unstuck.getIsUnstucking()) {
                    // Debug.Log("Is Unstucking");
                    followTarget.SetUnblock(false); 
                    radiate.SetIsUnblocked(false);
                }
                else if(followTarget.GetCurrentDistanceToTarget() <= radiate.GetCircleRadius()) {
                    // Debug.Log("Is Radiating");
                    followTarget.SetUnblock(false); 
                    radiate.SetIsUnblocked(true);
                    radiate.RadiateAroundTarget();
                    followTarget.SetUnblock(true);
                }
                else if (followTarget.GetEnabled() && followTarget.GetUnblocked())
                {
                    // Wenn der Spieler in Reichweite ist, bewegt sich der Gegner auf ihn zu
                    // Debug.Log("Is Following Target");
                    followTarget.SetUnblock(true); 
                    radiate.SetIsUnblocked(false);
                }
                else if(followTarget.GetCurrentDistanceToTarget() > followTarget.GetStartRadius())
                {
                    // Wenn der Spieler nicht in Reichweite ist, kann der Gegner andere Aktionen ausführen
                    // Debug.Log("Is doing other things");
                    followTarget.SetUnblock(false); 
                    radiate.SetIsUnblocked(false);
                }
                
            }
            else
            {
                Debug.Log("Player not found in the area.");
                followTarget.SetUnblock(false); 
            }
        }

        protected override void Update()
        {   
            base.Update();

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
