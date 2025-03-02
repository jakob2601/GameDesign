using UnityEngine;
using System.Collections;

namespace Scripts.Combats.Features
{
    public class Hitstop : MonoBehaviour
    {
        private bool isHitstopActive = false;
        private float hitstopDuration = 0.1f; // Default duration

        public void SetHitstopDuration(float duration)
        {
            hitstopDuration = duration;
        }

        public IEnumerator ApplyHitstop()
        {
            if (isHitstopActive)
                yield break;

            isHitstopActive = true;
            Time.timeScale = 0f; // Pause the game

            yield return new WaitForSecondsRealtime(hitstopDuration); // Wait for the duration in real time

            Time.timeScale = 1f; // Resume the game
            isHitstopActive = false;
        }
    }
}
