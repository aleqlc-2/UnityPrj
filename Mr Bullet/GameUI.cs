using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public static GameUI instance;

    private GameManager gameManager;

    private int startBB;

    [Header("WinScreen")]
    public Text goodJobText;
    public GameObject winPanel;
    public Image start1, start2, start3;
    public Sprite shineStar, darkStar;

    [Header("GameOver")]
    public GameObject gameOverPanel;

    void Awake()
    {
        instance = this;
        gameManager = FindObjectOfType<GameManager>();
    }

    void Start()
    {
        startBB = gameManager.blackBullets;
    }

    public void GameOverScreen()
    {
        gameOverPanel.SetActive(true);
    }

    public void WinScreen()
    {
        winPanel.SetActive(true);

        if (gameManager.blackBullets >= startBB) // 첫 발사(금색탄환)로 한방에 맞추면
        {
            goodJobText.text = "FANTASTIC!";
            StartCoroutine(Starts(3));
        }
        else if (gameManager.blackBullets >= startBB - (gameManager.blackBullets / 2)) // 2번째 발사로 맞추면
        {
            goodJobText.text = "AWESOME!";
            StartCoroutine(Starts(2));
        }
        else if (gameManager.blackBullets > 0) // 3번째 발사로 맞추면
        {
            goodJobText.text = "WELL DONE!";
            StartCoroutine(Starts(1));
        }
        else // 4번째 발사로 맞추면
        {
            goodJobText.text = "GOOD";
            StartCoroutine(Starts(0));
        }
    }

    private IEnumerator Starts(int shineNumber)
    {
        yield return new WaitForSeconds(0.5f);
        
        switch (shineNumber)
        {
            case 3:
                yield return new WaitForSeconds(0.15f);
                start1.sprite = shineStar;
                yield return new WaitForSeconds(0.15f);
                start2.sprite = shineStar;
                yield return new WaitForSeconds(0.15f);
                start3.sprite = shineStar;
                break;

            case 2:
                yield return new WaitForSeconds(0.15f);
                start1.sprite = shineStar;
                yield return new WaitForSeconds(0.15f);
                start2.sprite = shineStar;
                start3.sprite = darkStar;
                break;

            case 1:
                yield return new WaitForSeconds(0.15f);
                start1.sprite = shineStar;
                start2.sprite = darkStar;
                start3.sprite = darkStar;
                break;

            case 0:
                start1.sprite = darkStar;
                start2.sprite = darkStar;
                start3.sprite = darkStar;
                break;
        }
    }
}
