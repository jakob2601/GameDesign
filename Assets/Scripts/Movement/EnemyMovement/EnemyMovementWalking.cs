using UnityEngine;
using Pathfinding;
using System.Collections;
using Scripts.Combats.CharacterCombats;

namespace Scripts.Movements
{
    public partial class EnemyMovement : Movement 
    {
        public Transform target;
        public Transform enemyGFX;
        protected EnemyCombat enemyCombat; 
        public float nextWaypointDistance = 3f;
        public float startUpdatePathTime = 0f;
        public float updatePathRate = 0.5f;
        

        Path path;
        int currentWaypoint = 0;
        bool reachedEndOfPath = false;
        Seeker seeker;

        private Vector3 originalScale;

        private bool isUnstucking = false;
         private float stuckTimer = 0f;

        private float angle = 0f;
        public float circleRadius = 3f; // Radius, innerhalb dessen das Verhalten geändert wird
        public float combatSpeed = 2f; // Geschwindigkeit des Kreisens um den Spieler

        public float stuckCheckInterval = 1f; // Intervall, um zu überprüfen, ob der Gegner feststeckt
        public float stuckDistanceThreshold = 0.1f; // Schwellenwert, um festzustellen, ob der Gegner feststeckt
        public float unstuckDuration = 1f; // Dauer der Ausweichbewegung

        protected override void Start()
        {
            base.Start();
            seeker = GetComponent<Seeker>();
            enemyCombat = GetComponent<EnemyCombat>();

            originalScale = enemyGFX.localScale;
            InvokeRepeating("UpdatePath", startUpdatePathTime, updatePathRate);
            InvokeRepeating("CheckIfStuck", stuckCheckInterval, stuckCheckInterval);
        }


        // Update is called a persistent 50 times per second
        protected override void FixedUpdate()
        {
            if (path == null || target == null)
                return;

            float distanceToTarget = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

            if (distanceToTarget <= circleRadius || reachedEndOfPath)
            {
                // Beginne das Kreisen um den Spieler
                enemyCombat.inCombat = true;
                CombatMovement();
            }
            else
            {
                // Normale Bewegung zum Ziel
                enemyCombat.inCombat = false;
                MoveTowardsTarget();
            }
        }

        protected override void Update()
        {
            
        }

        protected void MoveTowardsTarget() 
        {
            if (path == null || target == null)
                return;

            if (currentWaypoint >= path.vectorPath.Count)
            {
                reachedEndOfPath = true;
                return;
            }
            else if(isUnstucking)
            {
                return;
            }
            else
            {
                reachedEndOfPath = false;
            }

            Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
            Vector2 force = moveSpeed * direction * Time.deltaTime;
            lastMoveDirection = direction;

            rb.AddForce(force);
            AnimateWalking(force);

            float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

            if (distance < nextWaypointDistance)
            {
                currentWaypoint++;
            }
            UpdateScale(force);
            

        }

        // Go in circles around the target
        protected void CombatMovement()
        {
            if(!isUnstucking)
            {
                angle += combatSpeed * Time.deltaTime;
                float x = Mathf.Cos(angle) * circleRadius;
                float y = Mathf.Sin(angle) * circleRadius;
                Vector2 offset = new Vector2(x, y);
                Vector2 targetPosition = (Vector2)target.position + offset;

                Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
                Vector2 force = direction * moveSpeed * Time.deltaTime;
                lastMoveDirection = direction;

                rb.AddForce(force);
                UpdateScale(force);
            }
        }

        protected void CheckIfStuck()
        {
            Collider2D[] obstacles = Physics2D.OverlapCircleAll(transform.position, stuckDistanceThreshold, LayerMask.GetMask("Obstacle"));
            if (obstacles.Length > 0)
            {
                stuckTimer += stuckCheckInterval;
                if (stuckTimer >= unstuckDuration)
                {
                    StartCoroutine(Unstuck(obstacles[0].transform.position));
                }
            }
            else
            {
                stuckTimer = 0f;
            }
        }

        private IEnumerator Unstuck(Vector2 obstaclePosition)
        {
            isUnstucking = true;
            Vector2 directionAwayFromObstacle = ((Vector2)transform.position - obstaclePosition).normalized;
            float timer = 0f;

            while (timer < unstuckDuration)
            {
                rb.AddForce(directionAwayFromObstacle * moveSpeed * Time.deltaTime);
                lastMoveDirection = directionAwayFromObstacle;
                timer += Time.deltaTime;
                yield return null;
            }

            isUnstucking = false;
            UpdatePath();
        }

        void UpdateScale(Vector2 force)
        {
            if (force.x >= 0.01f)
            {
                enemyGFX.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
            }
            else if (force.x <= -0.01f)
            {
                enemyGFX.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
            }
        }

        void UpdatePath()
        {
            if (target != null && seeker.IsDone())
            {
                seeker.StartPath(rb.position, target.position, OnPathComplete);
            }
        }

        void OnPathComplete(Path p)
        {
            if (!p.error)
            {
                path = p;
                currentWaypoint = 0;
            }
        }
    }
}