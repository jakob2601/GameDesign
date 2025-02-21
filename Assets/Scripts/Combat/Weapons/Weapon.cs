using UnityEngine;
using System.Collections;
using Scripts.Healths;
using Scripts.Combats.CharacterCombats;
using Scripts.Movements.AI;

namespace Scripts.Combats.Weapons
{
    public abstract class Weapon : MonoBehaviour
    {
        [Header("Weapon Properties")]
        [SerializeField] public Animator animator;
        [SerializeField] public Transform attackPoint; // Referenzpunkt für den Angriff
        [SerializeField] public Combat combat;
        [SerializeField] public LayerMask enemyLayer;
        [SerializeField] public MovementAI characterMovement;
        
        [Header("Attack Properties")]
        [SerializeField] public float attackRange = 0.5f; // Radius des Angriffsbereichs
        [SerializeField] public int attackDamage = 40;
        [SerializeField] public float attackRate = 2f;
        [SerializeField] protected float nextAttackTime = 0f;
        [SerializeField] public float knockbackForce = 1f;
        [SerializeField] public float knockbackDuration = 0.2f;

        protected bool ifDebug = false;
        protected GameObject debugAttackRangeVisualizer; // Referenz für das Visualisierungssprite

        public Animator GetAnimator()
        {
            return animator;
        }

        protected Animator SetAnimator(Animator animator)
        {
            return this.animator = animator;
        }

        public Transform GetAttackPoint()
        {
            return attackPoint;
        }

        public void SetAttackPoint(Transform attackPoint)
        {
            this.attackPoint = attackPoint;
        }

        public Combat GetCombat()
        {
            return combat;
        }

        public void SetCombat(Combat combat)
        {
            this.combat = combat;
        }

        public LayerMask GetEnemyLayer()
        {
            return enemyLayer;
        }

        public void SetEnemyLayer(LayerMask enemyLayer)
        {
            this.enemyLayer = enemyLayer;
        }

        public float GetAttackRange()
        {
            return attackRange;
        }

        public void SetAttackRange(float attackRange)
        {
            this.attackRange = attackRange;
        }

        public int GetAttackDamage()
        {
            return attackDamage;
        }

        public void SetAttackDamage(int attackDamage)
        {
            this.attackDamage = attackDamage;
        }

        public float GetAttackRate()
        {
            return attackRate;
        }

        public void SetAttackRate(float attackRate)
        {
            this.attackRate = attackRate;
        }

        public float GetNextAttackTime()
        {
            return nextAttackTime;
        }

        public void SetNextAttackTime(float nextAttackTime)
        {
            this.nextAttackTime = nextAttackTime;
        }

        public float GetKnockbackForce()
        {
            return knockbackForce;
        }

        public void SetKnockbackForce(float knockbackForce)
        {
            this.knockbackForce = knockbackForce;
        }

        public float GetKnockbackDuration()
        {
            return knockbackDuration;
        }

        public void SetKnockbackDuration(float knockbackDuration)
        {
            this.knockbackDuration = knockbackDuration;
        }

        protected bool GetIfDebug()
        {
            return ifDebug;
        }

        protected void SetIfDebug(bool ifDebug)
        {
            this.ifDebug = ifDebug;
        }

        public GameObject GetDebugAttackRangeVisualizer()
        {
            return debugAttackRangeVisualizer;
        }

        public void SetDebugAttackRangeVisualizer(GameObject debugAttackRangeVisualizer)
        {
            this.debugAttackRangeVisualizer = debugAttackRangeVisualizer;
        }

        protected virtual void Start()
        {
            // Get Referenz zum PlayerController
            attackPoint = transform.Find("AttackPoint");
            if (attackPoint == null)
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

            characterMovement = GetComponent<MovementAI>();
            if (characterMovement == null)
            {
                Debug.LogError("MovementAI component not found on " + gameObject.name);
            }

        }

        protected virtual void Update()
        {
            // Synchronisiere die Position des Visualisierungssprites mit attackPoint
            if (debugAttackRangeVisualizer != null && attackPoint != null)
            {
                debugAttackRangeVisualizer.transform.position = attackPoint.position;
            }
            if (!combat.inCombat)
            {
                debugAttackRangeVisualizer.SetActive(false);
            }

        }

        protected virtual void FixedUpdate()
        {

        }

        public virtual void PerformAttack()
        {
            debugAttackRangeVisualizer.SetActive(true); // Visualisierung anzeigen
        }

        /*
        protected void OnDrawGizmosSelected()
        {
            if (attackPoint == null)
                return;

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }*/

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