using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ControlTime : MonoBehaviour
{
    public SteamVR_TrackedObject headset; // CameraRig안에 있는 camera
    public SteamVR_TrackedObject left;
    public SteamVR_TrackedObject right;

    private Vector3 prevHeadset;
    private Vector3 prevLeft;
    private Vector3 prevRight;
    
    public float CapVelocity = .1f;
    public const float MIN_SCALE = 0.01f;

    void Start()
    {
        SetPrevState();
    }

    void Update()
    {
        Vector3 headVel = headset.transform.position - prevHeadset;
        Vector3 leftVel = left.transform.position - prevLeft;
        Vector3 rightVel = right.transform.position - prevRight;

        // Vector3값의 magnitude는 float값임
        float totalVel = (1.5f * headVel.magnitude) + (.8f * leftVel.magnitude) + (.8f * rightVel.magnitude);

        // 핸들이나 카메라 아무것도 움직이지 않으면
        // totalVel이 0이어서 timeScale이 0이되므로 scene의 모든게 멈춘상태가 되야하는데
        // MIN_SCALE이 있으므로 0.01f속도로만 움직임.
        Time.timeScale = (totalVel / CapVelocity) + MIN_SCALE; // 작을수록 모든게 느려짐, 1이 default값
        SetPrevState();
    }

    void SetPrevState()
    {
        prevHeadset = headset.transform.position;
        prevLeft = left.transform.position;
        prevRight = right.transform.position;
    }
}
