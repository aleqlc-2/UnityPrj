using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    void Start()
    {
        // 각각의 코인에 붙어있는 스크립트이므로 코인의 개수만큼 coinCount변수의 값이 +1됨
        GameplayManager.instance.SetCoinCount(1);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(TagManager.PLAYER_TAG))
        {
            GameplayManager.instance.SetCoinCount(-1);
            AudioManager.instance.PlayCoinSound();
            gameObject.SetActive(false);
        }
    }
}
