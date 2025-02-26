using UnityEngine;
using Pathfinding;
using System.Collections;
using Scripts.Combats.CharacterCombats;
using Scripts.Movements.Moves;
using Scripts.Movements.AI;

namespace Scripts.Movements.Behaviours
{
    public class Knockback : MonoBehaviour
    {
        [SerializeField] private bool isKnockbackActive = false;

        private void SetKnockbackActive(bool active)
        {
            isKnockbackActive = active;
        }

        public bool GetKnockbackActive()
        {
            return isKnockbackActive;
        }

        public IEnumerator KnockbackCharacter(Rigidbody2D rb, Vector2 hitDirection, float knockbackForce, float knockbackDuration)
        {
            if (rb == null)
            {
                Debug.LogWarning("Rigidbody is null");
                yield break;
            }

            else if (!isKnockbackActive)
            {
                isKnockbackActive = true;
                Debug.Log("Applying Knockback");
                Vector2 knockback = hitDirection.normalized * knockbackForce;
                rb.AddForce(knockback, ForceMode2D.Impulse);

                yield return new WaitForSeconds(knockbackDuration);

                Debug.Log("Go to Normal State");
                rb.velocity = Vector2.zero;
                isKnockbackActive = false;
            }
            else
            {
                Debug.Log("Knockback is already active");
            }
        }
    }
}