using UnityEngine;
using System.Collections;
using Scripts.Movements;
using Scripts.Healths;
using Scripts.Movements.AI;

namespace Scripts.Combats.CharacterCombats {
    public abstract class Combat : MonoBehaviour
    {
        [SerializeField] public Animator animator;
        [SerializeField] public LayerMask enemyLayer;

        [SerializeField] public Rigidbody2D rb;

        [SerializeField] protected MovementAI playerDirection; // Referenz auf den PlayerController

        [SerializeField] public float attackRate = 2f;
        [SerializeField] protected float nextAttackTime = 0f;

        public bool ifDebug = false;
        public GameObject debugAttackRangeVisualizer; // Referenz für das Visualisierungssprite

        [SerializeField] public bool inCombat = false;

        protected virtual void Start()
        {
            rb = GetComponent<Rigidbody2D>();

            if (rb == null)
            {
                Debug.LogError("Rigidbody2D component not found on " + gameObject.name);
            }
            
            playerDirection = getCharacterDirection();
            if(playerDirection == null)
            {
                Debug.LogError("Player direction is not assigned.");
            }


        }

        abstract public MovementAI getCharacterDirection();
    }
}