using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSceneScript : MonoBehaviour
{
    public static GameSceneScript instance;

    public Transform refPopUpText;

    public Text currentScoreDisplayText;

    private int totalScore;

    private void Awake()
    {
        instance = this;
    }

    public void SceneCall(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    public void InitPopUpText(Vector3 initPos, int value, bool isScore = true)
    {
        GameObject text = Instantiate(
            refPopUpText, initPos + new Vector3(0f, 1f, 0f), Quaternion.identity).gameObject;

        string str = "";

        if (isScore)
        {
            str += "+"; // score
            totalScore += 100;
            AddScore();
        }
        else
        {
            str += "$"; // money
        }

        str += value;

        text.GetComponent<TextMesh>().text = str;

        Destroy(text, 2f);
    }

    public void AddScore()
    {
        totalScore = ComboController.instance.AddCombo(totalScore, 100);
        currentScoreDisplayText.text = totalScore.ToString();
    }
}
