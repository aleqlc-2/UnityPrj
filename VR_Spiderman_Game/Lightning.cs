using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Lightning : MonoBehaviour
{
    public Rigidbody player; // CameraRig의 Rigidbody

    public SteamVR_Action_Boolean trigger;
    public SteamVR_TrackedObject controller;
    public LineRenderer line;

    private Vector3 target;
    private float speed = 20f;

    void FixedUpdate()
    {
        if (trigger.GetStateDown(SteamVR_Input_Sources.RightHand) && controller.name == "Controller (right)")
        {
            fire();
        }
        else if(trigger.GetState(SteamVR_Input_Sources.RightHand) && controller.name == "Controller (right)" && line.enabled)
        {
            stay();
        }
        else if(trigger.GetStateUp(SteamVR_Input_Sources.RightHand) && controller.name == "Controller (right)")
        {
            end();
        }
        

        if (trigger.GetStateDown(SteamVR_Input_Sources.LeftHand) && controller.name == "Controller (left)")
        {
            fire();
        }
        else if(trigger.GetState(SteamVR_Input_Sources.LeftHand) && controller.name == "Controller (left)" && line.enabled)
        {
            stay();
        }
        else if(trigger.GetStateUp(SteamVR_Input_Sources.LeftHand) && controller.name == "Controller (left)")
        {
            end();
        }
    }

    private void fire()
    {
        RaycastHit hit;
        if (Physics.Raycast(controller.transform.position, controller.transform.forward, out hit))
        {
            line.enabled = true; // 인스펙터창에서 Line Renderer 비활성화해두었기때문에 이 코드 필요
            line.SetPosition(0, controller.transform.position);

            target = hit.point;
            line.SetPosition(1, target);

            player.AddForce((target - controller.transform.position).normalized * speed);

            line.material.mainTextureOffset = Vector2.zero; // 아직 파동없음
        }
    }

    private void stay()
    {
        // 충돌지점은 고정하되 컨트롤러는 계속 따라가도록 0번 line은 컨트롤러를 따라 계속 갱신
        line.SetPosition(0, controller.transform.position);

        // Rope가 x축을 기준으로 파동을 일으키도록
        line.material.mainTextureOffset =
            new Vector2(line.material.mainTextureOffset.x + Random.Range(-0.1f, 0.05f), 0f);
        
        player.AddForce((target - controller.transform.position).normalized * speed);
    }

    private void end()
    {
        line.enabled = false;
    }
}