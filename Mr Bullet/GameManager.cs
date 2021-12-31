using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int enemyCount = 1;
    public int blackBullets = 3;
    public int goldenBullets = 1;

    [HideInInspector] public bool gameOver;

    public GameObject blackBullet, goldenBullet;

    private int levelNumber;

    void Awake()
    {
        //PlayerPrefs.DeleteAll();
        //levelNumber = PlayerPrefs.GetInt("Level", 1);
        //print(PlayerPrefs.GetInt("Level", 1));

        FindObjectOfType<PlayerController>().ammo = blackBullets + goldenBullets;

        for (int i = 0; i < blackBullets; i++)
        {
            GameObject bbTemp = Instantiate(blackBullet);
            bbTemp.transform.SetParent(GameObject.Find("Bullets").transform);
        }

        for (int i = 0; i < goldenBullets; i++)
        {
            GameObject gbTemp = Instantiate(goldenBullet);
            gbTemp.transform.SetParent(GameObject.Find("Bullets").transform);
        }
    }

    void Update()
    {
        // 적이 살아있지만 총알 다 떨어지면 게임 lose
        if (!gameOver && FindObjectOfType<PlayerController>().ammo <= 0 && enemyCount > 0
            && GameObject.FindGameObjectsWithTag("Bullet").Length <= 0) // 마지막 총알이 발사되자마자 게임오버가 아니라 Destroy되면 게임오버되도록
        {
            gameOver = true;
            GameUI.instance.GameOverScreen();
        }
    }

    public void CheckBullets()
    {
        if (goldenBullets > 0)
        {
            goldenBullets--;
            GameObject.FindGameObjectWithTag("GoldenBullet").SetActive(false);
        }
        else if (blackBullets > 0)
        {
            blackBullets--;
            GameObject.FindGameObjectWithTag("BlackBullet").SetActive(false);
        }
    }

    public void CheckEnemyCount()
    {
        enemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;

        // 적을 다 죽이면 게임 win
        if (enemyCount <= 0)
        {
            GameUI.instance.WinScreen();

            //// 현재가 레벨1이면 2로 미리설정
            //if (levelNumber >= SceneManager.GetActiveScene().buildIndex)
            //{
            //    PlayerPrefs.SetInt("Level", levelNumber + 1);
            //    print(PlayerPrefs.GetInt("Level", 1));
            //}
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void NextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Exit()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
