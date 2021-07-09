using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class VRHand : MonoBehaviour
{
    // 해당 컨트롤러 Grip버튼의 Action 변수
    public SteamVR_Action_Boolean grip;

    // 해당 컨트롤러의 동작 자세(pose)를 저장하는 변수, Input Source 개체
    private SteamVR_Behaviour_Pose myHand = null;

    private Transform myTransform = null;
    private Rigidbody myRigidbody = null;

    // 동시에 부딪힌 오브젝트들의 Rigidbody를 저장하는 변수
    private List<Rigidbody> contactRigidbodies = new List<Rigidbody>();

    // 컨트롤러에 부착될 가장 가까운 오브젝트의 Rigidbody를 저장하는 변수
    private Rigidbody currentRigidbody = null;

    void Start()
    {
        myHand = GetComponent<SteamVR_Behaviour_Pose>();
        myTransform = GetComponent<Transform>();
        myRigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // grip 버튼을 눌렀을 때
        if (grip.GetStateDown(myHand.inputSource))
        {
            // 오브젝트를 컨트롤러에 부착하는 함수
            Pickup();
        }

        // grip 버튼을 뗐을 때
        if (grip.GetStateUp(myHand.inputSource))
        {
            // 컨트롤러에 부착되어 있는 오브젝트를 해제하는 함수
            Drop();
        }
    }

    // 부딪힌 오브젝트중 가장 가까운 오브젝트를 컨트롤러에 부착시키는 함수
    public void Pickup()
    {
        currentRigidbody = GetNearestRigidBody();
        if (currentRigidbody == null) return;

        currentRigidbody.useGravity = false; // 오브젝트의 중력 비활성화
        currentRigidbody.isKinematic = true; // 오브젝트의 물리효과 비활성화, 잡을 때 튀지않도록

        // 컨트롤러의 위치에서 부착, 이 코드 없으면 오브젝트 자신의 위치에서 부착됨
        currentRigidbody.transform.position = myTransform.position;

        // child화하여 계속 컨트롤러의 위치를 따라오도록
        currentRigidbody.transform.parent = myTransform;
    }

    // 컨트롤러에 부딪힌 오브젝트들 중 컨트롤러와 가장 가까운 오브젝트를 판별
    private Rigidbody GetNearestRigidBody()
    {
        Rigidbody nearestRigidbody = null;

        float minDistance = float.MaxValue;
        float distance = 0.0f;

        // contactRigidbodies 리스트 안에 저장된 Rigidbody 컴포넌트들을 하나씩 불러서
        foreach (Rigidbody rigidbody in contactRigidbodies)
        {
            // 컨트롤러와 Rigidbody를 가지고 있는 오브젝트 사이의 거리를 비교
            distance = (rigidbody.transform.position - myTransform.position).sqrMagnitude;

            // 최소거리와 가장 가까운 rigidbody 갱신
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestRigidbody = rigidbody;
            }
        }
        return nearestRigidbody;
    }

    // 컨트롤러에 부착되어 있는 오브젝트를 다시 해제하는 함수
    public void Drop()
    {
        if (currentRigidbody == null) return;
        currentRigidbody.useGravity = true; // 날아가다 떨어져야하므로 중력 활성화
        currentRigidbody.isKinematic = false; // 오브젝트의 물리효과 활성화
        currentRigidbody.transform.parent = null; // child화 해제
        currentRigidbody.velocity = myHand.GetVelocity(); // 오브젝트에 현재 컨트롤러의 움직임 속도를 전달
        currentRigidbody.angularVelocity = myHand.GetAngularVelocity(); // 회전 속도도 전달
        currentRigidbody = null; // rigidbody 변수 초기화
    }

    // 컨트롤러와 오브젝트간에 충돌이 일어났을 때
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Interactable"))
        {
            // 충돌이 일어난 오브젝트의 Rigidbody를 리스트에 추가
            contactRigidbodies.Add(other.gameObject.GetComponent<Rigidbody>());
        }
    }

    // 컨트롤러와 오브젝트간에 충돌이 끝났을 때
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Interactable"))
        {
            // 충돌이 끝난 오브젝트의 Rigidbody를 리스트에서 삭제
            contactRigidbodies.Remove(other.gameObject.GetComponent<Rigidbody>());
        }
    }
}
