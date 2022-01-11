using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallPlayer : MonoBehaviour
{
    [SerializeField] private float moveForce = 20f;
    [SerializeField] private float maxAngularVelocity = 25f;
    [SerializeField] private float jumpForce = 20f;
    [SerializeField] private float groundCheckRayLength = 1f;

    [SerializeField] private bool useTorque;

    private Rigidbody myBody;

    void Awake()
    {
        myBody = GetComponent<Rigidbody>();
        myBody.maxAngularVelocity = maxAngularVelocity; // 리지드바디의 최대 각속도. 빠르게 회전할 때 생기는 불안정함을 피하기위해
    }

    public void Move(Vector3 moveDirection)
    {
        if (useTorque)
        {
            // 드리프트하듯이 이동
            // x값 z값을 토글해서 던져야하는듯?
            Debug.Log(new Vector3(moveDirection.z, 0f, -moveDirection.x));
            myBody.AddTorque(new Vector3(moveDirection.z, 0f, -moveDirection.x) * moveForce);
        }
        else
        {
            // 그냥이동
            myBody.AddForce(moveDirection * moveForce);
        }
    }

    public void Jump(bool jump)
    {
        if (Physics.Raycast(transform.position, -Vector3.up, groundCheckRayLength) && jump)
        {
            myBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
}
