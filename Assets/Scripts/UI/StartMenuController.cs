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

        public void OnBackClick()
        {
            SceneManager.LoadScene("StartMenu", LoadSceneMode.Additive);
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
