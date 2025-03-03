using UnityEngine;
using UnityEngine.SceneManagement;
using Scripts.Healths;

namespace Scripts.UI
{
    public class UIHealthDisplay : MonoBehaviour
    {
        private PlayerHealth playerHealth;
        private HealthBarController healthBarController;

        void Start()
        {
            healthBarController = GetComponent<HealthBarController>();
            SceneManager.sceneLoaded += OnSceneLoaded;
            FindAndConnectPlayer();
        }

        void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
        {
            Debug.Log("Scene loaded: " + scene.name);
            FindAndConnectPlayer();
        }

        public void SetPlayer(GameObject player)
        {
            playerHealth = player.GetComponentInChildren<PlayerHealth>();
            if (playerHealth != null && healthBarController != null)
            {
                Debug.Log("Player found: " + player.name);
                healthBarController.InitializeHearts(playerHealth.maxHealth);
                UpdateHealthUI();
            }
            else
            {
                Debug.LogWarning("PlayerHealth or HealthBarController is null");
            }
        }

        private void UpdateHealthUI()
        {
            if (playerHealth != null && healthBarController != null)
            {
                healthBarController.UpdateHearts(playerHealth.currentHealth, playerHealth.maxHealth);
                // Debug.Log("Health bar updated: " + playerHealth.currentHealth + "/" + playerHealth.maxHealth);
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
                Debug.LogWarning("Player object not found");
            }
        }

        void Update()
        {
            if (playerHealth != null)
            {
                UpdateHealthUI();
            }
        }
    }
}