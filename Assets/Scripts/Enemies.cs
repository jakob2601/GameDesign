using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour 
{
    public Animator animator;
    public int maxHealth = 100;
    private int currentHealth;
    public GameObject bloodParticlesPrefab;

    // Start is called before the first frame update
    void Start() {
        currentHealth = maxHealth;
    }
    // Update is called once per frame
    void Update() {
        
    }   

    public void TakeDamage(int damage) {
        currentHealth -= damage;

        // Play Hurt Animation
        Debug.Log("Enemy took " + damage + " damage.");
        animator.SetTrigger("Hurt");
        // Instanziere die Blutpartikel
        if (bloodParticlesPrefab != null)
        {
            Instantiate(bloodParticlesPrefab, transform.position, Quaternion.identity);
        }
    
        if(currentHealth <= 0) {
            Die();
        }
    }

    void Die() {
        
        Debug.Log("Enemy died.");

        // Play Death Animation
        animator.SetBool("IsDead", true);

        // Disable the enemy
        GetComponent<Collider2D>().enabled = false;  
        this.enabled = false;

        Destroy(gameObject, 3f);
    }
}