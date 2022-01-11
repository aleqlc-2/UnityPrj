using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using System.Text; // StringBuilder

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager instance;

    private Text coinTxt, timerTxt;

    private int coinCount;

    [SerializeField] private float timerTreshold = 150f; // 시간 한계점
    private float timerCount;

    private StringBuilder coinString = new StringBuilder(), timerString = new StringBuilder();

    private bool gameOver;

    [SerializeField] private GameObject gameOverPanel;

    void Awake()
    {
        instance = this;
        coinTxt = GameObject.Find("Coin Count").GetComponent<Text>();
        timerTxt = GameObject.Find("Timer Count").GetComponent<Text>();
        gameOverPanel.SetActive(false);

        timerCount = Time.time + 1f;
        timerTxt.text = "Time: " + timerTreshold;
    }

    void Update()
    {
        if (gameOver) return;

        CountTimer();

        // 시간 다지나거나 코인 다먹으면 게임종료
        if (timerTreshold == 0f || coinCount == 0)
        {
            GameOver();
        }
    }

    public void SetCoinCount(int coinValue)
    {
        coinCount += coinValue;

        coinString.Length = 0;
        coinString.Append("Coins: ");
        coinString.Append(coinCount.ToString());

        coinTxt.text = coinString.ToString();
    }

    private void CountTimer()
    {
        if (Time.time > timerCount)
        {
            timerCount = Time.time + 1f;
            timerTreshold--;

            timerString.Length = 0;
            timerString.Append("Time: ");
            timerString.Append(timerTreshold.ToString());

            timerTxt.text = timerString.ToString();
        }
    }

    public void GameOver()
    {
        gameOver = true;

        GameObject.FindWithTag(TagManager.PLAYER_TAG).GetComponent<BallController>().DestroyPlayer();

        Invoke("ShowGameOverPanel", 1f);
    }

    private void ShowGameOverPanel()
    {
        gameOverPanel.SetActive(true);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(1);
    }

    public void NextLevel()
    {
        SceneManager.LoadScene(0);
    }
}
