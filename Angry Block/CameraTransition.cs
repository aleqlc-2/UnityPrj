using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTransition : MonoBehaviour
{
    private Transform cameraContainer;

    private float rotateSemiAmount = 4;
    private float shakeAmount;

    private Vector3 startingLocalPos;

    void Start()
    {
        cameraContainer = GameObject.Find("CameraContainer").transform;
    }

    void Update()
    {
        if (shakeAmount > 0.01f)
        {
            Vector3 localPosition = startingLocalPos; // 한번 shake할때마다 카메라의 초기위치인 0,0,0으로 초기화하면서 떨리도록
            localPosition.x += shakeAmount * Random.Range(3, 5);
            localPosition.y += shakeAmount * Random.Range(3, 5);
            transform.localPosition = localPosition;
            shakeAmount = 0.9f * shakeAmount; // 점점 약하게 shake하면서 shake종료되도록
        }
    }

    public void Shake()
    {
        shakeAmount = Mathf.Min(0.1f, shakeAmount + 0.01f);
    }

    public void MediumShake()
    {
        shakeAmount = Mathf.Min(0.15f, shakeAmount + 0.015f);
    }

    public void RotateCameraToSide()
    {
        StartCoroutine(RotateCameraToSideRoutine());
    }

    private IEnumerator RotateCameraToSideRoutine()
    {
        int frames = 20;
        float increment = rotateSemiAmount / frames;

        for (int i = 0; i < frames; i++)
        {
            // Vector3.zero점을 지나는 Vector3.up축을 기준으로 increment 각도만큼 공전함
            cameraContainer.RotateAround(Vector3.zero, Vector3.up, increment);
            yield return null;
        }

        yield break;
    }

    public void RotateCameraToFront()
    {
        StartCoroutine(RotateCameraToFrontRoutine());
    }

    private IEnumerator RotateCameraToFrontRoutine()
    {
        int frames = 60;
        float increment = rotateSemiAmount / frames;

        for (int i = 0; i < frames; i++)
        {
            // 다음단계실행시 increment에 -붙이고 frames를 60으로 줘서 전 단계에서 회전했던 만큼의 1/3만 원위치로 되돌아오도록
            cameraContainer.RotateAround(Vector3.zero, Vector3.up, -increment);
            yield return null;
        }

        cameraContainer.localEulerAngles = new Vector3(0, 0, 0); // 새로운 단계가 시작되면 카메라가 정면을 바라보도록

        yield break;
    }
}
