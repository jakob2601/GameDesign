using UnityEngine;
using Pathfinding;
using System.Collections;
using Scripts.Combats.CharacterCombats;
using Scripts.Movements.Moves;
using Scripts.Movements.AI;

namespace Scripts.Movements.Behaviours
{
    public class Unstuck: MonoBehaviour 
    {
        [SerializeField] private bool isUnstucking = false;
        [SerializeField] private float timeSinceStuck = 0f;

        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private Walking walking;
        [SerializeField] private MovementAI movementAI;

        private Transform characterTransform;
        [SerializeField] private float timeSinceLastUpdate = 0f;
        [SerializeField] private float stuckTimerThreshold = 1f; // Zeit, die der Gegner feststecken muss, um als feststeckend zu gelten
        [SerializeField] private float stuckCheckInterval = 1f; // Intervall, um zu überprüfen, ob der Gegner feststeckt
        [SerializeField] private float startStuckCheckInterval = 0f; // Startintervall, um zu überprüfen, ob der Gegner feststeckt
        [SerializeField] public float stuckDistanceThreshold = 0.1f; // Schwellenwert, um festzustellen, ob der Gegner feststeckt
        [SerializeField] public float unstuckDuration = 1f; // Dauer der Ausweichbewegung

        public bool getIsUnstucking() 
        {
            return isUnstucking;
        }

        public float GetStartStuckCheckInterval() 
        {
            return startStuckCheckInterval;
        }

        public float GetStuckCheckInterval() 
        {
            return stuckCheckInterval;
        }

        public void Start() 
        {
            rb = GetComponent<Rigidbody2D>();
            if(rb == null) 
            {
                Debug.LogError("Rigidbody2D component not found on " + gameObject.name);
            }

            walking = GetComponent<Walking>();
            if(walking == null) 
            {
                Debug.LogError("Walking component not found on " + gameObject.name);
            }

            movementAI = GetComponent<MovementAI>();
            if(movementAI == null) 
            {
                Debug.LogError("MovementAI component not found on " + gameObject.name);
            }

            characterTransform = GetComponent<Transform>();
            if(characterTransform == null) 
            {
                Debug.LogError("Transform component not found on " + gameObject.name);
            }
            
        }

        public void Update() 
        {
            if (timeSinceLastUpdate >= stuckCheckInterval)
            {
                CheckIfStuck();
                timeSinceLastUpdate = 0f;
            }
            else 
            {
                timeSinceLastUpdate += Time.deltaTime;
            }
        }

        private void CheckIfStuck()
        {
            Collider2D[] obstacles = Physics2D.OverlapCircleAll(characterTransform.position, stuckDistanceThreshold, LayerMask.GetMask("Obstacle"));
            if (obstacles.Length > 0)
            {
                timeSinceStuck += Time.deltaTime;
                if (timeSinceStuck >= stuckTimerThreshold && !isUnstucking)
                {
                    StartCoroutine(PerformUnstuck(obstacles[0].transform.position, rb));
                    isUnstucking = true;
                }
                else {
                    isUnstucking = false;
                }
            }
            else
            {   
                timeSinceStuck = 0f;
                isUnstucking = false;
            }
        }

        public IEnumerator PerformUnstuck(Vector2 obstaclePosition, Rigidbody2D rb)
        {
            if (walking == null)
            {
                Debug.LogError("Walking component not found on " + gameObject.name);
                yield break;
            }

            isUnstucking = true;
            Vector2 directionAwayFromObstacle = ((Vector2)characterTransform.position - obstaclePosition).normalized;
            float timer = 0f;

            while (timer < unstuckDuration)
            {
                Vector2 newPosition = walking.getNewPosition(rb.position, directionAwayFromObstacle);
                rb.MovePosition(newPosition);
                movementAI.lastMoveDirection = directionAwayFromObstacle;
                timer += Time.deltaTime;
                yield return null;
            }

            isUnstucking = false;
        }

    }
}