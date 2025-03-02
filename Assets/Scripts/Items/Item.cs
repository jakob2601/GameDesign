using UnityEngine;
using Scripts.Combats.CharacterCombats;

namespace Scripts.Items
{
    public abstract class Item : MonoBehaviour
    {
        [Header("Expire Properties")]
        [SerializeField] protected float timeToLive = 10f; // Time in seconds before the item is destroyed
        [SerializeField] protected bool canExpire = true; // Whether the item can expire
        [Header("Item Properties")]
        [SerializeField] protected int amount = 1; // Amount of item
        //[SerializeField] protected Audioclip pickupSound; // Sound to play when the item is picked up
        

        protected virtual void Start()
        {
            // Destroy the item after timeToLive seconds
            if(canExpire) 
            {
                Destroy(gameObject, timeToLive);
            }
        }

        protected abstract void OnTriggerEnter2D(Collider2D collision);
    }
}