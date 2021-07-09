using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Text textScore;
    [SerializeField] private Text textBest;

    void Update()
    {
        textBest.text = "Best: " + GameManager.singleton.best;
        textScore.text = "Score: " + GameManager.singleton.score;
    }
}
