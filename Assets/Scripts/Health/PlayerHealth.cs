using System.Collections;
using Scripts.UI;
using UnityEngine;
using Scripts.Combat;

namespace Scripts.Health
{
    public class PlayerHealth : MonoBehaviour
    {
        public int maxHealth = 5; // Maximale Gesundheit
        public int currentHealth; // Aktuelle Gesundheit
        public HealthBarController healthBarController;

        private SpriteRenderer spriteRenderer;

        private void Start()
        {
            // Setze die Gesundheit auf das Maximum
            currentHealth = maxHealth;

            // Initialisiere und aktualisiere die Health Bar
            healthBarController.InitializeHearts(maxHealth);
            healthBarController.UpdateHearts(currentHealth, maxHealth);

            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            //Initialisieren Collision zu Gegner
            Enemy enemy = collision.GetComponent<Enemy>();

            if (enemy)
            {
                TakeDamage(enemy.damage);
            }
        }

        public void TakeDamage(int damage)
        {
            // Reduziere die Gesundheit
            currentHealth -= damage;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

            // Update die Health Bar
            healthBarController.UpdateHearts(currentHealth, maxHealth);

            //Rot aufleuchten nach Damage
            StartCoroutine(FlashRed());

            // Überprüfe, ob der Spieler tot ist
            if (currentHealth == 0)
            {
                Die();
            }
        }

        private IEnumerator FlashRed()
        {
            // Farbänderung nach Damage 
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.2f);
            spriteRenderer.color = Color.white;
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

}
