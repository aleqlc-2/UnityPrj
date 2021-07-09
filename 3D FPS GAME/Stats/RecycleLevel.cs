using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RecycleLevel : MonoBehaviour
{
    public GameObject gameOver;

    void Start()
    {
        GlobalLife.lifeValue -= 1;
        if (GlobalLife.lifeValue == 0)
        {
            gameOver.SetActive(true);
        }
        else
        {
            if (GlobalComplete.nextFloor == 4)
            {
                SceneManager.LoadScene(1);
            }
            else
            {
                SceneManager.LoadScene(GlobalComplete.nextFloor);
            }
        }
    }
}
