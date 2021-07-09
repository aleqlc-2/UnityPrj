using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullHealthCollect : MonoBehaviour
{
    public AudioSource collectSound;

    void OnTriggerEnter(Collider other)
    {
        GlobalHealth.healthValue = 100;
        collectSound.Play();
        GetComponent<BoxCollider>().enabled = false; // food 감싸던 콜라이더 비활성화
        this.gameObject.SetActive(false);
    }
}
