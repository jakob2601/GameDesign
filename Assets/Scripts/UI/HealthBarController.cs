using UnityEngine;
using UnityEngine.UI;

namespace Scripts.UI
{
    public class HealthBarController : MonoBehaviour
    {
        public GameObject heartContainer; // The parent GameObject that contains the hearts
        public Sprite fullHeart; // Sprite for full heart
        public Sprite emptyHeart; // Sprite for empty heart

        private Image[] heartImages; // Array for the UI images of the hearts

        public void InitializeHearts(int maxHealth)
        {
            // Delete old hearts
            foreach (Transform child in heartContainer.transform)
            {
                Destroy(child.gameObject);
            }

            // Create new hearts based on maxHealth
            heartImages = new Image[maxHealth];
            for (int i = 0; i < maxHealth; i++)
            {
                // Create a new heart
                GameObject newHeart = new GameObject("Heart_" + i, typeof(Image));
                newHeart.transform.SetParent(heartContainer.transform);

                // Add an Image component
                Image heartImage = newHeart.GetComponent<Image>();
                heartImage.sprite = emptyHeart; // Default: empty heart
                heartImages[i] = heartImage;

                // RectTransform settings
                RectTransform rectTransform = newHeart.GetComponent<RectTransform>();
                rectTransform.anchorMin = new Vector2(0, 1); // Top left
                rectTransform.anchorMax = new Vector2(0, 1); // Top left
                rectTransform.pivot = new Vector2(0, 1);     // Top left as pivot

                rectTransform.sizeDelta = new Vector2(100, 100); // Size of the heart icon
                rectTransform.anchoredPosition = new Vector2(i * 110 + 10, 0); // Horizontal spacing of hearts
            }
        }

        public void UpdateHearts(int currentHealth, int maxHealth)
        {
            for (int i = 0; i < heartImages.Length; i++)
            {
                if (i < currentHealth)
                {
                    heartImages[i].sprite = fullHeart; // Full heart
                }
                else
                {
                    heartImages[i].sprite = emptyHeart; // Empty heart
                }
            }
        }
    }
}