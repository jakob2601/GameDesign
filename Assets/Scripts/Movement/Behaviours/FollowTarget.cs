using UnityEngine;
using Pathfinding;
using System.Collections;
using Scripts.Combats.CharacterCombats;
using Scripts.Movements.Moves;
using Scripts.Movements.AI;

namespace Scripts.Movements.Behaviours
{
    public class FollowTarget : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Transform GFX;

        [SerializeField] private Walking walking;
        [SerializeField] private Seeker seeker;
        [SerializeField] private Path path;
        [SerializeField] private Rigidbody2D rb;

        [SerializeField] private float nextWaypointDistance = 3f;
        [SerializeField] private float startUpdatePathTime = 0f;
        [SerializeField] private float updatePathRate = 0.5f;
        [SerializeField] private float timeSinceLastUpdate = 0f;
        private int currentWaypoint = 0;
        [SerializeField] private bool reachedEndOfPath = false;

        [SerializeField] private float startRadius = 10f;
        [SerializeField] private float endRadius = 3f;

        [SerializeField] protected float currentDistanceToTarget;

        [SerializeField] private bool isEnabled = true; // Is Enabled by interior
        [SerializeField] private bool isUnblocked; // Unblocked by outerior

        public void SetNextWaypointDistance(float nextWaypointDistance)
        {
            this.nextWaypointDistance = nextWaypointDistance;
        }

        public void SetCurrentWaypoint(int currentWaypoint)
        {
            this.currentWaypoint = currentWaypoint;
        }

        public void SetTimeSinceLastUpdate(float timeSinceLastUpdate)
        {
            this.timeSinceLastUpdate = timeSinceLastUpdate;
        }

        public void SetStartUpdatePathTime(float startUpdatePathTime)
        {
            this.startUpdatePathTime = startUpdatePathTime;
        }

        public void SetUpdatePathRate(float updatePathRate)
        {
            this.updatePathRate = updatePathRate;
        }

        public Path GetPath()
        {
            return path;
        }

        public void SetPath(Path path)
        {
            this.path = path;
        }

        public void SetEnabled(bool isEnabled)
        {
            this.isEnabled = isEnabled;
        }
        public bool GetEnabled()
        {
            return isEnabled;
        }

        public void SetUnblock(bool isUnblocked)
        {
            this.isUnblocked = isUnblocked;
        }
        public bool GetUnblocked()
        {
            return isUnblocked;
        }



        public Transform GetTarget()
        {
            if (target != null)
            {
                return target;
            }
            else
            {
                return null;
            }
        }

        public void SetTarget(Transform target)
        {
            this.target = target;
            if (target == null)
            {
                isEnabled = false;
            }
        }

        public void SetReachedEndOfPath(bool reachedEndOfPath)
        {
            this.reachedEndOfPath = reachedEndOfPath;
            if (reachedEndOfPath)
                isEnabled = false;
        }

        public bool GetReachedEndOfPath()
        {
            return reachedEndOfPath;
        }

        public void SetRigidbody(Rigidbody2D rb)
        {
            this.rb = rb;
        }

        public float GetStartUpdatePathTime()
        {
            return startUpdatePathTime;
        }
        public float GetUpdatePathRate()
        {
            return updatePathRate;
        }

        public float GetStartRadius()
        {
            return startRadius;
        }

        public float GetEndRadius()
        {
            return endRadius;
        }

        protected void SetCurrentDistanceToTarget(float currentDistanceToTarget)
        {
            this.currentDistanceToTarget = currentDistanceToTarget;
        }

        public float GetCurrentDistanceToTarget()
        {
            if (currentDistanceToTarget <= startRadius && currentDistanceToTarget > endRadius)
            {
                isEnabled = true;
                reachedEndOfPath = false;
            }
            else
                isEnabled = false;
            return currentDistanceToTarget;
        }

        private void UpdateCurrentDistanceToTarget()
        {
            currentDistanceToTarget = Vector2.Distance(transform.position, target.position);
        }

        public void Start()
        {
            seeker = GetComponent<Seeker>();
            if (seeker == null)
            {
                Debug.LogWarning("Seeker not found on " + gameObject.name + " in Script FollowTarget.cs");
            }
            foreach (Transform child in transform)
            {
                if (child.name.EndsWith("GFX"))
                {
                    GFX = child;
                    break;
                }
            }

            if (GFX == null)
            {
                Debug.LogError("No child with name ending in 'GFX' found.");
            }

            walking = GetComponent<Walking>();
            if (walking == null)
            {
                Debug.LogWarning("Walking not found on " + gameObject.name + " in Script FollowTarget.cs");
            }
        }

        public void Update()
        {
            if (timeSinceLastUpdate >= updatePathRate)
            {
                UpdatePath(rb);
                timeSinceLastUpdate = 0f;
            }
            else
            {
                timeSinceLastUpdate += Time.deltaTime;
            }

        }

        public void FixedUpdate()
        {
            if (target == null)
            {
                // Debug.LogWarning("Target not found on " + gameObject.name + " in Script FollowTarget.cs");
                return;
            }
            else if (walking == null)
            {
                Debug.LogWarning("Walking not found on " + gameObject.name + " in Script FollowTarget.cs");
                return;
            }
            else if (rb == null)
            {
                Debug.LogWarning("RigidBody2D not found on " + gameObject.name + " in Script FollowTarget.cs");
                return;
            }
            UpdateCurrentDistanceToTarget();

            if (this.currentDistanceToTarget <= this.endRadius)
            {
                this.reachedEndOfPath = true;
            }
            else if (this.isEnabled)
            {
                if (this.path == null)
                {
                    // Debug.LogWarning("Path not completed on " + gameObject.name + " in Script FollowTarget.cs");
                    return;
                }
                if (this.isUnblocked)
                {
                    this.MoveTowardsTarget(rb, GetComponent<MovementAI>());
                }
            }
        }

        protected void MoveTowardsTarget(Rigidbody2D rb, MovementAI movementAI)
        {
            if (!this.isEnabled || !this.isUnblocked)
                return;
            if (this.path == null)
            {
                Debug.LogWarning("Path is null");
                return;
            }
            else if (this.target == null)
            {
                Debug.LogWarning("Target is null");
                return;
            }

            if (this.currentWaypoint >= this.path.vectorPath.Count)
            {
                Debug.Log("End of path reached");
                this.reachedEndOfPath = true;
                return;
            }

            Vector2 direction = ((Vector2)this.path.vectorPath[currentWaypoint] - this.rb.position).normalized;

            if (direction.magnitude >= 0.1f)
            {
                this.rb.MovePosition(walking.getNewPosition(rb.position, direction));
                movementAI.SetLastMoveDirection(direction);
            }

            float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

            if (distance < this.nextWaypointDistance)
            {
                this.currentWaypoint++;
            }
            // movementAI.UpdateScale(directedForce);
        }



        private void UpdatePath(Rigidbody2D rb)
        {
            if (this.target != null && this.seeker.IsDone())
            {
                seeker.StartPath(rb.position, target.position, OnPathComplete);
            }
        }

        private void OnPathComplete(Path p)
        {
            if (!p.error)
            {
                SetPath(p);
                currentWaypoint = 0;
            }
            else
            {
                Debug.LogWarning(p.error);
            }
        }
    }
}