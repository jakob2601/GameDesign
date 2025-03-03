using System.Collections;
using System.Collections.Generic;
using Scripts.Combats.CharacterCombats;
using UnityEngine;
using Scripts.Movements.Moves;
using Scripts.Movements.Behaviours;


namespace Scripts.Movements.AI
{
    public class EnemyMovementAI : MovementAI
    {
        [SerializeField] private EnemyCombat enemyCombat;
        [SerializeField] private Unstuck unstuck;
        [SerializeField] private FollowTarget followTarget;
        [SerializeField] private Knockback knockback;

        [SerializeField] private Transform player;
        [SerializeField] protected LayerMask playerLayer;
        [SerializeField] private Collider2D playerCollider;

        [Header("Bow Positioning")]
        [SerializeField] private float repositionCooldown = 1.5f;
        [SerializeField] private float repositionCheckRadius = 2f;
        private float lastRepositionTime = 0f;

        protected EnemyCombat GetEnemyCombat()
        {
            return enemyCombat;
        }

        protected void SetEnemyCombat(EnemyCombat enemyCombat)
        {
            this.enemyCombat = enemyCombat;
        }

        protected Unstuck GetUnstuck()
        {
            return unstuck;
        }

        protected void SetUnstuck(Unstuck unstuck)
        {
            this.unstuck = unstuck;
        }

        protected FollowTarget GetFollowTarget()
        {
            return followTarget;
        }

        protected void SetFollowTarget(FollowTarget followTarget)
        {
            this.followTarget = followTarget;
        }


        protected Knockback GetKnockback()
        {
            return knockback;
        }

        protected void SetKnockback(Knockback knockback)
        {
            this.knockback = knockback;
        }

        protected Transform GetPlayer()
        {
            return player;
        }

        protected void SetPlayer(Transform player)
        {
            this.player = player;
        }

        protected LayerMask GetPlayerLayer()
        {
            return playerLayer;
        }

        protected void SetPlayerLayer(LayerMask playerLayer)
        {
            this.playerLayer = playerLayer;
        }

        protected Collider2D GetPlayerCollider()
        {
            return playerCollider;
        }

        protected void SetPlayerCollider(Collider2D playerCollider)
        {
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

            this.SetEnemyCombat(transform.root.GetComponentInChildren<EnemyCombat>());
            if (enemyCombat == null)
            {
                Debug.LogError("EnemyCombat component not found on " + gameObject.name);
            }

            this.SetWalking(transform.root.GetComponentInChildren<Walking>());
            if (walking == null)
            {
                Debug.LogError("Walking component not found on " + gameObject.name);
            }

            this.SetUnstuck(transform.root.GetComponentInChildren<Unstuck>());
            if (unstuck == null)
            {
                Debug.LogError("Unstuck component not found on " + gameObject.name);
            }

            this.SetKnockback(transform.root.GetComponentInChildren<Knockback>());
            if (knockback == null)
            {
                Debug.LogError("Knockback component not found on " + gameObject.name);
            }

            this.SetFollowTarget(transform.root.GetComponentInChildren<FollowTarget>());
            if (followTarget == null)
            {
                Debug.LogError("FollowTarget component not found on " + gameObject.name);
            }
            else
            {
                followTarget.SetRigidbody(rb);
            }

            // Find player if not set
            if (player == null)
            {
                GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
                if (playerObj != null)
                {
                    player = playerObj.transform;
                    if (enemyCombat != null)
                    {
                        enemyCombat.SetPlayer(player);
                    }
                }
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

                if (knockback.GetKnockbackActive())
                {
                    // Debug.Log("Knockback active.");

                    return;
                }
                else if (unstuck.getIsUnstucking())
                {
                    // Debug.Log("Is Unstucking");
                    return;
                }
                else if (followTarget.GetEnabled())
                {
                    // Wenn der Spieler in Reichweite ist, bewegt sich der Gegner auf ihn zu
                    // Debug.Log("Is Following Target");
                    walking.Walk(this.GetWalkingInput());
                }
                else if (followTarget.GetCurrentDistanceToTarget() > followTarget.GetStartRadius())
                {
                    // Wenn der Spieler nicht in Reichweite ist, kann der Gegner andere Aktionen ausführen
                    // Debug.Log("Is doing other things");
                }
            }
            else
            {
                // Debug.Log("Player not found in the area.");
            }
        }

        protected override void Update()
        {
            base.Update();

            // Check for better shooting position if we have bow
            if (enemyCombat != null && enemyCombat.HasWeapon(Combat.WeaponTypes.Bow) && 
                Time.time > lastRepositionTime + repositionCooldown)
            {
                TryFindBetterShootingPosition();
            }
        }

        protected override void ProcessInputs()
        {
            this.SetWalkingInput(followTarget.GetWalkingInput());
            float moveX = walkingInput.x;
            float moveY = walkingInput.y;

            if (moveX != 0 || moveY != 0)
            {
                this.SetLastMoveDirection(new Vector2(moveX, moveY).normalized);
                isWalking = true;
            }
            else
            {
                isWalking = false;
                
            }
            characterAnimation.SetIsMoving(isWalking);
        }

        private void TryFindBetterShootingPosition()
        {
            if (player == null || followTarget == null || enemyCombat == null) return;
            
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            
            // Check if we're in a poor shooting position
            bool tooClose = distanceToPlayer < enemyCombat.minShootRange;
            bool tooFar = distanceToPlayer > enemyCombat.maxShootRange;
            bool noLineOfSight = !enemyCombat.HasLineOfSightToPlayer();
            
            if (tooClose || tooFar || noLineOfSight)
            {
                Vector2 betterPosition = CalculateBetterShootingPosition();
                
                // Only reposition if the new position is significantly different
                if (Vector2.Distance(betterPosition, transform.position) > repositionCheckRadius)
                {
                    followTarget.SetCustomTargetPoint(betterPosition);
                    lastRepositionTime = Time.time;
                    
                    // Verbesserte Log-Meldung
                    // string reason = tooClose ? "too close" : (tooFar ? "too far" : "no line of sight");
                    // Debug.Log($"Repositioning for better bow shot: {reason}. Distance to player={distanceToPlayer:F1}, ideal={enemyCombat.idealShootRange:F1}");
                }
            }
        }

        private Vector2 CalculateBetterShootingPosition()
        {
            Vector2 dirToPlayer = (player.position - transform.position).normalized;
            float currentDistance = Vector2.Distance(transform.position, player.position);
            
            // If too close, back away
            if (currentDistance < enemyCombat.minShootRange)
            {
                return (Vector2)transform.position - dirToPlayer * (enemyCombat.minShootRange - currentDistance + 1f);
            }
            
            // If too far, move closer
            if (currentDistance > enemyCombat.maxShootRange)
            {
                return (Vector2)transform.position + dirToPlayer * (currentDistance - enemyCombat.maxShootRange + 1f);
            }
            
            // If no line of sight, try moving perpendicular
            if (!enemyCombat.HasLineOfSightToPlayer())
            {
                // Try to find a position with line of sight within ideal range
                Vector2 perpendicular = new Vector2(-dirToPlayer.y, dirToPlayer.x);
                
                // Try left side first
                Vector2 leftPosition = (Vector2)transform.position + perpendicular * 2f;
                
                // Debug visualize
                Debug.DrawLine(transform.position, leftPosition, Color.cyan, 0.5f);
                
                return leftPosition;
            }
            
            // Default is to try to get to ideal range
            float idealDistance = enemyCombat.idealShootRange;
            return (Vector2)transform.position + dirToPlayer * (idealDistance - currentDistance);
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
