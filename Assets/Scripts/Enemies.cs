using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour 
{
    public Animator animator;
    public int maxHealth = 100;
    public int damage = 1;
    private int currentHealth;
    public GameObject bloodParticlesPrefab; // Referenz zum Blut-Partikel-Prefab

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

        // Blutpartikel abspielen
        SpawnBloodParticles();

        if(currentHealth <= 0) {
            Die();
        }
    }

    void SpawnBloodParticles() {
        if (bloodParticlesPrefab != null) {
            // Erstelle die Partikel an der Position des Gegners
            GameObject tempParticle = Instantiate(bloodParticlesPrefab, transform.position, Quaternion.identity);
            Destroy(tempParticle, 1f);
        } else {
            Debug.LogWarning("No blood particle prefab assigned!");
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
