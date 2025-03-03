using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyGame
{
    public class StartMenuController : MonoBehaviour
    {
        public void OnStartClick()
        {
            SceneManager.LoadScene("Arena lvl1");
        }

        private void Awake()
        {
            // Dieses Objekt beim Szenenwechsel nicht zerstören
            DontDestroyOnLoad(gameObject);
        }

        public void OnBackClick()
        {
            // Überprüfen, ob die Szene bereits geladen ist, um doppeltes Laden zu vermeiden
            if (SceneManager.GetSceneByName("StartMenu").isLoaded)
            {
                Debug.Log("StartMenu ist bereits geladen!");
                return;
            }

            // Überprüfen, ob die Szene existiert
            if (Application.CanStreamedLevelBeLoaded("StartMenu"))
            {
                SceneManager.LoadScene("StartMenu", LoadSceneMode.Additive);
            }
            else
            {
                Debug.LogError("Szene 'StartMenu' existiert nicht in den Build Settings!");
            }
        }

        public void OnResumeClick()
        {
            // Entfernt das Startmenü, wenn der Spieler zurück ins Spiel möchte
            Scene scene = SceneManager.GetSceneByName("StartMenu");
            if (scene.isLoaded)
            {
                SceneManager.UnloadSceneAsync("StartMenu");
            }
        }




        public void OnControlClick()
        {
            SceneManager.LoadScene("ControlsMenu");
        }

        public void OnExitClick()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            Application.Quit();  

        }
    }
}
