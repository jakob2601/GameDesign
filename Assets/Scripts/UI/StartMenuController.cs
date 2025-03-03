using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyGame
{
    public class StartMenuController : MonoBehaviour
    {
        private bool isPaused = false;
        public void OnStartClick()
        {
            isPaused = false;
            SceneManager.LoadScene("Arena lvl1");
        }

        public void OnBackClick()
        {
            SceneManager.LoadScene("StartMenu");
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
