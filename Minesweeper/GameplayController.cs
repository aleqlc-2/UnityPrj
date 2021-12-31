using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayController : MonoBehaviour
{
    public void RestartGame()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.minesCount = 0;
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GobackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
