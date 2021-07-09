using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModeScore : MonoBehaviour
{
    public int ModeSelection;
    public GameObject RaceUI;
    public GameObject ScoreUI;
    public GameObject AICar;
    public static int CurrentScore;
    public int InternalScore;
    public GameObject ScoreValue;
    public GameObject ScoreObjects;

    void Start()
    {
        ModeSelection = ModeSelect.RaceMode;

        if(ModeSelection == 1)
        {
            ScoreUI.SetActive(true);
            ScoreObjects.SetActive(true);
            RaceUI.SetActive(false);
            AICar.SetActive(false);
        }
    }

    void Update()
    {
        InternalScore = CurrentScore;
        ScoreValue.GetComponent<Text>().text = "" + InternalScore;
    }
}
