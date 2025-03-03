using UnityEngine;
using Scripts.Combats.CharacterCombats;
using Scripts.Combats.Weapons;
using Scripts.Healths;
using Scripts.Movements.Moves;
using UnityEngine.SceneManagement;
using System.Collections;
using Scripts.UI;

namespace Scripts.Scene
{
    public class PlayerPersistence : MonoBehaviour
    {
        [Header("Player Components")]
        private PlayerHealth playerHealth;
        private Combat playerCombat;

        [Header("Persistent Upgrades")]
        // Weapons availability
        public bool hasSword = false;
        public bool hasBow = false;

        // Weapon stats
        public int swordDamage = 1;
        public int bowDamage = 1;

        // Special arrow types
        public bool hasPiercingArrows = false;
        public bool hasRicochetArrows = false;

        // Movement Upgrades
        public float moveSpeed = 6.5f;

        public static PlayerPersistence Instance { get; private set; }


        private void Awake()
        {
           // And update the usage
               if (Instance != null && Instance != this)
               {
                   Destroy(gameObject);
                   return;
               }

               Instance = this;
            DontDestroyOnLoad(gameObject);

            // Register for scene loading events
            SceneManager.sceneLoaded += OnSceneLoaded;

            // Get references
            playerHealth = GetComponentInChildren<PlayerHealth>();
            playerCombat = GetComponentInChildren<Combat>();
        }
        private void OnDestroy()
                {
                    // Unregister when destroyed
                    SceneManager.sceneLoaded -= OnSceneLoaded;
                }
        // Add this method to transfer data to a new player
                public void TransferDataToNewPlayer(PlayerPersistence newPlayer)
                {
                    newPlayer.hasSword = this.hasSword;
                    newPlayer.hasBow = this.hasBow;
                    newPlayer.swordDamage = this.swordDamage;
                    newPlayer.bowDamage = this.bowDamage;
                    newPlayer.hasPiercingArrows = this.hasPiercingArrows;
                    newPlayer.hasRicochetArrows = this.hasRicochetArrows;
                    newPlayer.moveSpeed = this.moveSpeed;

                    // Have the new player apply the upgrades
                    newPlayer.ApplyPersistentUpgrades();
                }
private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
{
    Debug.Log("Scene loaded: " + scene.name);

    // Restore player health to maximum when entering a new scene
    if (playerHealth != null)
    {
        playerHealth.currentHealth = playerHealth.maxHealth;
        Debug.Log($"Player health restored to max: {playerHealth.maxHealth}");
    }

    // Wait a frame to let other objects initialize
    StartCoroutine(FindPlayerNextFrame());
}

       private IEnumerator FindPlayerNextFrame()
       {
           yield return null; // Wait one frame

           // Find all PlayerPersistence objects in the scene
           PlayerPersistence[] players = FindObjectsOfType<PlayerPersistence>();
           bool foundNewPlayer = false;

           foreach (PlayerPersistence player in players)
           {
               if (player != this)
               {
                   Debug.Log("Found new PlayerPersistence, transferring data...");
                   TransferDataToNewPlayer(player);
                   foundNewPlayer = true;
                   Destroy(gameObject); // Destroy old player after transfer
                   break;
               }
           }

           // If we didn't find a new player (meaning we're the persistent player)
           if (!foundNewPlayer)
           {
               // Update all UI elements to reference this player
               UpdateUIReferences();
               Debug.Log("Updated UI references to persistent player");
           }
       }

        private void Start()
        {
            ApplyPersistentUpgrades();
        }


        // Apply all saved upgrades when loading a scene
        public void ApplyPersistentUpgrades()
        {
            // Apply health
            if (playerHealth != null)
            {
                playerHealth.currentHealth = playerHealth.maxHealth;
                Debug.Log($"Player health reset to max: {playerHealth.maxHealth}");
            }

            // Apply movement upgrades
            Walking walking = GetComponentInChildren<Walking>(true);
            if (walking != null)
            {
                walking.SetMoveSpeed(moveSpeed);
                Debug.Log($"Applied saved move speed: {moveSpeed}");
            }

            // Apply combat upgrades
            if (playerCombat != null)
            {
                // Apply weapon availability
                playerCombat.SetWeaponAvailable(Combat.WeaponTypes.Sword, hasSword);
                playerCombat.SetWeaponAvailable(Combat.WeaponTypes.Bow, hasBow);

                // Apply sword upgrades
                Sword sword = GetComponentInChildren<Sword>(true);
                if (sword != null)
                {
                    sword.SetAttackDamage(swordDamage);
                    Debug.Log($"Applied saved sword damage: {swordDamage}");
                }

                // Apply bow upgrades
                Bow bow = GetComponentInChildren<Bow>(true);
                if (bow != null)
                {
                    bow.SetAttackDamage(bowDamage);
                    Debug.Log($"Applied saved bow damage: {bowDamage}");

             // Apply special arrow types
             if (hasPiercingArrows)
             {
                 bow.currentSpecialArrowType = Bow.SpecialArrowType.Pierce;
                 Debug.Log("Applied piercing arrows upgrade");
             }
             else if (hasRicochetArrows)
             {
                 bow.currentSpecialArrowType = Bow.SpecialArrowType.Ricochet;
                 Debug.Log("Applied ricochet arrows upgrade");
             }

            }
        }
        }
        public static void UpdateUIReferences()
         {
             var healthDisplays = FindObjectsOfType<UIHealthDisplay>();
             foreach (var display in healthDisplays)
             {
                 display.SetPlayer(Instance.gameObject);
             }
         }

        // Movement speed upgrade
        public void UpdateMoveSpeed(float newSpeed)
        {
            moveSpeed = newSpeed;
            Debug.Log($"Move speed saved to persistence: {moveSpeed}");
        }

        // Sword upgrades
        public void EnableSword(bool enabled)
        {
            hasSword = enabled;

            if (playerCombat != null)
            {
                playerCombat.SetWeaponAvailable(Combat.WeaponTypes.Sword, enabled);
                Debug.Log($"Sword {(enabled ? "enabled" : "disabled")}");
            }
        }

        public void UpgradeSword(int newDamage)
        {
            swordDamage = newDamage;

            Sword sword = GetComponentInChildren<Sword>(true);
            if (sword != null)
            {
                sword.SetAttackDamage(swordDamage);
                Debug.Log($"Sword damage saved to persistence: {swordDamage}");
            }
        }

        // Bow upgrades
        public void EnableBow(bool enabled)
        {
            hasBow = enabled;

            if (playerCombat != null)
            {
                playerCombat.SetWeaponAvailable(Combat.WeaponTypes.Bow, enabled);
                Debug.Log($"Bow {(enabled ? "enabled" : "disabled")}");
            }
        }

        public void UpgradeBowDamage(int newDamage)
        {
            bowDamage = newDamage;

            Bow bow = GetComponentInChildren<Bow>(true);
            if (bow != null)
            {
                bow.SetAttackDamage(bowDamage);
                Debug.Log($"Bow damage saved to persistence: {bowDamage}");
            }
        }

   public void EnablePiercing(bool enabled)
   {
       hasPiercingArrows = enabled;

       Bow bow = GetComponentInChildren<Bow>(true);
       if (bow != null)
       {
           if (enabled)
           {
               bow.currentSpecialArrowType = Bow.SpecialArrowType.Pierce;
           }
           else if (hasRicochetArrows)
           {
               bow.currentSpecialArrowType = Bow.SpecialArrowType.Ricochet;
           }
           else
           {
               bow.currentSpecialArrowType = Bow.SpecialArrowType.None; // Changed from Normal to None
           }
           Debug.Log($"Piercing arrows {(enabled ? "enabled" : "disabled")}");
       }
   }

   public void EnableRicochet(bool enabled)
   {
       hasRicochetArrows = enabled;

       Bow bow = GetComponentInChildren<Bow>(true);
       if (bow != null)
       {
           if (enabled)
           {
               bow.currentSpecialArrowType = Bow.SpecialArrowType.Ricochet;
           }
           else if (hasPiercingArrows)
           {
               bow.currentSpecialArrowType = Bow.SpecialArrowType.Pierce;
           }
           else
           {
               bow.currentSpecialArrowType = Bow.SpecialArrowType.None; // Changed from Normal to None
           }
           Debug.Log($"Ricochet arrows {(enabled ? "enabled" : "disabled")}");
       }
   }

       public void AddExtraHeart(int healthIncrease)
       {
           if (playerHealth != null)
           {
               // Use the IncreaseMaxHealth method from PlayerHealth
               playerHealth.IncreaseMaxHealth(healthIncrease);

               // Don't manually adjust health values here as that would duplicate the effect
               // We just need to log the change
               Debug.Log($"Extra heart added. New max health: {playerHealth.maxHealth}");
           }
       }

    }
}