using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Button3Gaze : MonoBehaviour
{
    bool isTimerOn = false;
    float delta = 0;
    float span = 2f;
    GameObject player;
    GameObject timer;

    private void Start()
    {
        player = GameObject.Find("Player");
        timer = GameObject.Find("ImageTimer");
    }

    void Update()
    {
        if (isTimerOn == true)
        {
            delta += Time.deltaTime;
            timer.GetComponent<Image>().fillAmount = delta / span;

            if (delta > span)
            {
                // 버튼을 2초동안 응시하면 GotoS2()호출하여 이동
                player.GetComponent<PlayerControl>().GotoS1();
            }
        }
    }

    public void GazeStart()
    {
        isTimerOn = true;
    }

    public void GazeEnd()
    {
        isTimerOn = false;
        delta = 0;
        timer.GetComponent<Image>().fillAmount = 0;
    }
}
