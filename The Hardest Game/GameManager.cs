using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Vector2 playerInitialPosition;

    private GameObject[] enemies;
    private GameObject player;

    void Awake()
    {
        if (instance == null)
            instance = this;

        Time.timeScale = 1f; // 없어도 될듯
    }

    void Start()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy"); // s
        player = GameObject.FindWithTag("Player");
    }

    public void PlayerReachedGoal()
    {
        player.transform.position = playerInitialPosition;
        player.GetComponent<PlayerScript>().moveSpeed += 0.3f;

        // for(int i = 0; i < enemies.Length; i++)
        // {
        //    enemies[i].GetComponent<EnemyScript>().moveSpeed += 1f;
        // } //밑에 코드랑 같음

        foreach (GameObject g in enemies)
        {
            g.GetComponent<EnemyScript>().moveSpeed += 1f;
        }
    }

    public void PlayerDied()
    {
        Time.timeScale = 0f; // 모두 멈춤
        StartCoroutine(RestartGame());
    }
    
    IEnumerator RestartGame()
    {
        yield return new WaitForSecondsRealtime(2f); // 2초 후, Realtime
        UnityEngine.SceneManagement.SceneManager.LoadScene
            (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
    
}
