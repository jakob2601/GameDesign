using UnityEngine;
using Scripts.Combats.CharacterCombats; // Importiere das Combat-System
using Scripts.Combats.Weapons; // Importiere das Waffen-System

namespace Scripts.Items
{
    public class UpgradeBowDamageItem : Item
    {
        [SerializeField] private int damageIncrease = 1; // Erhöht den Schaden um 1

        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player")) // Überprüfe, ob der Spieler das Upgrade berührt
            {
                Combat playerCombat = collision.GetComponentInChildren<Combat>(); // Suche das Combat-System des Spielers

                if (playerCombat != null)
                {
                    // Enable bow if not already enabled
                    if (!playerCombat.HasWeapon(Combat.WeaponTypes.Bow))
                    {
                        playerCombat.SetWeaponAvailable(Combat.WeaponTypes.Bow, true);
                        Debug.Log("Player can now use a bow.");
                    }

                    Weapon bow = collision.GetComponentInChildren<Bow>();
                    if (bow != null)
                    {
                        bow.SetAttackDamage(bow.GetAttackDamage() + damageIncrease);
                        Debug.Log("Bow Damage erhöht! Neuer Wert: " + bow.GetAttackDamage());
                        Destroy(gameObject); // Zerstöre das Upgrade nach dem Aufsammeln
                    }
                }
                else
                {
                    Debug.LogError("Spieler hat kein Combat-System gefunden!");
                }
            }
        }
    }
}
