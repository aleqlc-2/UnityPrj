using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class BasicInputManagerLeft : MonoBehaviour
{
    public SteamVR_Action_Boolean gripL;

    public Rigidbody Body;
    public SteamVR_TrackedObject controllerLeft;

    [HideInInspector]
    public Vector3 prevPosLeft;

    [HideInInspector]
    public bool canGripL;

    void Start()
    {
        // 컨트롤러의 시작할때 localPosition은 0,0,0
        prevPosLeft = controllerLeft.transform.localPosition;
    }

    void FixedUpdate()
    {
        if (canGripL && gripL.GetState(SteamVR_Input_Sources.LeftHand))
        {
            Body.useGravity = false;
            Body.isKinematic = true; // 등반중일때는 튕겨나가지않도록
            // 처음 잡은 순간은 0,0,0 - 0,0,0으로 변화없음
            Body.transform.position += (prevPosLeft - controllerLeft.transform.localPosition);
        }
        else if (canGripL && gripL.GetStateUp(SteamVR_Input_Sources.LeftHand)) // grip버튼 눌렀다 뗐을때
        {
            Body.useGravity = true;
            Body.isKinematic = false;
            // 높은데서 떨어질수록 가속도
            Body.velocity = (prevPosLeft - controllerLeft.transform.localPosition) / Time.deltaTime;
        }
        else // 양손 다 놓치면 떨어지도록
        {
            Body.useGravity = true;
            Body.isKinematic = false;
        }
        // 잡은채로 움직이는 순간 변화값이 prevPosLeft에 저장된채로 if문으로 들어가면
        // prevPosLeft 값은 변했고 controllerLeft.transform.localPosition은 여전히 그대로이므로 몸이 움직임
        prevPosLeft = controllerLeft.transform.localPosition;
    }

    void OnTriggerEnter()
    {
        canGripL = true;
    }

    void OnTriggerExit()
    {
        canGripL = false;
    }
}
