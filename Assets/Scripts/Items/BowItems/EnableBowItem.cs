using UnityEngine;
using Scripts.Combats.CharacterCombats;
using Scripts.Scene; // Add this for PlayerPersistence

namespace Scripts.Items
{
    public class EnableBowItem : Item
    {
        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                Combat playerCombat = collision.GetComponentInChildren<Combat>();

                if (playerCombat != null)
                {
                    playerCombat.SetWeaponAvailable(Combat.WeaponTypes.Bow, true);

                    // Save to persistence
                    PlayerPersistence persistence = collision.GetComponent<PlayerPersistence>();
                    if (persistence != null)
                    {
                        persistence.EnableBow(true);
                    }

                    Debug.Log("Bow enabled for player!");
                    Destroy(gameObject);
                }
                else
                {
                    Debug.LogError("Player combat component not found!");
                }
            }
        }
    }
}