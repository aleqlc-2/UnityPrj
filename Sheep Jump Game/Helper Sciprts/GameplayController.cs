using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameplayController : MonoBehaviour
{
    public static GameplayController instance;

    [SerializeField] private Text scoreText;

    private int score;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void IncrementScore()
    {
        score++;
        scoreText.text = "x" + score;
    }

    public void RestartGame()
    {
        Invoke("ReloadGame", 1.5f);
    }

    void ReloadGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Gameplay");
    }
}
