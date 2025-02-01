using UnityEngine;
using System.Collections;
using MyGame;

namespace Scripts.Movements.Moves
{
    public class Walking : Move
    {
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private Rigidbody2D rb;


        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                Debug.LogError("Rigidbody2D component not found on " + gameObject.name);
            }
        }


        public void SetMoveSpeed(float speed)
        {
            moveSpeed = speed;
        }

        public float GetMoveSpeed()
        {
            return moveSpeed;
        }
        public Vector2 getNewPosition(Vector2 position, Vector2 walkingInput)
        {
            return walkingInput * moveSpeed * Time.fixedDeltaTime + position;
        }

        public void Walk(Vector2 walkingInput)
        {
            if (rb == null)
            {
                return;
            }
            rb.MovePosition(rb.position + walkingInput * moveSpeed * Time.fixedDeltaTime);
        }
    }
}