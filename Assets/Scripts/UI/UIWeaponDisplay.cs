using UnityEngine;
using UnityEngine.SceneManagement;
using Scripts.Combats.CharacterCombats;
using Scripts.Combats.Weapons;
using Scripts.Movements.Moves;

namespace Scripts.UI
{
    public class UIWeaponDisplay : MonoBehaviour
    {
        private PlayerCombat playerCombat;
        private WeaponBarController weaponBarController;
        private Bow playerBow;
        private Dash playerDash;

        void Start()
        {
            weaponBarController = GetComponent<WeaponBarController>();
            SceneManager.sceneLoaded += OnSceneLoaded;
            FindAndConnectPlayer();
        }

        void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            
            if (playerCombat != null)
            {
                // Unsubscribe from events if we created any
            }
        }

        void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
        {
            Debug.Log("Scene loaded: " + scene.name);
            FindAndConnectPlayer();
        }

        public void SetPlayer(GameObject player)
        {
            // Find player combat component
            playerCombat = player.GetComponentInChildren<PlayerCombat>();
            playerDash = player.GetComponentInChildren<Dash>();
            
            if (playerCombat != null && weaponBarController != null)
            {
                Debug.Log("Player found for weapon display: " + player.name);
                
                // Find bow for arrow type updates
                playerBow = player.GetComponentInChildren<Bow>(true); // Include inactive
                
                if (playerDash == null)
                {
                    Debug.LogWarning("Player Dash component not found");
                }
                
                // Initial update
                UpdateWeaponUI();
            }
            else
            {
                Debug.LogWarning("PlayerCombat or WeaponBarController is null");
            }
        }

        private void UpdateWeaponUI()
        {
            if (playerCombat != null && weaponBarController != null)
            {
                // Update available weapons
                weaponBarController.UpdateWeapons(playerCombat.GetAvailableWeapons());
                
                // Update arrow type if bow exists
                if (playerBow != null)
                {
                    weaponBarController.UpdateArrowType(playerBow.currentSpecialArrowType);
                }
                
                // Update dash cooldown if dash exists
                if (playerDash != null)
                {
                    float currentCooldown = playerDash.GetCurrentDashCooldown();
                    float maxCooldown = playerDash.GetDashCooldown();
                    weaponBarController.UpdateDashCooldown(currentCooldown, maxCooldown);
                    
                    // If dash was just used, play animation
                    if (currentCooldown > 0 && Time.frameCount % 60 == 0)  // Check once per second
                    {
                        if (currentCooldown <= 0.1f)
                        {
                            weaponBarController.PlayDashActivatedAnimation();
                        }
                    }
                }
            }
        }

        void FindAndConnectPlayer()
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                SetPlayer(playerObject);
            }
            else
            {
                Debug.LogWarning("Player object not found for weapon display");
            }
        }

        void Update()
        {
            if (playerCombat != null)
            {
                UpdateWeaponUI();
            }
        }
    }
}
