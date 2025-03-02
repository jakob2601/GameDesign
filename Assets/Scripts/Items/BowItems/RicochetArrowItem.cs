using UnityEngine;
using Scripts.Combats.CharacterCombats; // Importiere das Combat-System
using Scripts.Combats.Weapons; // Importiere das Waffen-System

namespace Scripts.Items
{
    public class RicochetArrowItem : Item
    {
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

                    Bow bow = collision.GetComponentInChildren<Bow>();
                    if (bow != null)
                    {
                        // Check if pierce is enabled before replacing
                        if (bow.currentSpecialArrowType == Bow.SpecialArrowType.Pierce)
                        {
                            // Show replacement message
                            Debug.Log("Ricocheting arrows replace Piercing arrows!");
                            
                            // Here you could also show a UI message to the player
                            // UIManager.Instance.ShowMessage("Ricocheting arrows replace Piercing arrows!");
                        }
                        
                        bow.SetRicochet(true);
                        Debug.Log("Pfeile prallen jetzt ab!");
                        Destroy(gameObject); // Zerstöre das Upgrade nach dem Aufsammeln
                    }
                }
                else
                {
                    Debug.LogWarning("Player has no bow equipped!");
                }
            }
        }
    }
}