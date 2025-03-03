using UnityEngine;
using UnityEngine.UI;
using Scripts.Combats.CharacterCombats;
using Scripts.Combats.Weapons;
using System.Collections;
using TMPro;

namespace Scripts.UI
{
    public class WeaponBarController : MonoBehaviour
    {
        [Header("Container")]
        [SerializeField] private Transform weaponBarContainer;  // Parent container for all UI elements
        [SerializeField] private Vector2 containerPosition = new Vector2(10, -110); // Position below hearts

        [Header("Icon Sizes and Spacing")]
        [SerializeField] private Vector2 iconSize = new Vector2(80, 80);
        [SerializeField] private float iconSpacing = 90;
        [SerializeField] private Vector2 textOffset = new Vector2(0, -20);

        [Header("Dash UI")]
        [SerializeField] private Image dashIcon;
        [SerializeField] private Image dashCooldownOverlay;
        [SerializeField] private TextMeshProUGUI dashKeyText;
        [SerializeField] private string dashKeyBinding = "SPACE BAR"; // Default key binding

        [Header("Weapon Icons")]
        [SerializeField] private Image swordIcon;
        [SerializeField] private Image bowIcon;
        [SerializeField] private TextMeshProUGUI swordKeyText;
        [SerializeField] private TextMeshProUGUI bowKeyText;
        [SerializeField] private string swordKeyBinding = "LMB"; // Default key binding for sword
        [SerializeField] private string bowKeyBinding = "RMB"; // Default key binding for bow
        
        [Header("Arrow Type Icons")]
        [SerializeField] private Image normalArrowIcon;
        [SerializeField] private Image piercingArrowIcon;
        [SerializeField] private Image ricochetArrowIcon;
        
        [Header("Icon Properties")]
        [SerializeField] private Color availableColor = Color.white;
        [SerializeField] private Color unavailableColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        [SerializeField] private Color cooldownColor = new Color(0.3f, 0.3f, 0.3f, 0.7f);
        [SerializeField] private Color selectedColor = Color.yellow;

        [Header("Sprites")]
        [SerializeField] private Sprite dashSprite;
        [SerializeField] private Sprite swordSprite;
        [SerializeField] private Sprite bowSprite;
        [SerializeField] private Sprite normalArrowSprite;
        [SerializeField] private Sprite piercingArrowSprite;
        [SerializeField] private Sprite ricochetArrowSprite;

        // Track current weapon and arrow type state
        private bool hasSword = false;
        private bool hasBow = false;
        private Bow.SpecialArrowType currentArrowType = Bow.SpecialArrowType.None;

        // UI elements created dynamically if needed
        private GameObject dashContainer;
        private GameObject weaponIconContainer;
        private GameObject arrowTypeContainer;

        private void Awake()
        {
            // Use assigned container or create one
            if (weaponBarContainer == null)
            {
                // Use this object's transform as the container
                weaponBarContainer = transform;
                Debug.LogWarning("No weapon bar container assigned. Using this GameObject as container.");
            }

            // Create UI elements if they don't exist
            CreateUIElementsIfNeeded();

            // Set key binding texts
            if (dashKeyText != null) dashKeyText.text = dashKeyBinding;
            if (swordKeyText != null) swordKeyText.text = swordKeyBinding;
            if (bowKeyText != null) bowKeyText.text = bowKeyBinding;
        }

        private void CreateUIElementsIfNeeded()
        {
            // Create or find the main container if needed
            if (weaponBarContainer == null)
            {
                weaponBarContainer = transform;
            }

            // Clear existing UI elements if needed
            if (Application.isPlaying)
            {
                // Option to clear existing elements if you want to recreate them
                // ClearExistingElements();
            }

            // Fixed index positions (from left to right)
            int dashIndex = 0;
            int swordIndex = 1;
            int bowIndex = 2;
            int normalArrowIndex = 3;
            int pierceArrowIndex = 4;
            int ricochetArrowIndex = 5;

            // Create dash UI
            if (dashIcon == null)
            {
                dashIcon = CreateFixedPositionImage("DashIcon", dashIndex, dashSprite);
                
                // Create cooldown overlay
                GameObject cooldownObj = new GameObject("CooldownOverlay");
                cooldownObj.transform.SetParent(dashIcon.transform, false);
                dashCooldownOverlay = cooldownObj.AddComponent<Image>();
                dashCooldownOverlay.color = cooldownColor;
                dashCooldownOverlay.fillMethod = Image.FillMethod.Radial360;
                dashCooldownOverlay.type = Image.Type.Filled;
                dashCooldownOverlay.fillAmount = 0;
                
                // Make overlay the same size as the icon
                RectTransform rt = dashCooldownOverlay.rectTransform;
                rt.anchorMin = Vector2.zero;
                rt.anchorMax = Vector2.one;
                rt.offsetMin = Vector2.zero;
                rt.offsetMax = Vector2.zero;
                
                // Create key text
                dashKeyText = CreateKeyText("DashKeyText", dashIcon.transform, dashKeyBinding);
            }

            // Create weapon icons
            if (swordIcon == null)
            {
                swordIcon = CreateFixedPositionImage("SwordIcon", swordIndex, swordSprite);
                swordKeyText = CreateKeyText("SwordKeyText", swordIcon.transform, swordKeyBinding);
            }
            
            if (bowIcon == null)
            {
                bowIcon = CreateFixedPositionImage("BowIcon", bowIndex, bowSprite);
                bowKeyText = CreateKeyText("BowKeyText", bowIcon.transform, bowKeyBinding);
            }

            // Create arrow type icons
            if (normalArrowIcon == null)
                normalArrowIcon = CreateFixedPositionImage("NormalArrowIcon", normalArrowIndex, normalArrowSprite);
            
            if (piercingArrowIcon == null)
                piercingArrowIcon = CreateFixedPositionImage("PiercingArrowIcon", pierceArrowIndex, piercingArrowSprite);
            
            if (ricochetArrowIcon == null)
                ricochetArrowIcon = CreateFixedPositionImage("RicochetArrowIcon", ricochetArrowIndex, ricochetArrowSprite);
            
            // Store references to containers for show/hide management
            arrowTypeContainer = normalArrowIcon.transform.parent.gameObject;
            
            Debug.Log("UI elements created or verified successfully with fixed positioning");
        }

        private Image CreateFixedPositionImage(string name, int positionIndex, Sprite sprite)
        {
            GameObject imageObj = new GameObject(name);
            imageObj.transform.SetParent(weaponBarContainer, false);
            
            Image image = imageObj.AddComponent<Image>();
            image.sprite = sprite;
            image.preserveAspect = true;
            
            // Position using the heart icon style positioning
            RectTransform rectTransform = image.rectTransform;
            rectTransform.anchorMin = new Vector2(0, 1); // Top left
            rectTransform.anchorMax = new Vector2(0, 1); // Top left
            rectTransform.pivot = new Vector2(0, 1);     // Top left as pivot
            rectTransform.sizeDelta = iconSize;          // Size of the icon
            
            // Position horizontally like the hearts are positioned
            rectTransform.anchoredPosition = new Vector2(
                positionIndex * iconSpacing + containerPosition.x, 
                containerPosition.y // Position below hearts
            );
            
            return image;
        }

        private TextMeshProUGUI CreateKeyText(string name, Transform parent, string text)
        {
            GameObject textObj = new GameObject(name);
            textObj.transform.SetParent(parent, false);
            
            TextMeshProUGUI tmpText = textObj.AddComponent<TextMeshProUGUI>();
            tmpText.text = text;
            tmpText.fontSize = 12;
            tmpText.alignment = TextAlignmentOptions.Center;
            
            // Position below the icon
            RectTransform rt = tmpText.rectTransform;
            rt.anchorMin = new Vector2(0.5f, 0);
            rt.anchorMax = new Vector2(0.5f, 0);
            rt.pivot = new Vector2(0.5f, 1);
            rt.sizeDelta = new Vector2(60, 20);
            rt.anchoredPosition = textOffset;
            
            return tmpText;
        }

        // Add a method to toggle the arrow type icons' visibility
        private void SetArrowTypeIconsVisibility(bool visible)
        {
            if (normalArrowIcon != null)
                normalArrowIcon.gameObject.SetActive(visible);
            
            if (piercingArrowIcon != null)
                piercingArrowIcon.gameObject.SetActive(visible);
            
            if (ricochetArrowIcon != null)
                ricochetArrowIcon.gameObject.SetActive(visible);
        }

        private void Start()
        {
            // Initialize with default values
            UpdateWeapons(Combat.WeaponTypes.None);
            UpdateArrowType(Bow.SpecialArrowType.None);
            UpdateDashCooldown(0f, 1f);  // Initially no cooldown
        }

        public void UpdateWeapons(Combat.WeaponTypes availableWeapons)
        {
            hasSword = (availableWeapons & Combat.WeaponTypes.Sword) != 0;
            hasBow = (availableWeapons & Combat.WeaponTypes.Bow) != 0;

            // Update sword icon
            if (swordIcon != null)
            {
                swordIcon.sprite = swordSprite;
                swordIcon.color = hasSword ? availableColor : unavailableColor;
            }

            // Update bow icon
            if (bowIcon != null)
            {
                bowIcon.sprite = bowSprite;
                bowIcon.color = hasBow ? availableColor : unavailableColor;
            }

            // Show/hide arrow type icons based on bow availability
            SetArrowTypeIconsVisibility(hasBow);
            
            Debug.Log($"WeaponBarController updated - Sword: {hasSword}, Bow: {hasBow}");
        }

        public void UpdateDashCooldown(float currentCooldown, float maxCooldown)
        {
            // Update dash icon
            if (dashIcon != null)
            {
                dashIcon.sprite = dashSprite;
                dashIcon.color = availableColor;  // Dash is always available
            }

            // Update cooldown overlay
            if (dashCooldownOverlay != null)
            {
                if (currentCooldown <= 0)
                {
                    dashCooldownOverlay.fillAmount = 0;  // No cooldown
                }
                else
                {
                    float fillAmount = currentCooldown / maxCooldown;
                    dashCooldownOverlay.fillAmount = fillAmount;
                    dashCooldownOverlay.color = cooldownColor;
                }
            }
        }

        public void UpdateArrowType(Bow.SpecialArrowType arrowType)
        {
            currentArrowType = arrowType;
            
            // If we don't have a bow, we don't show arrow types
            if (!hasBow)
                return;

            // Update normal arrow icon
            if (normalArrowIcon != null)
            {
                normalArrowIcon.sprite = normalArrowSprite;
                normalArrowIcon.color = arrowType == Bow.SpecialArrowType.None ? selectedColor : unavailableColor;
            }

            // Update piercing arrow icon
            if (piercingArrowIcon != null)
            {
                piercingArrowIcon.sprite = piercingArrowSprite;
                piercingArrowIcon.color = arrowType == Bow.SpecialArrowType.Pierce ? selectedColor : unavailableColor;
            }

            // Update ricochet arrow icon
            if (ricochetArrowIcon != null)
            {
                ricochetArrowIcon.sprite = ricochetArrowSprite;
                ricochetArrowIcon.color = arrowType == Bow.SpecialArrowType.Ricochet ? selectedColor : unavailableColor;
            }
            
            Debug.Log($"WeaponBarController updated arrow type: {arrowType}");
        }

        public void PlayWeaponAcquiredAnimation(Combat.WeaponTypes weaponType)
        {
            if (weaponType == Combat.WeaponTypes.Sword && swordIcon != null)
            {
                StartCoroutine(PulseIcon(swordIcon));
            }
            else if (weaponType == Combat.WeaponTypes.Bow && bowIcon != null)
            {
                StartCoroutine(PulseIcon(bowIcon));
            }
        }

        public void PlayDashActivatedAnimation()
        {
            if (dashIcon != null)
            {
                StartCoroutine(PulseIcon(dashIcon));
            }
        }

        public void PlayArrowTypeChangedAnimation(Bow.SpecialArrowType arrowType)
        {
            Image targetIcon = null;
            
            switch (arrowType)
            {
                case Bow.SpecialArrowType.None:
                    targetIcon = normalArrowIcon;
                    break;
                case Bow.SpecialArrowType.Pierce:
                    targetIcon = piercingArrowIcon;
                    break;
                case Bow.SpecialArrowType.Ricochet:
                    targetIcon = ricochetArrowIcon;
                    break;
            }
            
            if (targetIcon != null)
            {
                StartCoroutine(PulseIcon(targetIcon));
            }
        }

        private IEnumerator PulseIcon(Image icon)
        {
            // Save original scale and color
            Vector3 originalScale = icon.transform.localScale;
            Color originalColor = icon.color;
            
            // Pulse effect
            float duration = 0.5f;
            float elapsed = 0;
            
            while (elapsed < duration)
            {
                float t = elapsed / duration;
                float scale = 1f + 0.3f * Mathf.Sin(t * Mathf.PI);
                icon.transform.localScale = originalScale * scale;
                
                // Brighten color
                icon.color = Color.Lerp(originalColor, Color.white, Mathf.Sin(t * Mathf.PI));
                
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            // Restore original values
            icon.transform.localScale = originalScale;
            icon.color = originalColor;
        }
    }
}
