using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;

    public Camera myCamera;

    // Change orthographic zoom size
    public float minSize;
    public float maxSize;

    // 카메라가 따라가는 속도
    [Range(1f, 10f)] public float speed;

    // 카메라가 흔들리는 정도
    public float shakeForce = 5f;

    public bool isFollow;

    // 카메라가 따라가는 대상의 위치
    public Transform target;

    // target의 속도를 계산하여 zoom effect에 사용하기위해
    private Rigidbody2D target_rigidbody;

    // 타겟과의 거리
    public Vector3 padding;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        target_rigidbody = target.GetComponent<Rigidbody2D>();
        isFollow = true;
    }

    private void FixedUpdate()
    {
        if (isFollow)
        {
            // 타겟과의 거리가 멀수록 빨리 카메라가 따라가도록 하기위해
            // Vector3.Distance(transform.position, target.position)를 곱함
            transform.position = Vector3.Lerp(
                            transform.position,
                            target.position + padding,
                            Time.deltaTime * speed * Vector3.Distance(transform.position, target.position));
        }
    }

    private void LateUpdate()
    {
        // 플레이어 속도가 빨라질수록 orthographicSize가 커져서 카메라가 더 멀리서 바라봄
        if (isFollow)
        {
            myCamera.orthographicSize = Mathf.Lerp(
                myCamera.orthographicSize,
                Mathf.Clamp(Remap(target_rigidbody.velocity.magnitude, 0, 200, minSize, maxSize), minSize, maxSize),
                Time.deltaTime * speed);
        }
    }

    private float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return ((value - from1) / (to1 - from1) * (to2 - from2)) + from2;
    }

    public void CallCameraShakeEffect()
    {
        StopCoroutine(IShake());
        StartCoroutine(IShake());
    }

    private IEnumerator IShake()
    {
        float offset = shakeForce * 0.1f;
        float min = offset - 0.1f;
        float max = offset;
        float duration = 0.25f;
        float t = 0f; // time

        Vector3 newPos;

        // 흔들릴때 메인카메라의 변하는 위치를 저장
        Transform cam = transform.GetChild(0); // CameraHandler의 자식개체인 Main Camera
        cam.eulerAngles = Vector3.zero; // 카메라의 rotation을 zero로 설정
        Time.timeScale = 0.5f; // 흔들릴 때 슬로우모션을 적용하기 위해

        yield return new WaitForSecondsRealtime(0.1f); // Realtime

        while (t < duration) // 0.25초동안 흔들림
        {
            t += Time.deltaTime;

            // 랜덤으로 흔들리도록(위치도 랜덤, 회전도 랜덤)
            newPos = new Vector3(Random.Range(min, max), Random.Range(min, max), 0);
            cam.localPosition = newPos;
            cam.rotation = Quaternion.Euler(Vector3.forward * Random.Range(-2f, 2f)); // 축 * 값

            // 살짝 슬로우모션 걸고 흔들리면서 timeScale 원상복귀
            Time.timeScale = Mathf.Lerp(Time.timeScale, 1, t / duration);

            // 이 코드 없으면 슬로우모션만 걸리고 shake가 안됨
            // 랜덤위치 잡고 한프레임쉬고 이걸 반복해줘야 shake되는 듯
            // 그리고 IEnumerator반환하려면 yield return 필요
            yield return null;
        }

        // reset
        cam.localPosition = Vector3.zero;
        cam.eulerAngles = Vector3.zero;
        Time.timeScale = 1f;
    }
}
