using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 5; // Maximale Gesundheit
    public int currentHealth; // Aktuelle Gesundheit
    public HealthBarController healthBarController;

    private void Start()
    {
        // Setze die Gesundheit auf das Maximum
        currentHealth = maxHealth;

        // Initialisiere und aktualisiere die Health Bar
        healthBarController.InitializeHearts(maxHealth);
        healthBarController.UpdateHearts(currentHealth, maxHealth);
    }

    public void TakeDamage(int damage)
    {
        // Reduziere die Gesundheit
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // Update die Health Bar
        healthBarController.UpdateHearts(currentHealth, maxHealth);

        // Überprüfe, ob der Spieler tot ist
        if (currentHealth == 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        // Erhöhe die Gesundheit
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // Update die Health Bar
        healthBarController.UpdateHearts(currentHealth, maxHealth);
    }

    private void Die()
    {
        Debug.Log("Player has died!");
        // Hier kannst du eine Logik für den Tod des Spielers einfügen
    }
}
