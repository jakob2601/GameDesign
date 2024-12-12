using UnityEngine;

namespace Scripts.Movements
{
    public abstract class Movement : MonoBehaviour
    {
        public float moveSpeed = 200f; 
        public Animator animator;  // Animation für Character
        public Vector2 lastMoveDirection; // letzte Bewegungsrichtung
        protected Rigidbody2D rb; // Rigidbody2D-Komponente
        protected bool isFacingRight = false; // der Charakter wendet sich rechte Seite zu

        protected virtual void Start()
        {
            rb = GetComponent<Rigidbody2D>(); // Rigidbody2D zuweisen
            animator = GetComponent<Animator>(); // Animator zuweisen
        }

        protected virtual void FixedUpdate()
        {
        }

        protected void Flip()
        {
            // spiegel Sprite sheet, damit Charakter nach links guckt während er nach links läuft 
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
            isFacingRight = !isFacingRight;
        }
    }
}