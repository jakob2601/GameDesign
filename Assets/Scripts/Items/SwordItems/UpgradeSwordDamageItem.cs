using UnityEngine;
using Scripts.Combats.CharacterCombats; // Importiere das Combat-System
using Scripts.Combats.Weapons; // Importiere das Waffen-System

namespace Scripts.Items
{
    public class UpgradeSwordDamageItem : Item
    {
        [SerializeField] private int damageIncrease = 1; // Erhöht den Schaden um 1

        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player")) // Überprüfe, ob der Spieler das Upgrade berührt
            {
                Combat playerCombat = collision.GetComponentInChildren<Combat>(); // Suche das Combat-System des Spielers

                if (playerCombat != null && playerCombat.HasWeapon(Combat.WeaponTypes.Sword))
                {
                    Weapon sword = collision.GetComponentInChildren<Sword>();
                    if (sword != null)
                    {
                        sword.SetAttackDamage(sword.GetAttackDamage() + damageIncrease);
                        Debug.Log("Sword Damage erhöht! Neuer Wert: " + sword.GetAttackDamage());
                        Destroy(gameObject); // Zerstöre das Upgrade nach dem Aufsammeln
                    }
                }
                else
                {
                    Debug.LogError("Spieler hat kein Schwert gefunden!");
                }
            }
        }
    }
}
