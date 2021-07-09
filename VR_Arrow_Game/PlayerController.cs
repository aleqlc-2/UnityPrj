using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem; // Player

public class PlayerController : MonoBehaviour
{
    public SteamVR_Action_Vector2 input;
    public float speed = 1;
    private CharacterController characterController;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (input.axis.magnitude > 0.1f)
        {
            Vector3 direction
            = Player.instance.hmdTransform.TransformDirection(new Vector3(input.axis.x, 0, input.axis.y));

            // 이렇게만쓰면 반대로 움직임
            // transform.position += speed * Time.deltaTime * new Vector3(input.axis.x, 0, input.axis.y);

            // 올바른 방향으로 움직이나 계단 못올라감
            // transform.position += speed * Time.deltaTime * Vector3.ProjectOnPlane(direction, Vector3.up);

            // 올바른 방향으로 움직이고 계단도 올라갈 수 있음
            characterController.Move(speed * Time.deltaTime * Vector3.ProjectOnPlane(direction, Vector3.up)
                                    - new Vector3(0, 9.81f, 0) * Time.deltaTime); // 중력 적용
        }
    }
}
