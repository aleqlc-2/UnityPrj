using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallController : MonoBehaviour
{
    private Rigidbody rb;
    private bool isReady = true;
    private Vector2 startPos;
    public float resetTime = 3.0f;

    public float captureRate = 0.3f; // 포획확률 30%
    public Text result;

    public GameObject effect; // 맞출시 파티클

    void Start()
    {
        result.text = ""; // 포획결과텍스트를 공백으로 초기화

        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

	private void OnCollisionEnter(Collision collision)
	{
        if (isReady) return;

        float draw = Random.Range(0, 1.0f);

        if (draw <= captureRate)
        {
            result.text = "포획 성공!";

            // DB에 저장된 상태를 포획된 상태로 저장
            DB_Manager.instance.UpdateCaptured();
        }
        else
            result.text = "포획에 실패해 도망쳤습니다...";

        // 포획했든 못했든 맞췄으면 파티클생성
        Instantiate(effect, collision.transform.position, Camera.main.transform.rotation);

        // 포획했든 못했든 맞췄으면 고양이 캐릭터를 제거하고 공을 비활성화
        Destroy(collision.gameObject);
        gameObject.SetActive(false);
	}

	void Update()
    {
        if (!isReady) return;

        // 공을 카메라 전방 하단에 배치
        SetBallPosition(Camera.main.transform);

        if (Input.touchCount > 0 && isReady)
		{
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began) // 터치시작했다면
            {
                startPos = touch.position; // 터치시작위치 저장
			}
            else if (touch.phase == TouchPhase.Ended) // 터치끝나면
			{
                // 손가락이 드래그한 픽셀의 Y축 거리를 구함
                float dragDistance = touch.position.y - startPos.y;

                // AR 카메라를 기준으로 던질 방향(전방 45도 위쪽)을 설정한다.
                Vector3 throwAngle = (Camera.main.transform.forward + Camera.main.transform.up).normalized;

                // 날아가는 동안 물리효과 적용되고 카메라앞에만 고정되지 않도록
                rb.isKinematic = false;
                isReady = false;

                // ForceMode.VelocityChange는 리지드바디가 가진 질량을 무시하고 직접적으로 속도의 변화를 주는 모드. 속도가 순간적으로 변함
                // 방향 * 드래그거리만큼 더세게날림 * 픽셀때문에 힘의크기낮춤
                rb.AddForce(throwAngle * dragDistance * 0.005f, ForceMode.VelocityChange);

                // 3초 후에 볼 초기화
                Invoke("ResetBall", resetTime);
			}
		}
    }

    private void SetBallPosition(Transform anchor)
	{
        Vector3 offset = anchor.forward * 0.5f + anchor.up * -0.2f;
        transform.position = anchor.position + offset;
	}

    private void ResetBall()
	{
        rb.isKinematic = true;
        rb.velocity = Vector3.zero;
        isReady = true;
        gameObject.SetActive(true);
	}
}
