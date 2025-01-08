using UnityEngine;
using UnityEngine.UI;

namespace Scripts.UI
{
    public class HealthBarController : MonoBehaviour
    {
        public GameObject heartContainer; // Das Parent-GameObject, das die Herzen enthält
        public Sprite fullHeart; // Sprite für volles Herz (Heart_0)
        public Sprite emptyHeart; // Sprite für leeres Herz (Heart_4)

        private Image[] heartImages; // Array für die UI-Images der Herzen

        public void InitializeHearts(int maxHealth)
        {
            // Lösche alte Herzen
            foreach (Transform child in heartContainer.transform)
            {
                Destroy(child.gameObject);
            }

            // Erstelle neue Herzen basierend auf maxHealth
            heartImages = new Image[maxHealth];
            for (int i = 0; i < maxHealth; i++)
            {
                // Neues Herz erstellen
                GameObject newHeart = new GameObject("Heart_" + i, typeof(Image));
                newHeart.transform.SetParent(heartContainer.transform);

                // Füge ein Image hinzu
                Image heartImage = newHeart.GetComponent<Image>();
                heartImage.sprite = emptyHeart; // Standard: leeres Herz
                heartImages[i] = heartImage;

                // RectTransform-Einstellungen
                RectTransform rectTransform = newHeart.GetComponent<RectTransform>();
                rectTransform.anchorMin = new Vector2(0, 1); // Oben links
                rectTransform.anchorMax = new Vector2(0, 1); // Oben links
                rectTransform.pivot = new Vector2(0, 1);     // Oben links als Drehpunkt

                rectTransform.sizeDelta = new Vector2(100, 100); // Größe des Herz-Icons
                rectTransform.anchoredPosition = new Vector2(i * 110 + 10, 0); // Abstand der Herzen (horizontal)
            }

        }

        public void UpdateHearts(int currentHealth, int maxHealth)
        {
            for (int i = 0; i < maxHealth; i++)
            {
                if (i < currentHealth)
                {
                    heartImages[i].sprite = fullHeart; // Volles Herz
                }
                else
                {
                    heartImages[i].sprite = emptyHeart; // Leeres Herz
                    
                }
            }
        }
    }

}
