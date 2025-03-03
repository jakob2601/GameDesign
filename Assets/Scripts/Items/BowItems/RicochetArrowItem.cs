using System.Collections;
using UnityEngine;
using Scripts.Combats.CharacterCombats;
using Scripts.Combats.Weapons;
using Scripts.Scene; // Add this for PlayerPersistence


namespace Scripts.Items
{
    public class RicochetArrowItem : Item
    {
        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                Combat playerCombat = collision.GetComponentInChildren<Combat>();

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
                            Debug.Log("Ricocheting arrows replace Piercing arrows!");
                        }

                        bow.SetRicochet(true);
                        Debug.Log("Pfeile prallen jetzt ab!");

                        // Fix the typo in method name
                        PlayerPersistence persistence = collision.GetComponent<PlayerPersistence>();
                        if (persistence != null)
                        {
                            persistence.EnableRicochet(true); // Fixed method name
                        }

                        if (isTemporary)
                        {
                            Debug.Log("Ricochet Buff expires after " + buffDuration + " seconds.");
                            StartCoroutine(RemoveBuffAfterDuration(bow));
                        }

                        Destroy(gameObject);
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
            bow.SetRicochet(false);

            // Update persistence when buff expires
            PlayerPersistence persistence = bow.GetComponentInParent<PlayerPersistence>();
            if (persistence != null)
            {
                persistence.EnableRicochet(false);
            }

            Debug.Log("Buff expired. Ricochet disabled.");
        }
    }
}