using UnityEngine;
using Pathfinding;
using System.Collections;
using Scripts.Combats.CharacterCombats;
using Scripts.Movements.Moves;
using Scripts.Movements.AI;

namespace Scripts.Movements.Behaviours
{
    public class FollowTarget: MonoBehaviour 
    {
        [SerializeField] private Transform target;
        [SerializeField] private Transform GFX;

        [SerializeField] private Walking walking;
        [SerializeField] private Seeker seeker;
        private Path path;
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


        public void setTarget(Transform target) 
        {
            this.target = target;
            if(target == null)
            {
                isEnabled = false;
            }
        }
        public Transform getTarget() 
        {
            if(target != null) 
            {
                return target;
            }
            else 
            {
                return null;
            }
        }

        public void SetReachedEndOfPath(bool reachedEndOfPath) 
        {
            this.reachedEndOfPath = reachedEndOfPath;
            if(reachedEndOfPath)
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

        public float GetCurrentDistanceToTarget() 
        {
            if(currentDistanceToTarget <= startRadius && currentDistanceToTarget > endRadius) {
                isEnabled = true;
                reachedEndOfPath = false;
            }
            else 
                isEnabled = false;
            return currentDistanceToTarget;
        }

        private void UpdateCurrentDistanceToTarget() {
            currentDistanceToTarget = Vector2.Distance(transform.position, target.position);
        }
        
        public void Start() 
        {
            seeker = GetComponent<Seeker>();
            if(seeker == null)
            {
                Debug.LogWarning("Seeker not found on " + gameObject.name + " in Script FollowTarget.cs");
            }
            GFX = transform.Find("EnemyGFX");
            if(GFX == null) 
            {
                Debug.LogWarning("GFX not found on " + gameObject.name + " in Script FollowTarget.cs");
            }

            walking = GetComponent<Walking>();
            if(walking == null)
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
            if(target == null) {
                Debug.LogWarning("Target not found on " + gameObject.name + " in Script FollowTarget.cs");
                return;
            }
            else if(walking == null) {
                Debug.LogWarning("Walking not found on " + gameObject.name + " in Script FollowTarget.cs");
                return;
            }
            else if(rb == null) {
                Debug.LogWarning("RigidBody2D not found on " + gameObject.name + " in Script FollowTarget.cs");
                return;
            }
            UpdateCurrentDistanceToTarget();
            
            if(currentDistanceToTarget <= endRadius)
            {
                reachedEndOfPath = true;
            }
            else if(isEnabled)
            {
                if (path == null) {
                    Debug.LogWarning("Path not found on " + gameObject.name + " in Script FollowTarget.cs");
                    return;
                }
                if(isUnblocked)
                    MoveTowardsTarget(walking.GetMoveSpeed(), rb, GetComponent<MovementAI>());
            }
        }

        protected void MoveTowardsTarget(float moveSpeed, Rigidbody2D rb, MovementAI movementAI) 
        {
            if(!isEnabled || !isUnblocked)
                return;
            if (path == null){
                Debug.LogWarning("Path is null");
                return;
            }
            else if(target == null){
                Debug.LogWarning("Target is null");
                return;
            }

            if (currentWaypoint >= path.vectorPath.Count)
            {
                Debug.Log("End of path reached");
                reachedEndOfPath = true;
                return;
            }

            Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
            
            if(direction.magnitude >= 0.1f)
            {
                rb.MovePosition(walking.getNewPosition(rb.position, direction));
                movementAI.lastMoveDirection = direction;
            }
        
            float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

            if (distance < nextWaypointDistance)
            {
                currentWaypoint++;
            }
            // movementAI.UpdateScale(directedForce);
        }

        

        private void UpdatePath(Rigidbody2D rb)
        {
            if (target != null && seeker.IsDone())
            {
                seeker.StartPath(rb.position, target.position, OnPathComplete);
            }
        }

        private void OnPathComplete(Path p)
        {
            if (!p.error)
            {
                path = p;
                currentWaypoint = 0;
            }
            else 
            {
                Debug.LogWarning(p.error);
            }
        }
    }
}