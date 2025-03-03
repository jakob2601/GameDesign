using UnityEngine;
using Scripts.Combats.CharacterCombats;
using Scripts.Movements.Moves;
using Scripts.Scene; // Add this for PlayerPersistence

namespace Scripts.Items
{
    public class UpgradeMoveSpeedItem : Item
    {
        [SerializeField] protected float moveSpeedIncrease = 0.5f; // Erhöht die Bewegungsgeschwindigkeit um 1

        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player")) // Überprüfe, ob der Spieler das Upgrade berührt
            {
                Walking walking = collision.GetComponentInChildren<Walking>(); // Suche das Walking-System des Spielers

                if (walking != null)
                {
                    float newSpeed = walking.GetMoveSpeed() + moveSpeedIncrease;
                    walking.SetMoveSpeed(newSpeed);

                    // Save to persistence system
                    PlayerPersistence persistence = collision.GetComponent<PlayerPersistence>();
                    if (persistence != null)
                    {
                        persistence.UpdateMoveSpeed(newSpeed);
                    }

                    Debug.Log("Move Speed erhöht! Neuer Wert: " + walking.GetMoveSpeed());
                    Destroy(gameObject); // Zerstöre das Upgrade nach dem Aufsammeln
                }
                else
                {
                    Debug.LogError("Spieler hat kein Walking gefunden!");
                }
            }
        }
    }
}