using UnityEngine;
using System.Collections;
using Scripts.Healths;
using Scripts.Combats.CharacterCombats;
using Scripts.Movements;

namespace Scripts.Combats.Weapons {
    public abstract class Weapon : MonoBehaviour 
    {
        public Animator animator;
        public Transform attackPoint; // Referenzpunkt für den Angriff
        public Combat combat; 

        public float attackRange = 0.5f; // Radius des Angriffsbereichs
        public int attackDamage = 40;
        public float attackRate = 2f;
        protected float nextAttackTime = 0f;
        public float knockbackForce = 1f;

        protected bool ifDebug = false;
        protected GameObject debugAttackRangeVisualizer; // Referenz für das Visualisierungssprite


        protected virtual void Start()
        {
            // Get Referenz zum PlayerController
            attackPoint = transform.Find("AttackPoint");
            debugAttackRangeVisualizer = transform.Find("AttackPoint").gameObject;

            // Initialisiere das Attack-Visualizer-Sprite (falls zugewiesen)
            if (debugAttackRangeVisualizer != null)
            {
                UpdateAttackRangeVisualizer(); // Größe anpassen
                debugAttackRangeVisualizer.SetActive(false); // Zunächst ausblenden
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
        public virtual void PerformAttack(Movement characterDireciton, LayerMask enemyLayers) 
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