using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ForceGrab : MonoBehaviour
{
    public SteamVR_Action_Boolean trigger;
    //public SteamVR_TrackedObject controller;
    private LineRenderer lineRenderer;
    private Vector3[] positions;
    private GrabItem grabbable;
    private bool grabbed;
    private Vector3 lastHandPos;
    private Quaternion lastHandRot;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        positions = new Vector3[2];
    }

    void Update()
    {
        if (!grabbed) // 무언가 잡고있으면 RaycastForGrabbableObject() 호출되지 않도록
        {
            grabbable = RaycastForGrabbableObject();
            if (!grabbable) return; // 잡을 수 있는 물체에 ray가 그려졌을때에만 아래 코드로 내려가도록
        }

        Vector3 curHandPos = transform.position;
        Quaternion curHandRot = transform.rotation;

        if (trigger.GetStateDown(SteamVR_Input_Sources.Any))
        {
            grabbed = true;

            // 잡는순간 튀지않도록 물리효과 미적용
            // 다른 물체와 부딪히는 것은 다른 물체는 grabbable.Grab(true); 하지 않았기때문.
            grabbable.Grab(true);
            grabbable.SetMoveScale(transform.position);
            lastHandPos = curHandPos;
            lastHandRot = curHandRot;
            DisplayLine(false, transform.position); // 그랩이 시작되면 line 안보이게
        }
        // 잡고있을때 이 else if 구문에 머무는게 아니라 계속 Update로 돔
        else if (trigger.GetState(SteamVR_Input_Sources.Any))
        {
            grabbable.Move(curHandPos, lastHandPos, curHandRot, lastHandRot);
        }
        else if (trigger.GetStateUp(SteamVR_Input_Sources.Any))
        {
            grabbed = false;
            grabbable.Grab(false); // 놓으면 물리효과 적용
        }

        // 이 코드 없으면 그랩중에 lastHandPos값이 갱신이 안되서 상자 강하게 날아감
        // 핸들 움직이다 멈추면 잡힌 물체도 움직이지 않도록
        lastHandPos = curHandPos;
        lastHandRot = curHandRot;
    }

    private GrabItem RaycastForGrabbableObject()
    {
        RaycastHit hit;
        Ray r = new Ray(transform.position, transform.forward);
        Debug.DrawRay(transform.position, transform.forward);

        // ray가 GrabItem스크립트가 부착되어있는 물체에 닿았을때만 line을 그림
        if (Physics.Raycast(r, out hit, Mathf.Infinity) && hit.collider.gameObject.GetComponent<GrabItem>() != null)
        {
            DisplayLine(true, hit.point);
            return hit.collider.gameObject.GetComponent<GrabItem>(); // 스크립트객체를 반환
        }
        else
        {
            // 이 코드 없으면 마지막 충돌지점에서 line이 제거되지않고 남아있음
            DisplayLine(false, transform.position);
            return null;
        }
    }

    void DisplayLine(bool display, Vector3 endpoint)
    {
        lineRenderer.enabled = display;
        positions[0] = transform.position;
        positions[1] = endpoint;
        lineRenderer.SetPositions(positions); // s
    }
}
