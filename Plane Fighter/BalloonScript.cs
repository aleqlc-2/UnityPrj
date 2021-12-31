using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonScript : MonoBehaviour
{
    private GameObject player;

    void Start()
    {
        player = GameObject.FindWithTag("Player");

        if (player.transform.position.y > 140)
            GetComponent<Rigidbody>().velocity = new Vector3(0, 30, 0);
        else
            GetComponent<Rigidbody>().velocity = new Vector3(0, 20, 0);
    }

    void Update()
    {
        if (transform.position.z < player.transform.position.z - 100) gameObject.SetActive(false);
    }
}
