using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    private GameObject player;

    private bool cameraLockToPlayer = false;
    private bool cameraEndScreen = false;
    [HideInInspector] public bool gameStarted;

    private Vector3 currentAngle;

    private int cameraType = 0;

    public Vector3 cameraMenuPosition = new Vector3(174f, 33f, -2314f); // 카메라 시작위치

    public float cameraTilt = 10f; // 카메라 기울기

    // add player controller
    private PlayerController playerController;

    void Start()
    {
        currentAngle = transform.eulerAngles; // 카메라 시작 기울기를 변수에 저장
        transform.position = cameraMenuPosition; // 카메라 시작위치 설정

        player = GameObject.FindWithTag("Player");

        playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
    }

    void Update()
    {
        SwitchCameraType();
    }

    void LateUpdate()
    {
        LerpCameraToGameStart();
        CameraFollowPlayer();
    }

    // 게임 시작시 카메라가 플레이어쪽으로 lerp하여 이동
    private void LerpCameraToGameStart()
    {
        if (gameStarted)
        {
            transform.position = new Vector3(Mathf.Lerp(transform.position.x, player.transform.position.x, Time.deltaTime / 1.5f),
                                             Mathf.Lerp(transform.position.y, player.transform.position.y + 20f, Time.deltaTime / 1.5f),
                                             Mathf.Lerp(transform.position.z, player.transform.position.z - 80f, Time.deltaTime / 1.5f));

            // x축을 기준으로만 10f만큼 회전
            currentAngle = new Vector3(Mathf.LerpAngle(currentAngle.x, cameraTilt, Time.deltaTime / 1.5f),
                                       Mathf.LerpAngle(currentAngle.y, 0f, Time.deltaTime / 1.5f),
                                       Mathf.LerpAngle(currentAngle.z, 0f, Time.deltaTime));

            transform.eulerAngles = currentAngle;

            // 게임시작시 카메라 lerp가 끝나면 if문 true
            if (transform.position.x > player.transform.position.x - 0.5f)
            {
                gameStarted = false; // 카메라 lerp종료
                cameraLockToPlayer = true; // 카메라가 플레이어 따라가기 시작

                // 이륙 애니메이션 시작
                playerController.StartTakeOff();
            }
        }
    }

    private void CameraFollowPlayer()
    {
        if (cameraLockToPlayer && !cameraEndScreen)
        {
            if (cameraType == 0) // 3인칭
            {
                transform.position = new Vector3(Mathf.Lerp(transform.position.x, player.transform.position.x, Time.deltaTime * 5f),
                                                 Mathf.Lerp(transform.position.y, player.transform.position.y + 20f, Time.deltaTime + 5f),
                                                 player.transform.position.z - 80f);

                transform.eulerAngles = new Vector3(cameraTilt, 0f, 0f);
            }
            else // 1인칭
            {
                transform.position = new Vector3(250f, 90f, player.transform.position.z - 120f);
                transform.eulerAngles = new Vector3(0f, 0f, 0f);
            }
        }
    }

    private void SwitchCameraType()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (cameraType == 0)
                cameraType = 1;
            else
                cameraType = 0;
        }
    }
}
