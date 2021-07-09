using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chain : MonoBehaviour
{
    public Transform player;

    public float speed = 2f;

    public static bool isFired;

    void Start()
    {
        isFired = false;
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1")) // 스페이스바
        {
            isFired = true;
        }

        if (isFired)
        {
            transform.localScale += Vector3.up * Time.deltaTime * speed;
        }
        else
        {
            transform.position = player.position;
            transform.localScale = new Vector3(1f, 0f, 1f);
        }
    }
}
