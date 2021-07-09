using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Per10Health : MonoBehaviour
{
    public AudioSource collectSound;

    void OnTriggerEnter(Collider other)
    {
        if(GlobalHealth.healthValue >= 91)
        {
            GlobalHealth.healthValue = 100;
        }
        else
        {
            GlobalHealth.healthValue += 10;
        }
        collectSound.Play();
        GetComponent<BoxCollider>().enabled = false; // food 감싸던 콜라이더 비활성화
        this.gameObject.SetActive(false);
    }
}
