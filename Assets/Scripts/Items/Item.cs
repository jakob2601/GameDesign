using UnityEngine;
using Scripts.Combats.CharacterCombats;

namespace Scripts.Items
{
    public abstract class Item : MonoBehaviour
    {
        [SerializeField] protected float timeToLive = 10f; // Time in seconds before the item is destroyed

        protected virtual void Start()
        {
            // Destroy the item after timeToLive seconds
            Destroy(gameObject, timeToLive);
        }

        protected abstract void OnTriggerEnter2D(Collider2D collision);
    }
}