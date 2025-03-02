using UnityEngine;
using Scripts.Combats.CharacterCombats; // Importiere das Combat-System
using Scripts.Combats.Weapons; // Importiere das Waffen-System

namespace Scripts.Items
{
    public class UpgradeWeaponDamageItem : Item
    {
        [SerializeField] private int damageIncrease = 1; // Erhöht den Schaden um 1

        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player")) // Überprüfe, ob der Spieler das Upgrade berührt
            {
                Combat playerCombat = collision.GetComponentInChildren<Combat>(); // Suche das Combat-System des Spielers

                if (playerCombat != null)
                {
                    // Erhöhe den Schaden für alle verfügbaren Waffen
                    foreach (Combat.WeaponTypes weaponType in System.Enum.GetValues(typeof(Combat.WeaponTypes)))
                    {
                        if (weaponType == Combat.WeaponTypes.None) continue;

                        // Enable weapon if not already enabled
                        if (!playerCombat.HasWeapon(weaponType))
                        {
                            playerCombat.SetWeaponAvailable(weaponType, true);
                            Debug.Log("Player can now use " + weaponType + ".");
                        }

                        if (playerCombat.HasWeapon(weaponType))
                        {
                            System.Type weaponClassType = System.Type.GetType("Scripts.Combats.Weapons." + weaponType.ToString());
                            Weapon weapon = collision.GetComponentInChildren(weaponClassType) as Weapon;
                            if (weapon != null)
                            {
                                weapon.SetAttackDamage(weapon.GetAttackDamage() + damageIncrease);
                                Debug.Log(weaponType + " Damage erhöht! Neuer Wert: " + weapon.GetAttackDamage());
                            }
                        }
                    }

                    // Zerstöre das Upgrade nach dem Aufsammeln
                    Destroy(gameObject);
                }
                else
                {
                    Debug.LogError("Spieler hat kein Combat-System gefunden!");
                }
            }
        }
    }
}
