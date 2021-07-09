using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraLife : MonoBehaviour
{
    public AudioSource lifeSound;

    void Update()
    {
        transform.Rotate(0, 1, 0, Space.World);
    }

    void OnTriggerEnter(Collider other)
    {
        lifeSound.Play();
        GlobalLife.lifeValue += 1;
        this.gameObject.SetActive(false);
    }
}
