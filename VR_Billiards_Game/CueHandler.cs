using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class CueHandler : MonoBehaviour
{
    public SteamVR_Action_Boolean trigger;

    public SteamVR_TrackedObject frontController;
    public SteamVR_TrackedObject backController;

    private Rigidbody cueRB;

    private Vector3 cuePos;
    private Vector3 lockUp;
    private float lockOffset;

    public Transform cueTip;

    void Start()
    {
        cueRB = gameObject.GetComponent<Rigidbody>();
    }

    void Update()
    {
        UpdateCuePosition();
    }

    void UpdateCuePosition()
    {
        Vector3 frontPos = frontController.transform.position; // lefthand 위치 계속하여 갱신
        Vector3 backPos = backController.transform.position; // righthand 위치 계속하여 갱신

        if (trigger.GetStateDown(SteamVR_Input_Sources.RightHand)) // 처음누를때
        {
            lockUp = transform.up; // 처음누를때 y값 저장
            lockOffset = (frontPos - backPos).magnitude; // 처음누를때 lockOffset저장
        }
        else if (trigger.GetState(SteamVR_Input_Sources.RightHand)) // 누르고있을때
        {
            float currOffset = (frontPos - backPos).magnitude; // 누르고있을때 currOffset 계속 갱신

            // 갱신된 offset만큼을 y에 곱하여 감도를 계속하여 갱신하며 cue의 위치 계속 갱신
            cueRB.MovePosition(cuePos + lockUp * (lockOffset - currOffset));
        }
        else // else이므로 눌렀다 뗐을 경우와 아예 free mode인 경우가 다 포함된거임.
        {
            // righthand를 움직일 때 감도를 더 크게
            cuePos = 0.75f * backPos + 0.25f * frontPos;
            cueRB.MovePosition(cuePos);

            // cue가 자꾸 수직으로 서서 90도 회전시킴
            // LookRotation메서드의 인자는 forward 거리
            cueRB.MoveRotation(Quaternion.Euler(90f, 0f, 0f) * Quaternion.LookRotation(frontPos - backPos));
        }
    }

    void OnCollisionEnter(Collision col)
    {
        Rigidbody rb = col.gameObject.GetComponent<Rigidbody>(); // ball의 리지드바디
        if (!rb) return;
            
        // contacts[0].point는 첫번째 충돌지점
        Vector3 forceDirection = (col.contacts[0].point - cueTip.position).normalized; // 방향

        // 인자는 방향 * 스피드인데 cue를 더 많이 당긴만큼 강하게 적용하기위해
        // 스피드에 cueRB.velocity.magnitude를 곱함
        rb.AddForce(forceDirection * cueRB.velocity.magnitude);
    }
}
