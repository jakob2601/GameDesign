using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerCombat : MonoBehaviour
{
    public Animator animator;
    public LayerMask enemyLayers;
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public int attackDamage = 40;
    public float attackRate = 2f;

    private float nextAttackTime = 0f;
    private TopDownPlayerController playerDirection; //Referenz auf den PlayerController

    private void Start()
    {
        // get Referenz zum playerController
        playerDirection = GetComponent<TopDownPlayerController>();
    }


    // Update is called once per frame
    void Update()
    {
        if(Time.time >= nextAttackTime) 
        {
            if(Input.GetKeyDown(KeyCode.R))
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
        foreach(Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<Enemy>().TakeDamage(attackDamage);
        }

        Debug.Log("Attacking");
    }


    // Draw a gizmo to show the attack range
    void OnDrawGizmosSelected()
    {
        if(attackPoint == null)
        {
            return;
        }
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
