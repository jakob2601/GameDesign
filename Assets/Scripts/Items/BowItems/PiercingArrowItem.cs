using System.Collections;
using UnityEngine;
using Scripts.Combats.CharacterCombats; // Importiere das Combat-System
using Scripts.Combats.Weapons; // Importiere das Waffen-System

namespace Scripts.Items
{
    public class PiercingArrowItem : Item
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
                        // Check if ricochet is enabled before replacing
                        if (bow.currentSpecialArrowType == Bow.SpecialArrowType.Ricochet)
                        {
                            // Show replacement message
                            Debug.Log("Piercing arrows replace Ricocheting arrows!");
                            
                            // Here you could also show a UI message to the player
                            // UIManager.Instance.ShowMessage("Piercing arrows replace Ricocheting arrows!");
                        }
                        
                        bow.SetPierce(true);
                        Debug.Log("Pfeile gehen jetzt durch Gegner durch!");

                        if (isTemporary)
                        {
                            Debug.Log("Piercing Buff expires after " + buffDuration + " seconds.");
                            StartCoroutine(RemoveBuffAfterDuration(bow));
                        }

                        Destroy(gameObject); // Zerstöre das Upgrade nach dem Aufsammeln
                    }
                }
                else
                {
                    Debug.LogWarning("Player has no bow equipped!");
                }
            }
        }

        private IEnumerator RemoveBuffAfterDuration(Bow bow)
        {
            yield return new WaitForSeconds(buffDuration);
            bow.SetPierce(false);
            Debug.Log("Buff expired. Pierce disabled.");
        }
    }
}