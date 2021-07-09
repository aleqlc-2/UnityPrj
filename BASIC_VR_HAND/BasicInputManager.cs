using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class BasicInputManager : MonoBehaviour
{
    public SteamVR_Action_Boolean grip;
    public SteamVR_Action_Boolean trigger;
    public SteamVR_Action_Boolean menu;
    public SteamVR_Action_Boolean trackpad;
    
    public SteamVR_Action_Vector2 trackpadMove;

    public SteamVR_Action_Vibration haptic;

    void Start()
    {
        
    }

    void Update()
    {
        // grip버튼을 눌렀을 때 1회, Any = 모든 컨트롤러, LeftHand, RightHand
        if (grip.GetStateDown(SteamVR_Input_Sources.Any)) // s
        {
            Debug.Log("Grip Button Down");
        }

        // trigger버튼을 눌렀을 때 1회, LeftHand = 왼쪽 컨트롤러만
        if (trigger.GetStateDown(SteamVR_Input_Sources.LeftHand))
        {
            haptic.Execute(0, 0.5f, 160f, 0.5f, SteamVR_Input_Sources.LeftHand);
        }
        // trigger버튼을 눌렀을 때 1회, RightHand = 오른쪽 컨트롤러만
        if (trigger.GetStateDown(SteamVR_Input_Sources.RightHand))
        {
            haptic.Execute(0, 0.5f, 160f, 0.5f, SteamVR_Input_Sources.RightHand);
        }

        // menu버튼을 눌렀을 때 1회
        if (menu.GetStateDown(SteamVR_Input_Sources.Any))
        {
            Debug.Log("Menu Button Down");
        }

        // trackpad버튼을 눌렀을 때 1회
        if (trackpad.GetStateDown(SteamVR_Input_Sources.Any))
        {
            Debug.Log("Trackpad Button Down");
        }

        Vector2 moveDir = trackpadMove.GetAxis(SteamVR_Input_Sources.Any);
        Debug.Log("Trackpad X pos : " + moveDir.x);
        Debug.Log("Trackpad Y pos : " + moveDir.y);

        // y는 위로가기때문에 그대로
        transform.position = Vector3.Lerp(
             transform.position,
             new Vector3(transform.position.x + moveDir.x, transform.position.y, transform.position.z + moveDir.y),
             Time.deltaTime);

        // 이동하는 방향으로 바라보기
        transform.rotation = Quaternion.LookRotation
                        (new Vector3(transform.position.x + moveDir.x, 0, transform.position.z + moveDir.y));
    }
}
