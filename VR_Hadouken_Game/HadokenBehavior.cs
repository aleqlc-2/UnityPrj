using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class HadokenBehavior : MonoBehaviour
{
    private float currentScale = 0.5f;
    private float scaleLimit = 1.5f;

    public Transform handle;

    private Rigidbody rb;

    private ParticleSystem ball;
    public GameObject explosionEffect;

    void Start()
    {
        ball = gameObject.GetComponent<ParticleSystem>();
        StartCoroutine(GrowHadoken());
    }

    void Update()
    {
        if (handle)
        {
            // 핸들 움직이는대로 hadoken이 따라가도록
            transform.position = Vector3.Lerp(transform.position, handle.position, 0.9f);

            rb = gameObject.GetComponent<Rigidbody>(); // Start에 넣는 게 좋지 않나?
        }
    }
    
    // hadoken이 점점 커지도록
    private IEnumerator GrowHadoken()
    {
        while (currentScale < scaleLimit)
        {
            currentScale += 0.1f;
            Vector3 hadokenScale = new Vector3(currentScale, currentScale, currentScale);
            transform.localScale = hadokenScale;
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void Throw(SteamVR_Behaviour_Pose myHand)
    {
        StopAllCoroutines(); // 날아가는순간부터는 hadoken이 커지지않도록
        rb.isKinematic = false; // 물리효과 적용되도록
        rb.transform.parent = null; // child화 해제
        rb.velocity = myHand.GetVelocity(); // 오브젝트에 현재 컨트롤러의 움직임 속도를 전달
        rb.angularVelocity = myHand.GetAngularVelocity(); // 회전 속도도 전달
        handle = null; // 날아간 hadouken은 Update함수에서 handle의 위치를 따라가지않도록
        rb = null; // 변수 초기화
    }

    private void OnCollisionEnter(Collision col)
    {
        Kill();
    }

    private void Kill()
    {
        ball.Stop(); // 파티클 정지
        explosionEffect.SetActive(true);
        Destroy(gameObject, 2f);
    }
}
