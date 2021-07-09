using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoldCollect : MonoBehaviour
{
    public GameObject goldIngots;
    public AudioSource collectSound;
    public GameObject pickUpDisplay;

    void OnTriggerEnter(Collider other)
    {
        GlobalScore.scoreValue += 10000;
        GlobalComplete.treasureCount += 1;
        collectSound.Play();
        goldIngots.SetActive(false);
        GetComponent<BoxCollider>().enabled = false; // gold 감싸던 콜라이더 비활성화

        pickUpDisplay.SetActive(false);
        pickUpDisplay.GetComponent<Text>().text = "GOLD";
        pickUpDisplay.SetActive(true);
    }
}
