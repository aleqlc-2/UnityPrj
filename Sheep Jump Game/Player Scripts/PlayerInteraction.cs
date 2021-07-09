using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    private Rigidbody rb;
    private bool playerDied;
    private CameraFollow cameraFollow;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // Camera.main이 Main Camera인듯
        cameraFollow = Camera.main.GetComponent<CameraFollow>();
    }

    void Update()
    {
        if (!playerDied)
        {
            if (rb.velocity.sqrMagnitude > 60)
            {
                playerDied = true;
                cameraFollow.CanFollow = false;

                SoundManager.instance.GameEndSound();
                GameplayController.instance.RestartGame();
            }
        }
    }

    void OnTriggerEnter(Collider target)
    {
        if (target.tag == "Coin")
        {
            target.gameObject.SetActive(false);
            SoundManager.instance.PickedUpCoin();
            GameplayController.instance.IncrementScore();
        }

        if (target.tag == "Spike")
        {
            cameraFollow.CanFollow = false;
            gameObject.SetActive(false);
            SoundManager.instance.GameEndSound();
            GameplayController.instance.RestartGame();
        }
    }

    void OnCollisionEnter(Collision target)
    {
        if (target.gameObject.tag == "EndPlatform")
        {
            SoundManager.instance.GameStartSound();
            GameplayController.instance.RestartGame();
        }
    }
}
