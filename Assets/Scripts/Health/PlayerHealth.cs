using System.Collections;
using Scripts.UI;
using UnityEngine;
using Scripts.Combats.CharacterCombats;
using Scripts.Combats.Weapons;
using System.Threading;

namespace Scripts.Healths
{
    public class PlayerHealth : Health
    {   
        public HealthBarController healthBarController;


        protected override void Start()
        {
            // Rufe die gemeinsame Initialisierung der Basisklasse auf
            base.Start();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            //Initialisieren Collision zu Gegner
            Weapon weapon = collision.GetComponent<Weapon>();
            EnemyCombat enemy = collision.GetComponent<EnemyCombat>();

            if (weapon && enemy)
            {
                float knockbackForce = weapon.knockbackForce;
                Vector2 hitDirection = (transform.position - enemy.transform.position).normalized;
                TakeDamage(weapon.attackDamage, hitDirection, knockbackForce);
            }
            else if(!weapon)
            {
                Debug.Log("Weapon script not found");
            }
            else if(!enemy)
            {
                Debug.Log("Enemy script not found");
            }
        }

        public override void TakeDamage(int damage, Vector2 hitDirection, float knockbackForce)
        {
            base.TakeDamage(damage, hitDirection, knockbackForce);
        }
    
        protected override void initializeHealthBar(int maxHealth)
        {
            // Initialisiere die Health Bar
            healthBarController.InitializeHearts(maxHealth);
        }

        protected override void updateHealthBar(int currentHealth, int maxHealth)
        {
            // Update die Health Bar
            healthBarController.UpdateHearts(currentHealth, maxHealth);
        }

        public override void Heal(int amount)
        {
            base.Heal(amount);
            // Update die Health Bar
            healthBarController.UpdateHearts(currentHealth, maxHealth);
        }

        protected override void Die()
        {
            Debug.Log("Player has died!");
            // Hier kannst du eine Logik für den Tod des Spielers einfügen
        }
    }

}
