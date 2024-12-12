using UnityEngine;
using Pathfinding;

namespace Scripts.Movements
{
    public class EnemyMovement : Movement 
    {
        public Transform target;
        public Transform enemyGFX;
        public float nextWaypointDistance = 3f;

        Path path;
        int currentWaypoint = 0;
        bool reachedEndOfPath = false;

        Seeker seeker;
        private Vector3 originalScale;

        protected override void Start()
        {
            base.Start();
            seeker = GetComponent<Seeker>();

            originalScale = enemyGFX.localScale;
            InvokeRepeating("UpdatePath", 0f, 0.5f);
        }


        // Update is called a persistent 50 times per second
        protected override void FixedUpdate()
        {
            if (path == null)
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
            if (seeker.IsDone())
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