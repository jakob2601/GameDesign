using UnityEngine;
using Scripts.Combats.CharacterCombats;
using Scripts.Combats.Weapons;
using Scripts.Scene; // Add this for PlayerPersistence

namespace Scripts.Items
{
    public class UpgradeSwordDamageItem : Item
    {
        [SerializeField] private int damageIncrease = 1;

        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                Combat playerCombat = collision.GetComponentInChildren<Combat>();

                if (playerCombat != null)
                {
                    // Enable sword if not already enabled
                    if (!playerCombat.HasWeapon(Combat.WeaponTypes.Sword))
                    {
                        playerCombat.SetWeaponAvailable(Combat.WeaponTypes.Sword, true);
                        Debug.Log("Player can now use a sword.");
                    }

                    Weapon sword = collision.GetComponentInChildren<Sword>();
                    if (sword != null)
                    {
                        int newDamage = sword.GetAttackDamage() + damageIncrease;
                        sword.SetAttackDamage(newDamage);

                        // Update PlayerPersistence with new sword damage
                        PlayerPersistence persistence = collision.GetComponent<PlayerPersistence>();
                        if (persistence != null)
                        {
                            persistence.swordDamage = newDamage;
                            Debug.Log("Sword damage saved to persistence: " + newDamage);
                        }

                        Debug.Log("Sword Damage erhöht! Neuer Wert: " + newDamage);
                        Destroy(gameObject);
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