using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float follow_Height = 8f;
    public float follow_Distance = 6f;

    private Transform player;

    private float target_Height;
    private float current_Height;
    private float current_Rotation;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        target_Height = player.position.y + follow_Height;
        current_Height = Mathf.Lerp(transform.position.y, target_Height, 0.9f * Time.deltaTime);

        current_Rotation = transform.eulerAngles.y; // 카메라의 현재 회전값인 165(하이어라키창에 입력된 값)
        Quaternion euler = Quaternion.Euler(0f, current_Rotation, 0f); // 회전값을 Quaternion으로 변환

        Vector3 target_Position = player.position - (euler * Vector3.forward) * follow_Distance; // x, z
        target_Position.y = current_Height; // y

        transform.position = target_Position;
        transform.LookAt(player);
    }
}
