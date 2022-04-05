using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndGameManager : MonoBehaviour
{
    public static EndGameManager instance;

    public Image endGamePanel;

    public Sprite you_Win_Sprite, you_Lose_Sprite;

    private AudioSource audioSource;
    public AudioClip you_Win_Audio, you_Lose_Audio;

    void Start()
    {
        MakeInstance();

        audioSource = GetComponent<AudioSource>();
    }

    private void MakeInstance()
    {
        if (instance == null) instance = this;
    }

    public void GameOver(bool win)
    {
        StartCoroutine(RestartGame());

        endGamePanel.gameObject.SetActive(true);

        if (win)
        {
            endGamePanel.sprite = you_Win_Sprite;
            audioSource.clip = you_Win_Audio;
        }
        else
        {
            endGamePanel.sprite = you_Lose_Sprite;
            audioSource.clip = you_Lose_Audio;
            DeactivateAllEnemyScripts();
        }

        audioSource.Play();
    }

    private IEnumerator RestartGame()
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(6f);
        Time.timeScale = 1f;

        LoadingScreen.instance.LoadLevel(SceneNames.MAIN_MENU);
    }

    private void DeactivateAllEnemyScripts()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(TagManager.ENEMY_TAG);

        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].GetComponent<EnemyController>().enabled = false;
            enemies[i].GetComponent<CharacterAnimation>().Walk(false);
        }

        GameObject boss = GameObject.FindGameObjectWithTag(TagManager.BOSS_TAG);

        if (boss != null)
        {
            boss.GetComponent<BossController>().enabled = false;
            boss.GetComponent<CharacterAnimation>().Walk(false);
        }
    }
}
