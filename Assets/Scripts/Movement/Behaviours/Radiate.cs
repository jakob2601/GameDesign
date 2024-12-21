using UnityEngine;
using Pathfinding;
using System.Collections;
using Scripts.Combats.CharacterCombats;
using Scripts.Movements.Moves;

namespace Scripts.Movements.Behaviours
{
    public class Radiate: MonoBehaviour 
    {
        [SerializeField] private float angle = 0f;
        [SerializeField] private float circleRadius = 3f; // Radius, innerhalb dessen das Verhalten geändert wird
        [SerializeField] private bool isRadiating = false;
        [SerializeField] private Walking walking;

        private void Start()
        {
            walking = GetComponent<Walking>();
            if (walking == null)
            {
                Debug.LogError("Walking component not found on " + gameObject.name);
            }
        }

        public bool GetIsRadiating() {
            return isRadiating;
        }

        public void SetCircleRadius(float radius) 
        {
            circleRadius = radius;
        }

        public float GetCircleRadius() 
        {
            return circleRadius;
        }


        public void RadiateAroundTarget(Transform target, float combatSpeed, Rigidbody2D rb, ref Vector2 lastMoveDirection) 
        {
            if (walking == null)
            {
                Debug.LogError("Walking component not found on " + gameObject.name);
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