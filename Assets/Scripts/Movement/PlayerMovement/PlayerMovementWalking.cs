using UnityEngine;
using System.Collections;

namespace Scripts.Movements
{
    public partial class PlayerMovement: Movement
    {
        protected Vector2 walkingInput; // Bewegungseingabe


        protected void ProccessInputs()
        {
            float moveX = Input.GetAxisRaw("Horizontal");
            float moveY = Input.GetAxisRaw("Vertical");

            // Toleranz für kleine Eingabewerte
            if (Mathf.Abs(moveX) < 0.1f) moveX = 0;
            if (Mathf.Abs(moveY) < 0.1f) moveY = 0;

            if (moveX != 0 || moveY != 0)
            {
                lastMoveDirection = new Vector2(moveX, moveY).normalized;
            }

            walkingInput = new Vector2(moveX, moveY).normalized;
        }
    }
}