using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    private MainCamera mainCam;

    private void Start()
    {
        mainCam = GameObject.Find("Main Camera").GetComponent<MainCamera>();
    }

    public void PlayGame()
    {
        mainCam.gameStarted = true;
    }

    public void QuitGame()
    {
        print("QUIT GAME");

        // Application.Quit();
    }
}
