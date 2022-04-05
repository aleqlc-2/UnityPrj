using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour
{
    public static LoadingScreen instance;

    public GameObject loading_Screen;

    public Image loading_Bar_Progress;

    private float progress_Value = 1.1f;
    private float progress_Multiplier_1 = 0.5f;
    private float progress_Multiplier_2 = 0.07f;

    public bool loadMainMenuFirstTime;

    void Awake()
    {
        MakeSingleton();
    }

    void Update()
    {
        ShowLoadingScreen();
    }

    private void MakeSingleton()
    {
        if (instance != null)
            Destroy(gameObject);
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void LoadLevel(string levelName)
    {
        loading_Screen.SetActive(true);
        progress_Value = 0f;
        Time.timeScale = 0f;
        SceneManager.LoadScene(levelName);
    }

    private void ShowLoadingScreen()
    {
        if (progress_Value < 1f)
        {
            progress_Value += progress_Multiplier_1 * progress_Multiplier_2;
            loading_Bar_Progress.fillAmount = progress_Value;

            if (progress_Value >= 1f)
            {
                progress_Value = 1.1f;
                loading_Bar_Progress.fillAmount = 0f;
                loading_Screen.SetActive(false);
                Time.timeScale = 1f;
            }
        }
    }

    public void LoadLevelAsync(string levelName)
    {
        StartCoroutine(LoadAsynchronously(levelName));
    }

    private IEnumerator LoadAsynchronously(string levelName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(levelName);
        loading_Screen.SetActive(true);

        while (!operation.isDone)
        {
            float progress = operation.progress / 0.9f;
            loading_Bar_Progress.fillAmount = progress;

            if (progress >= 1f) loading_Screen.SetActive(false);

            yield return null;
        }
    }
}
