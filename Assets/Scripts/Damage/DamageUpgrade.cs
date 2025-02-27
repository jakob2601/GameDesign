using UnityEngine;
using Scripts.Combats.Weapons; // Importiere das Waffen-System

public class DamageUpgrade : MonoBehaviour
{
    [SerializeField] private int damageIncrease = 1; // Erhöht den Schaden um 1

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // Überprüfe, ob der Spieler das Upgrade berührt
        {
            Weapon playerWeapon = collision.GetComponentInChildren<Weapon>(); // Suche die Waffe des Spielers

            if (playerWeapon != null)
            {
                // Erhöhe den Schaden
                playerWeapon.SetAttackDamage(playerWeapon.GetAttackDamage() + damageIncrease);
                Debug.Log("Attack Damage erhöht! Neuer Wert: " + playerWeapon.GetAttackDamage());

                // Zerstöre das Upgrade nach dem Aufsammeln
                Destroy(gameObject);
            }
            else
            {
                Debug.LogError("Spieler hat keine Waffe gefunden!");
            }
        }
    }
}
