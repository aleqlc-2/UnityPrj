using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetection : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(TagManager.PLAYER_TAG))
        {
            GameplayManager.instance.GameOver();
        }
    }
}
