using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    private GameObject UI_Hud;

    private Text scoreText;

    private Slider fuelSlider;

    private bool canCountScore;

    private int scoreValue;

    private float scoreCount;
    private float fuelValue = 100f;
    private float fuel_Spend_Treshold = 1f;

    PlayerController playerController;

    void Start()
    {
        UI_Hud = GameObject.Find("HUD");
        scoreText = GameObject.Find("ScoreText").GetComponent<Text>();
        fuelSlider = GameObject.Find("Fuel").GetComponent<Slider>();

        playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();

        UI_Hud.SetActive(false);
    }

    void Update()
    {
        ScoreAndFuel();
    }

    public void ActivateHUD(bool Active)
    {
        canCountScore = true;
        UI_Hud.SetActive(Active);
    }

    private void ScoreAndFuel()
    {
        if (canCountScore)
        {
            // Slider 컴포넌트의 value가 0~1이므로 연료가 1에서 점점 떨어지도록
            fuelValue -= fuel_Spend_Treshold * Time.deltaTime;
            fuelSlider.value = fuelValue / 100f;

            scoreCount += 5f * Time.deltaTime;
            scoreValue = (int)scoreCount;
            scoreText.text = "Score: " + scoreValue;

            if (fuelValue <= 0f)
            {
                playerController.PlayerCrashed();
                canCountScore = false;
            }
        }
    }

    public void FuelCollected()
    {
        fuelValue += 30f;

        if (fuelValue > 100) fuelValue = 100;
    }

    public void IncreaseScore()
    {
        scoreCount += 50;
    }
}
