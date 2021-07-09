using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public float slowness = 10f;
    public void EndGame()
    {
        StartCoroutine(RestartLevel());
    }

    IEnumerator RestartLevel()
    {
        Time.timeScale = 1f / slowness; // 10배 느려짐
        Time.fixedDeltaTime = Time.fixedDeltaTime / slowness; // 10배 느려짐

        yield return new WaitForSeconds(1f / slowness); // 0.1초 기다림

        Time.timeScale = 1f; // 다시 정상화
        Time.fixedDeltaTime = Time.fixedDeltaTime * slowness; // 다시 정상화

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
