using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour 
{
    public Animator animator;
    public int maxHealth = 100;
    private int currentHealth;
    public GameObject bloodParticlesPrefab; // Referenz zum Blut-Partikel-Prefab
    public float knockbackForce = 10f; // Stärke des Rückstoßes

    public Rigidbody2D rb;

    // Start is called before the first frame update
    void Start() {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            Debug.LogError("Rigidbody2D component not found on " + gameObject.name);
        }
    }

    // Update is called once per frame
    void Update() {
        
    }

    public void TakeDamage(int damage, Vector2 hitDirection) {
        currentHealth -= damage;

        // Play Hurt Animation
        Debug.Log("Enemy took " + damage + " damage.");
        animator.SetTrigger("Hurt");

        // Blutpartikel abspielen
        SpawnBloodParticles();

        // Rückstoß anwenden
        ApplyKnockback(hitDirection);

        if(currentHealth <= 0) {
            Die();
        }
    }

    void ApplyKnockback(Vector2 hitDirection)
    {
        if (rb != null)
        {
            Debug.Log("Applying knockback");
            // Normalisiere die hitDirection und multipliziere sie mit der knockbackForce
            Vector2 force = hitDirection.normalized * knockbackForce;
            rb.AddForce(force, ForceMode2D.Impulse);
        }
    }

    void SpawnBloodParticles() {
        if (bloodParticlesPrefab != null) {
            // Erstelle die Partikel an der Position des Gegners
            GameObject tempParticle = Instantiate(bloodParticlesPrefab, transform.position, Quaternion.identity);
            Destroy(tempParticle, 2f); // Zerstöre die Partikel nach 2 Sekunden
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
