using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSoundController : MonoBehaviour
{
    public AudioSource playerAttackSound;

    // 플레이어의 노말어택 애니메이션의 이벤트에 등록하여 공격할때 소리나도록
    private void Play_PlayerAttackSound()
    {
        playerAttackSound.Play();
    }
}
