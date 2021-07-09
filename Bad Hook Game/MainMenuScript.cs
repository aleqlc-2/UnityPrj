using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public Button musicOnButton;
    public Button musicOffButton;
    public Button soundOnButton;
    public Button soundOffButton;

    void Start()
    {
        // 음악이 켜져있으면
        if (PlayerPrefs.GetInt("MusicOnOff") == 1)
        {
            musicOnButton.interactable = false; // on버튼 비활성화
            musicOffButton.interactable = true; // off버튼 활성화
        }

        // 소리가 켜져있으면
        if (PlayerPrefs.GetInt("SoundOnOff") == 1)
        {
            soundOnButton.interactable = false; // on버튼 비활성화
            soundOffButton.interactable = true; // off버튼 활성화
        }
    }

    public void MusicOnOff(int musicId)
    {
        SoundManagerScript.instance.uiButtonClickSound.Play();
        SoundManagerScript.instance.SetMusic(musicId);
        SoundManagerScript.instance.TurnMusicOnOff();
    }

    public void SoundOnOff(int soundId)
    {
        SoundManagerScript.instance.uiButtonClickSound.Play();
        SoundManagerScript.instance.SetSound(soundId);
        SoundManagerScript.instance.TurnSoundOnOff();
    }

    public void NextSceneCall(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    public void ExitCall()
    {
        Application.Quit();
    }
}
