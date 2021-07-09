using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class PunchHand : MonoBehaviour
{
    public SteamVR_TrackedObject hand;
    private Rigidbody rBody;

    private float speed = 10f;

    void Start()
    {
        rBody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        rBody.MovePosition(hand.transform.position);
        rBody.MoveRotation(hand.transform.rotation);
    }

    void OnCollisionEnter(Collision other)
    {
        if (other == null) return;

        Rigidbody otherR = other.gameObject.GetComponentInChildren<Rigidbody>();

        // 글러브의 면적이 넓기때문에 충돌지점이 많아 이런식으로 평균을 구하는듯
        // 그렇지 않으면 특정 충돌지점만 적용되어 이상한 방향으로 날아가버릴듯
        Vector3 avgPoint = Vector3.zero;
        foreach (ContactPoint p in other.contacts)
        {
            avgPoint += p.point;
        }
        avgPoint /= other.contacts.Length;

        Vector3 dir = (avgPoint - transform.position).normalized;

        // 더 세게 치면 더 많이 나가도록 rBody.velocity.magnitude를 곱함
        otherR.AddForceAtPosition(dir * speed * rBody.velocity.magnitude, avgPoint);
    }
}
