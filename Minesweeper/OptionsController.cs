using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OptionsController : MonoBehaviour
{
    public void SelectLevel()
    {
        // UI 클릭했을때 클릭된 UI의 name 가져옴
        string level = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name;

        switch (level)
        {
            case "Easy":
                if (GameManager.instance != null)
                {
                    GameManager.instance.rows = 8;
                    GameManager.instance.columns = 9;
                    GameManager.instance.cameraX = 3.5f;
                    GameManager.instance.cameraY = 5.5f;
                    GameManager.instance.level = GameManager.Level.EASY;
                }
                break;

            case "Medium":
                if (GameManager.instance != null)
                {
                    GameManager.instance.rows = 8;
                    GameManager.instance.columns = 9;
                    GameManager.instance.cameraX = 4f;
                    GameManager.instance.cameraY = 7f;
                    GameManager.instance.level = GameManager.Level.MEDIUM;
                }
                break;

            case "Hard":
                if (GameManager.instance != null)
                {
                    GameManager.instance.rows = 10;
                    GameManager.instance.columns = 13;
                    GameManager.instance.cameraX = 4.5f;
                    GameManager.instance.cameraY = 7;
                    GameManager.instance.level = GameManager.Level.HARD;
                }
                break;
        }
    }

    public void Back()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
