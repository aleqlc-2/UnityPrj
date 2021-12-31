using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioClip pickUp_Sound, dead_Sound;

    void Awake()
    {
        MakeInstance();
    }

    private void MakeInstance()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void Play_PickUpSound()
    {
        AudioSource.PlayClipAtPoint(pickUp_Sound, transform.position);
    }

    public void Play_DeadSound()
    {
        // 지정된 위치에서 소리가 남. 멀리두면 소리가 작게들림
        AudioSource.PlayClipAtPoint(dead_Sound, transform.position);
    }
}
