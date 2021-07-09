using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform player;
    private Vector3 startPos;

    private float damping = 2f;
    private float height = 10f;

    private bool can_Follow;
    public bool CanFollow
    {
        get { return can_Follow; }
        set { can_Follow = value; }
    }

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        startPos = transform.position;
        can_Follow = true;
    }

    void Update()
    {
        Follow();
    }

    void Follow()
    {
        if (can_Follow)
        {
            transform.position = Vector3.Lerp(
                                    transform.position,
                                    new Vector3(player.position.x + 10f,
                                                player.position.y + height,
                                                player.position.z -10f),
                                    Time.deltaTime * damping);

            //transform.position = player.position; // 이렇게만 써버리면 player가 안보임
        }
    }
} // class
