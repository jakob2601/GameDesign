using UnityEngine;
using System.Collections;
using Scripts.Movements;
using Scripts.Healths;

namespace Scripts.Combats.CharacterCombats {
    public abstract class Combat : MonoBehaviour
    {
        public Animator animator;
        public LayerMask enemyLayers;

        public Rigidbody2D rb;

        protected Movement playerDirection; // Referenz auf den PlayerController

        public float attackRate = 2f;
        protected float nextAttackTime = 0f;

        public bool ifDebug = false;
        public GameObject debugAttackRangeVisualizer; // Referenz für das Visualisierungssprite

        public bool inCombat = false;

        protected virtual void Start()
        {
            rb = GetComponent<Rigidbody2D>();

            if (rb == null)
            {
                Debug.LogError("Rigidbody2D component not found on " + gameObject.name);
            }
            // Get Referenz zum PlayerController
            playerDirection = getCharacterDirection();
        }

        abstract public Movement getCharacterDirection();
    }
}