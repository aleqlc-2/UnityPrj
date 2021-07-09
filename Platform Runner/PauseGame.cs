using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseGame : MonoBehaviour
{
    public bool gamePaused = false;
    public AudioSource levelMusic;
    public GameObject pauseMenu;

    void Update()
    {
        if (Input.GetButtonDown("Cancel")) // esc
        {
            if (gamePaused == false) // 일시정지
            {
                Time.timeScale = 0;
                gamePaused = true;
                Cursor.visible = true;
                levelMusic.Pause();
                pauseMenu.SetActive(true);
            }
            else
            {
                pauseMenu.SetActive(false);
                levelMusic.UnPause();
                Cursor.visible = false;
                gamePaused = false;
                Time.timeScale = 1;
            }
        }
    }

    public void ResumeGame() // 게임 재개
    {
        levelMusic.UnPause();
        Cursor.visible = false;
        gamePaused = false;
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
    }

    public void RestartLevel() // 게임 재시작
    {
        pauseMenu.SetActive(false);
        levelMusic.UnPause();
        Cursor.visible = false;
        gamePaused = false;
        Time.timeScale = 1;

        SceneManager.LoadScene(2);
    }

    public void QuitToMenu() // 메인메뉴로 이동
    {
        pauseMenu.SetActive(false);
        levelMusic.UnPause();
        Cursor.visible = true;
        gamePaused = false;
        Time.timeScale = 1;

        SceneManager.LoadScene(1);
    }
}
