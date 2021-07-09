using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManagerScript : MonoBehaviour
{
    public AudioSource bgMusic;
    public AudioSource uiButtonClickSound;
    public AudioSource playerHitSound;
    public AudioSource playerDiedSound;
    public AudioSource playerGrapplingSound;
    public AudioSource coinCollectSound;
    public AudioSource enemyBlastSound;

    public static SoundManagerScript instance;

    void Awake()
    {
        MakeSingleton();
    }

    void MakeSingleton()
    {
        if (instance != null)
            Destroy(gameObject);
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        // Check it is first time gameplay or not
        if (!PlayerPrefs.HasKey("FirstTimeSoundCheck"))
        {
            PlayerPrefs.SetInt("MusicOnOff", 1);
            PlayerPrefs.SetInt("SoundOnOff", 1);
            PlayerPrefs.SetInt("FirstTimeSoundCheck", 0);
        }
        TurnMusicOnOff();
        TurnSoundOnOff();
    }

    public void TurnMusicOnOff()
    {
        if (GetMusic() == 1)
            bgMusic.enabled = true;
        else
            bgMusic.enabled = false;
    }

    public void TurnSoundOnOff()
    {
        if (GetSound() == 1)
        {
            uiButtonClickSound.enabled = true;
            playerHitSound.enabled = true;
            playerDiedSound.enabled = true;
            playerGrapplingSound.enabled = true;
            coinCollectSound.enabled = true;
            enemyBlastSound.enabled = true;
        }
        else
        {
            uiButtonClickSound.enabled = false;
            playerHitSound.enabled = false;
            playerDiedSound.enabled = false;
            playerGrapplingSound.enabled = false;
            coinCollectSound.enabled = false;
            enemyBlastSound.enabled = false;
        }
    }

    public void SetMusic(int isOn)
    {
        PlayerPrefs.SetInt("MusicOnOff", isOn);
    }

    public int GetMusic()
    {
        return PlayerPrefs.GetInt("MusicOnOff");
    }

    public void SetSound(int isOn)
    {
        PlayerPrefs.SetInt("SoundOnOff", isOn);
    }

    public int GetSound()
    {
        return PlayerPrefs.GetInt("SoundOnOff");
    }
}
