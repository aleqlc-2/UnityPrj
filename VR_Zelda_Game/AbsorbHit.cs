using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbsorbHit : MonoBehaviour
{
    Vector3 totalForce = Vector3.zero;
    Vector3 avgPosition = Vector3.zero;
    int totalPoints = 0;

    Rigidbody r;
    Renderer render;

    private bool firstPunch = true;

    private ReverseTime rt;

    public GameObject wallParent; // 인스펙터창에서 넣었음
    private List<ReverseTime> wall = new List<ReverseTime>();

    void Start()
    {
        r = GetComponent<Rigidbody>();
        rt = GetComponent<ReverseTime>();
        render = GetComponent<Renderer>();

        // Wall 객체의 하위계층 전부 가져와서 wall 리스트에 넣음
        // 부모객체까지 불러오지만 부모객체에는 ReverseTime 스크립트가 없으므로 List에 안들어감
        foreach (ReverseTime child in wallParent.GetComponentsInChildren<ReverseTime>())
        {
            wall.Add(child);
        }
    }

    void OnCollisionEnter(Collision collide)
    {
        if (firstPunch)
        {
            Boxing hand = collide.transform.GetComponent<Boxing>();
            if (hand != null)
            {
                StartCoroutine(WaitForForces()); // 코루틴이므로 아래 코드도 같이 실행
                firstPunch = false;
            }
        }

        totalForce += collide.impulse / Time.fixedDeltaTime;
        foreach (ContactPoint cp in collide.contacts)
        {
            avgPosition += cp.point;
            totalPoints++;
        }

        if (collide.gameObject.tag != "Ground")
        {
            StopCoroutine("Highlight"); // 이 코드 없어도 동작은 동일함
            StartCoroutine("Highlight");
        }
    }

    IEnumerator WaitForForces()
    {
        r.constraints = RigidbodyConstraints.FreezePosition; // 제자리에 고정된채로 rotation만 적용되면서 펀치당하도록
        yield return new WaitForSeconds(3f); // 큐브가 3초동안 제자리에서 펀치당함
        r.constraints = RigidbodyConstraints.None; // 날아갈 수 있게 제약을 없앰

        // 3초동안 날아가면서 모든 position과 rotation 저장
        // 이 코드가 r.AddForceAtPosition 메서드 위에 있어야 첫 프레임의 위치도 저장이 되어 원위치로 가까이 옴
        rt.Record(); // 펀치당하는 상자의 Record
        foreach (ReverseTime child in wall)
        {
            child.Record(); // 벽의 Record
        }

        // 다음 프레임까지 대기하라는 뜻.
        // 이 코드 없으면 첫 프레임 위치 저장안됨.
        yield return null;

        // totalForce 코루틴으로 인해 값 받아올 수 있음
        r.AddForceAtPosition(totalForce * 3f, avgPosition / totalPoints); // cube날아가기시작

        // 3초 동안 일시정지 하였다가(3초 동안 날아가다가) 아래 코드가 실행됨
        yield return new WaitForSeconds(3f);

        foreach (ReverseTime child in wall)
        {
            StartCoroutine(child.Playback(1));
        }

        // rt.Playback 코루틴이 종료될때까지 기다렸다가 아래 Reset코드가 실행됨
        yield return StartCoroutine(rt.Playback(1));

        Reset();
    }

    void Reset()
    {
        totalForce = Vector3.zero;
        avgPosition = Vector3.zero;
        totalPoints = 0;

        firstPunch = true;
    }

    IEnumerator Highlight()
    {
        Color start = Color.white;
        Color end = Color.red * 1.4f;

        float t = 0f;

        while (t < 0.1f) // 큐브가 맞을때 0.1초간 red가 되었다가
        {
            render.material.color = Color.Lerp(start, end, t / 0.1f);
            t += Time.deltaTime;
            yield return null;
        }

        t = 0f;

        while (t < 0.1f) // white로 돌아옴
        {
            render.material.color = Color.Lerp(end, start, t / 0.1f);
            t += Time.deltaTime;
            yield return null;
        }
    }
}
