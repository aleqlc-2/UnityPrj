using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudScript : MonoBehaviour
{
    private GameObject player;

    void Start()
    {
        player = GameObject.FindWithTag("Player");

        // 한번 줬는데 구름은 계속 움직이네
        GetComponent<Rigidbody>().velocity = new Vector3(Random.Range(-15, -10), 0, 0);
    }

    void Update()
    {
        // 플레이어가 지나간 구름은 비활성화
        if (transform.position.z < player.transform.position.z)
        {
            gameObject.SetActive(false);
        }
    }
}
