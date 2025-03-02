using UnityEngine;
using Scripts.Combats.CharacterCombats;

namespace Scripts.Items
{
    public abstract class Item : MonoBehaviour
    {
        protected abstract void OnTriggerEnter2D(Collider2D collision);
        
    }
}