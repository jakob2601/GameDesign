using UnityEngine;
using System.Collections;
using Scripts.Healths;
using Scripts.Combats.CharacterCombats;
using Scripts.Movements.AI;

namespace Scripts.Combats.Weapons {
    public abstract class Weapon : MonoBehaviour 
    {
        public Animator animator;
        public Transform attackPoint; // Referenzpunkt für den Angriff
        public Combat combat; 

        [SerializeField] public float attackRange = 0.5f; // Radius des Angriffsbereichs
        [SerializeField] public int attackDamage = 40;
        [SerializeField] public float attackRate = 2f;
        [SerializeField] protected float nextAttackTime = 0f;
        [SerializeField] public float knockbackForce = 1f;

        protected bool ifDebug = false;
        protected GameObject debugAttackRangeVisualizer; // Referenz für das Visualisierungssprite


        protected virtual void Start()
        {
            // Get Referenz zum PlayerController
            attackPoint = transform.Find("AttackPoint");
            if(attackPoint == null)
            {
                Debug.LogError("AttackPoint not found on " + gameObject.name);
            }

            debugAttackRangeVisualizer = transform.Find("AttackPoint").gameObject;

            // Initialisiere das Attack-Visualizer-Sprite (falls zugewiesen)
            if (debugAttackRangeVisualizer != null)
            {
                UpdateAttackRangeVisualizer(); // Größe anpassen
                debugAttackRangeVisualizer.SetActive(false); // Zunächst ausblenden
            }

            else 
            {
                Debug.LogWarning("Attack Range Visualizer not found on " + gameObject.name);
            }

        }

        protected virtual void Update() 
        {
            // Synchronisiere die Position des Visualisierungssprites mit attackPoint
            if (debugAttackRangeVisualizer != null && attackPoint != null)
            {
                debugAttackRangeVisualizer.transform.position = attackPoint.position;
            }
            if(!combat.inCombat) 
            {
                debugAttackRangeVisualizer.SetActive(false);
            }
            
        }
        public virtual void PerformAttack(MovementAI characterDireciton, LayerMask enemyLayers) 
        {
            debugAttackRangeVisualizer.SetActive(true); // Visualisierung anzeigen

        }


        protected void OnDrawGizmosSelected()
        {
            if (attackPoint == null)
                return;

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }

        // Aktualisiert die Größe des Visualisierungs-Sprites basierend auf attackRange
        protected void UpdateAttackRangeVisualizer()
        {
            if (debugAttackRangeVisualizer != null)
            {
                float scale = attackRange * 2; // attackRange repräsentiert den Radius, daher Durchmesser * 2
                debugAttackRangeVisualizer.transform.localScale = new Vector3(scale, scale, 1);
            }
        }
    }
}