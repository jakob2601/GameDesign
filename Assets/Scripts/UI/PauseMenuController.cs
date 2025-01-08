using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyGame
{
    public class PauseMenuController : MonoBehaviour
    {
        [SerializeField] GameObject pausMenu;
        private bool isPaused = false; // Zum Überprüfen, ob das Spiel pausiert ist

        void Update()
        {
            // Überprüfen, ob die Escape-Taste gedrückt wurde
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (isPaused)
                {
                    Resume();
                }
                else
                {
                    Pause();
                }
            }
        }


        public void Pause()
        {
            pausMenu.SetActive(true);
            Time.timeScale = 0;
            isPaused = true;
        }

        public void Menu()
        {
            Time.timeScale = 1;
            SceneManager.LoadScene("StartMenu");
        }

        public void Resume()
        {
            pausMenu.SetActive(false);
            Time.timeScale = 1;
            isPaused = false;
        }

        public void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }

        public void Restart()
        {
            Time.timeScale = 1;
            SceneManager.LoadScene("SampleScene");
        }


    }  
}
