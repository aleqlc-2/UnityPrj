using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [SerializeField]
    private AudioSource gameStart, gameEnd, coinSound, jumpSound;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void GameStartSound()
    {
        gameStart.Play();
    }

    public void GameEndSound()
    {
        gameEnd.Play();
    }

    public void PickedUpCoin()
    {
        coinSound.Play();
    }

    public void JumpSound()
    {
        jumpSound.Play();
    }

}
