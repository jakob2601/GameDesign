using UnityEngine;
using System.Collections;
using MyGame;

namespace Scripts.Movements.Moves
{
    public class Walking: Move
    {
        [SerializeField] private float moveSpeed = 5f;   

        public void SetMoveSpeed(float speed) {
            moveSpeed = speed;
        }
        
        public float GetMoveSpeed() {
            return moveSpeed;
        }
        public Vector2 getNewPosition(Vector2 position, Vector2 walkingInput) {
            return walkingInput * moveSpeed * Time.fixedDeltaTime + position;
        }
    } 
}