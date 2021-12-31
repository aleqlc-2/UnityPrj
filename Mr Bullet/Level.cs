using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Level : MonoBehaviour
{
    private Button levelBtn;

    public int levelReq;

    void Start()
    {
        levelBtn = GetComponent<Button>();
        levelBtn.onClick.AddListener(() => LoadLevel());

        //if (PlayerPrefs.GetInt("Level") >= levelReq)
        //{
        //    levelBtn.onClick.AddListener(() => LoadLevel());
        //}
        //else
        //{
        //    GetComponent<CanvasGroup>().alpha = 1f;
        //}
    }

    public void LoadLevel()
    {
        SceneManager.LoadScene(gameObject.name);
    }
}
