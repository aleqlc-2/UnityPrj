using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectPuzzleController : MonoBehaviour
{
    public void SelectPuzzle()
    {
        string[] name = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name.Split(); // 공백을 기준으로 분리
        int index = int.Parse(name[1]);
        Debug.Log("You selected puzzle number " + index);
        if (GameManager.instance != null) GameManager.instance.SetPuzzleIndex(index);

        SceneManager.LoadScene("Gameplay");
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
