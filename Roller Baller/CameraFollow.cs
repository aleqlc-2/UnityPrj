using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform playerTarget;

    [SerializeField] private float distance = 10f;
    [SerializeField] private float cameraHeight = 5f;
    [SerializeField] private float heightDamping = 2f;
    private float wantedHeight, currentHeight;

    private Quaternion currentRotation;

    void Awake()
    {
        playerTarget = GameObject.FindWithTag(TagManager.PLAYER_TAG).transform;
    }

    void LateUpdate()
    {
        if (!playerTarget) return;

        // 카메라의 높이
        wantedHeight = playerTarget.position.y + cameraHeight;
        currentHeight = transform.position.y;
        currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightDamping * Time.deltaTime);

        // 하이어라키창에 입력된 카메라의 회전 y값(플레이어를 바라보는 각도)
        currentRotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);

        transform.position = playerTarget.position; // 일단 카메라의위치 = 플레이어의위치
        transform.position -= currentRotation * Vector3.forward * distance; // 플레이어의 위치에서 (-8.6, 0.0, -5.1)를 빼준 값을 할당
        transform.position = new Vector3(transform.position.x, currentHeight, transform.position.z); // 높이만 재설정

        transform.LookAt(playerTarget);
    }
}
