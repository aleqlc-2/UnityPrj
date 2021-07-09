using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class BasicInputManagerRight : MonoBehaviour
{
    public SteamVR_Action_Boolean gripR;

    public Rigidbody Body;
    public SteamVR_TrackedObject controllerR;

    [HideInInspector]
    public Vector3 prevPosRight;

    [HideInInspector]
    public bool canGripR;

    void Start()
    {
        prevPosRight = controllerR.transform.localPosition;
    }

    void FixedUpdate()
    {
        if (canGripR && gripR.GetState(SteamVR_Input_Sources.RightHand)) // grip버튼 누르고있을때
        {
            Body.useGravity = false;
            Body.isKinematic = true;
            Body.transform.position += (prevPosRight - controllerR.transform.localPosition);
        }
        else if (canGripR && gripR.GetStateUp(SteamVR_Input_Sources.RightHand)) // grip버튼 뗐을때
        {
            Body.useGravity = true;
            Body.isKinematic = false;
            // 높은데서 떨어질수록 가속도
            Body.velocity = (prevPosRight - controllerR.transform.localPosition) / Time.deltaTime;
        }
        else // 양손 다 놓치면 떨어지도록
        {
            Body.useGravity = true;
            Body.isKinematic = false;
        }
        prevPosRight = controllerR.transform.localPosition;
    }

    void OnTriggerEnter()
    {
        canGripR = true;
    }

    void OnTriggerExit()
    {
        canGripR = false;
    }
}
