using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public Animator animator;
    public LayerMask enemyLayers;
    public Transform attackPoint; // Referenzpunkt für den Angriff
    public float attackRange = 0.5f; // Radius des Angriffsbereichs
    public int attackDamage = 40;
    public float attackRate = 2f;
    public GameObject debugAttackRangeVisualizer; // Referenz für das Visualisierungssprite
    public bool ifDebug = false;

    private float nextAttackTime = 0f;
    private TopDownPlayerController playerDirection; // Referenz auf den PlayerController

    private void Start()
    {
        // Get Referenz zum PlayerController
        playerDirection = GetComponent<TopDownPlayerController>();

        // Initialisiere das Attack-Visualizer-Sprite (falls zugewiesen)
        if (debugAttackRangeVisualizer != null)
        {
            UpdateAttackRangeVisualizer(); // Größe anpassen
            debugAttackRangeVisualizer.SetActive(false); // Zunächst ausblenden
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Synchronisiere die Position des Visualisierungssprites mit attackPoint
        if (debugAttackRangeVisualizer != null && attackPoint != null)
        {
            debugAttackRangeVisualizer.transform.position = attackPoint.position;
        }

        // Steuerung der Angriffsvisualisierung
        if (debugAttackRangeVisualizer != null && ifDebug)
        {
            if (Input.GetKey(KeyCode.R) || Input.GetMouseButton(0))
            {
                debugAttackRangeVisualizer.SetActive(true); // Visualisierung anzeigen
            }
            else
            {
                debugAttackRangeVisualizer.SetActive(false); // Visualisierung ausblenden
            }
        }

        // Angriff ausführen
        if (Time.time >= nextAttackTime)
        {
            if (Input.GetKeyDown(KeyCode.R) || Input.GetMouseButtonDown(0))
            {
                Attack();
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }
    }

    // Function to attack enemies in range
    void Attack()
    {
        animator.SetFloat("StayHorizontal", playerDirection.lastMoveDirection.x);
        animator.SetFloat("StayVertical", playerDirection.lastMoveDirection.y);
        // Play an Attack Animation
        animator.SetTrigger("Attack");
        // Detect enemies in range
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        // Deal damage to enemies
        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<Enemy>().TakeDamage(attackDamage);
        }

        Debug.Log("Attacking");
    }

    // Draw a gizmo to show the attack range
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    // Aktualisiert die Größe des Visualisierungs-Sprites basierend auf attackRange
    private void UpdateAttackRangeVisualizer()
    {
        if (debugAttackRangeVisualizer != null)
        {
            float scale = attackRange * 2; // attackRange repräsentiert den Radius, daher Durchmesser * 2
            debugAttackRangeVisualizer.transform.localScale = new Vector3(scale, scale, 1);
        }
    }
}
