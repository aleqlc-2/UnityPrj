using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabItem : MonoBehaviour
{
    private Rigidbody rBody;
    private float moveScale;

    void Start()
    {
        rBody = GetComponent<Rigidbody>();
    }

    public void Grab(bool shouldGrab)
    {
        // 처음 잡는순간(버튼down일때)은 물리효과 미적용해야 잡히는 물체가 튀지않음
        rBody.isKinematic = shouldGrab;
    }

    public void SetMoveScale(Vector3 handPosition)
    {
        // 플레이어의 위치
        Vector3 origin = GameObject.FindGameObjectWithTag("MainCamera").transform.position;
        
        // 물체의 위치 - 플레이어의 위치 / 핸들의 위치 - 플레이어의 위치이므로
        // 핸들을 움직이면 잡힌 물체의 움직임은 상대적으로 커져서 움직임
        moveScale = Vector3.Magnitude(transform.position - origin) / Vector3.Magnitude(handPosition - origin);
    }

    public void Move(Vector3 curHandPos, Vector3 lastHandPos, Quaternion curHandRot, Quaternion lastHandRot)
    {
         // 리지드바디.position은 Vector3값임
        rBody.MovePosition(rBody.position + (curHandPos - lastHandPos) * moveScale);
        rBody.MoveRotation(Quaternion.RotateTowards(lastHandRot, curHandRot, Time.deltaTime));
    }
}
