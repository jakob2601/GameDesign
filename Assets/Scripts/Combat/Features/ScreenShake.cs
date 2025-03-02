using UnityEngine;
using System.Collections;

namespace Scripts.Combats.Features
{
    public class ScreenShake : MonoBehaviour
    {
        [SerializeField] private float shakeDuration = 0.1f; // Default shake duration
        [SerializeField] private float shakeIntensity = 1.0f; // Default shake intensity
        [SerializeField] private bool isShakeActive = true; // Default to active

        private Transform playerTransform;

        private void Start()
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }

        public void SetShakeActive(bool isActive)
        {
            isShakeActive = isActive;
        }

        public IEnumerator Shake()
        {
            if (!isShakeActive)
                yield break;

            Vector3 originalPosition = playerTransform.position;

            float elapsed = 0.0f;

            while (elapsed < shakeDuration)
            {
                float x = Random.Range(-1f, 1f) * shakeIntensity;
                float y = Random.Range(-1f, 1f) * shakeIntensity;

                transform.position = originalPosition + new Vector3(x, y, 0);

                elapsed += Time.deltaTime;

                yield return null;
            }

            transform.position = originalPosition;
        }
    }

}
