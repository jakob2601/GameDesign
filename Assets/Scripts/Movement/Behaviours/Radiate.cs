using UnityEngine;
using Pathfinding;
using System.Collections;
using Scripts.Combats.CharacterCombats;
using Scripts.Movements.Moves;

namespace Scripts.Movements.Behaviours
{
    public class Radiate: MonoBehaviour 
    {
        [SerializeField] protected float angle = 0f;
        [SerializeField] protected float circleRadius = 3f; // Radius, innerhalb dessen das Verhalten geändert wird
        [SerializeField] protected bool isRadiating = false;
        [SerializeField] protected bool isEnabled = false;
        [SerializeField] protected bool isUnblocked = true;
        [SerializeField] protected Walking walking;
        [SerializeField] protected Rigidbody2D rb;


        public float GetAngle() 
        {
            return angle;
        }

        public void SetAngle(float angle) 
        {
            this.angle = angle;
        }

        public float GetCircleRadius() 
        {
            return circleRadius;
        }

        protected void SetCircleRadius(float circleRadius) 
        {
            this.circleRadius = circleRadius;
        }   

        public bool GetIsRadiating() 
        {
            return isRadiating;
        }

        protected void SetIsRadiating(bool isRadiating) 
        {
            this.isRadiating = isRadiating;
        }

        public bool GetIsEnabled() 
        {
            return isEnabled;
        }

        protected void SetIsEnabled(bool enabled) 
        {
            isEnabled = enabled;
        }

        public void SetIsUnblocked(bool unblocked) 
        {
            isUnblocked = unblocked;
        }

        public bool GetIsUnblocked() 
        {
            return isUnblocked;
        }

        protected Walking GetWalking() 
        {
            return walking;
        }

        protected void SetWalking(Walking walking) 
        {
            this.walking = walking;
        }

        protected Rigidbody2D GetRigidbody() 
        {
            return rb;
        }

        protected void SetRigidbody(Rigidbody2D rb) 
        {
            this.rb = rb;
        }


        public void Start()
        {
            walking = GetComponent<Walking>();
            if (walking == null)
            {
                Debug.LogError("Walking component not found on " + gameObject.name);
            }

            rb = GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                Debug.LogError("Rigidbody2D component not found on " + gameObject.name);
            }
        }

        public void Update() 
        {
            
        }


        public void RadiateAroundTarget(Transform target, float combatSpeed, ref Vector2 lastMoveDirection) 
        {
            if (walking == null)
            {
                Debug.LogError("Walking component not found on " + gameObject.name);
                return;
            }

            if(isUnblocked == false) 
            {
                Debug.Log("Radiate is blocked");
                return;
            }
            if(isEnabled == false) 
            {
                return;
            }
            
            isRadiating = true;
            angle += combatSpeed * Time.deltaTime;
            float x = Mathf.Cos(angle) * circleRadius;
            float y = Mathf.Sin(angle) * circleRadius;
            Vector2 offset = new Vector2(x, y);
            Vector2 targetPosition = (Vector2)target.position + offset;                    

            Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
            lastMoveDirection = direction;
 
            rb.MovePosition(walking.getNewPosition(rb.position, direction));

            isRadiating = false;
        }
    }
}