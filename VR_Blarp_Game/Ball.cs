using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

[RequireComponent(typeof(Rigidbody))]
public class Ball : MonoBehaviour
{
    public SteamVR_TrackedObject rightController;
    public SteamVR_Action_Boolean grip;

    private Rigidbody rigidbody;

    private LineRenderer line;

    private float speed = 5f;

    //private SteamVR_Behaviour_Pose gripPose;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        line = GetComponent<LineRenderer>();
        //gripPose = GetComponent<SteamVR_Behaviour_Pose>();

        rightController = ControllerManager.Instance.rightController;
    }

    void Update()
    {
        if (grip.GetState(SteamVR_Input_Sources.RightHand))
        {
            float gripPress = 1; // random.range?

            // 컨트롤러쪽으로 오게끔 거리구함
            Vector3 direction = rightController.transform.position - transform.position;

            // 방향 * 스피드
            rigidbody.AddForce(direction.normalized * speed * gripPress); // gripPress 0~1
        }

        // if(grip.GetState(SteamVR_Input_Sources.RightHand))
        // {
        //     Vector3 direction = transform.position - rightController.transform.position;
        //     float lV1 = direction.magnitude;
        //     direction.Normalize();

        //     Vector3 velocity = gripPose.GetVelocity();
        //     float lVel = velocity.magnitude;
        //     float dot = Vector3.Dot(velocity, direction);

        //     float gripVal = 1; // 0 ~ 1
        //     direction = -1f * gripVal * direction * lVel * (-dot+ 1);
        //     rigidbody.AddForce(direction);

        //     SpringJoint sj = GetComponent<SpringJoint>();
        //     sj.spring = 1 * gripVal;
        // }
        // else
        // {
        //     SpringJoint sj = GetComponent<SpringJoint>();
        //     sj.spring = 0;
        // }

        line.SetPosition(0, transform.position);
        line.SetPosition(1, rightController.transform.position);
    }
}
