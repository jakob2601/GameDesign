using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



namespace MyGame
{
    public class main_menu : MonoBehaviour
    {
      public void PlayGame()
      {
      SceneManager.LoadScene("Arena lvl1"); // startet unsere aktuelle arbeitsfläche

      }
      public void Settings()
            {
            SceneManager.LoadScene("SampleScene"); // aktueller Platzhalter
            }
      public void QuitGame()
            {
          Application.Quit();

            }
    }

}
