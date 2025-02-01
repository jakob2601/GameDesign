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

        [SerializeField] private float nextWaypointDistance = 0.5f;
        [SerializeField] private float startUpdatePathTime = 0f;
        [SerializeField] private float updatePathRate = 0.5f;
        [SerializeField] private float timeSinceLastUpdate = 0f;
        [SerializeField] private int currentWaypoint = 0;
        [SerializeField] private bool reachedEndOfPath = false;
        [SerializeField] private bool targetLost = false;
        
        [SerializeField] private Vector2 walkingInput;

        [SerializeField] private float startRadius;
        [SerializeField] private float endRadius;


        [SerializeField] protected float currentDistanceToTarget;

        [SerializeField] private bool isEnabled = true; // Is Enabled by interior
        

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

        public void SetEnabled()
        {
            if (this.target != null && !this.reachedEndOfPath && !this.targetLost
             && this.currentDistanceToTarget > this.endRadius && this.currentDistanceToTarget <= startRadius) 
            {
                isEnabled = true;
            }
            else 
            {
                isEnabled = false;
            }
        }

        public bool GetEnabled()
        {
            return isEnabled;
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
        }

        public void SetReachedEndOfPath()
        {
            this.reachedEndOfPath = true;
        }

        public void ResetReachedEndOfPath()
        {
            this.reachedEndOfPath = false;
        }

        public bool GetReachedEndOfPath()
        {
            return reachedEndOfPath;
        }

        public void SetTargetLost(bool targetLost)
        {
            this.targetLost = targetLost;
        }

        public bool GetTargetLost()
        {
            return targetLost;
        }

        protected void SetWalkingInput(Vector2 walkingInput)
        {
            this.walkingInput = walkingInput;
        }

        public Vector2 GetWalkingInput()
        {
            return walkingInput;
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
                SetReachedEndOfPath();
            }
            else 
            {
                ResetReachedEndOfPath();
            }

            if (path == null)
            {
                return;
            }
            if(this.currentDistanceToTarget <= this.startRadius) {
                UpdateLineOfSight();
            }
               
            SetEnabled();

            MoveTowardsTarget(rb, GetComponent<MovementAI>());
        }

        protected void UpdateLineOfSight()
        {
            if (path == null)
            {
                return;
            }
            bool targetFound = HasCombinedLOSPoint();

            if (targetFound)
            {  
                SetTargetLost(false);
            }
            else
            {
                Debug.DrawRay(transform.position, target.position - transform.position, Color.red);
                SetTargetLost(true);
            }

        }

        protected void MoveTowardsTarget(Rigidbody2D rb, MovementAI movementAI)
        {
            if (!this.isEnabled)
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
                SetReachedEndOfPath();
                return;
            }
            else 
            {
                ResetReachedEndOfPath();
            }

            if (targetLost)
            {

            }
            else
            {
                Vector2 direction = ((Vector2)this.path.vectorPath[currentWaypoint] - this.rb.position).normalized;

                this.SetWalkingInput(direction);

                float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

                if (distance < this.nextWaypointDistance)
                {
                    this.currentWaypoint++;
                }
            }
        }

        // Check if the character and the target have line of sight to the same point on the path
        private bool HasCombinedLOSPoint()
        {
            if (path == null || path.vectorPath == null)
            {
                Debug.LogError("Path or path.vectorPath is null");
                return false;
            }

            bool pointFound = false;
            for (int i = 0; i < path.vectorPath.Count; i++)
            {

                // Check if the character has line of sight to the waypoint
                bool characterHasLineOfSight = HasLineOfSight(this.path.vectorPath[i],  transform);

                // Check if the target has line of sight to the waypoint
                bool targetHasLineOfSight = HasLineOfSight(this.path.vectorPath[i], target);

                // If both the character and the target have line of sight to the same point on the path and the distance between the character and the path is less than the start radius
                if (characterHasLineOfSight && targetHasLineOfSight && Vector2.Distance(transform.position, this.path.vectorPath[i]) < startRadius)
                {
                    Debug.DrawRay(transform.position, this.path.vectorPath[i] - transform.position, Color.green);
                    Debug.DrawRay(target.position, this.path.vectorPath[i] - target.position, Color.green);

                    pointFound = true;
                    break;
                }
            }

            return pointFound;
        }

        // Check if there is line of sight between one point and a target
        private bool HasLineOfSight(Vector3 start, Transform endTarget) 
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll(start, endTarget.position - start);
            bool targetFound = false;
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider != null && hit.collider.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
                {
                    break;
                }
                if (hit.collider != null && hit.collider.transform == endTarget)
                {
                    targetFound = true;
                    break;
                }
            }
            return targetFound;
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