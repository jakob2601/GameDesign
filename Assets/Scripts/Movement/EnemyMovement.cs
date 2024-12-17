using UnityEngine;
using Pathfinding;

namespace Scripts.Movements
{
    public class EnemyMovement : Movement 
    {
        public Transform target;
        public Transform enemyGFX;
        public float nextWaypointDistance = 3f;
        public float startUpdatePathTime = 0f;
        public float updatePathRate = 0.5f;
        public float circleRadius = 3f; // Radius, innerhalb dessen das Verhalten geändert wird
        public float combatSpeed = 2f; // Geschwindigkeit des Kreisens um den Spieler

        Path path;
        int currentWaypoint = 0;
        bool reachedEndOfPath = false;

        Seeker seeker;
        private Vector3 originalScale;
        private bool inCombat = false;
        private float angle = 0f;

        protected override void Start()
        {
            base.Start();
            seeker = GetComponent<Seeker>();

            originalScale = enemyGFX.localScale;
            InvokeRepeating("UpdatePath", startUpdatePathTime, updatePathRate);
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
                inCombat = true;
                CombatMovement();
            }
            else
            {
                // Normale Bewegung zum Ziel
                inCombat = false;
                MoveTowardsTarget();
            }
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
            else
            {
                reachedEndOfPath = false;
            }

            Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
            Vector2 force = moveSpeed * direction * Time.deltaTime;

            rb.AddForce(force);

            float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

            if (distance < nextWaypointDistance)
            {
                currentWaypoint++;
            }
            UpdateScale(force);

        }

        // Go in circles around the target
        void CombatMovement()
        {
            angle += combatSpeed * Time.deltaTime;
            float x = Mathf.Cos(angle) * circleRadius;
            float y = Mathf.Sin(angle) * circleRadius;
            Vector2 offset = new Vector2(x, y);
            Vector2 targetPosition = (Vector2)target.position + offset;

            Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
            Vector2 force = direction * moveSpeed * Time.deltaTime;

            rb.AddForce(force);
            UpdateScale(force);
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