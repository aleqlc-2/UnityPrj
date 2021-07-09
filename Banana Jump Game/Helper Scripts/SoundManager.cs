using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [SerializeField] private AudioSource soundFX;
    [SerializeField] private AudioClip jumpClip, gameOverClip;

    void Awake()
    {
        if (instance == null)
            instance = this;

        // audiosource 인스펙터창에서 직접 넣었으므로 getcomponent 할 필요 없음
    }

    public void JumpSoundFX()
    {
        soundFX.clip = jumpClip;
        soundFX.Play();
    }

    public void GameOverSoundFX()
    {
        soundFX.clip = gameOverClip;
        soundFX.Play();
    }
}
