using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementMotor : MonoBehaviour
{
    public float gravityMultiplier = 1f;
    public float lerpTime = 10f;

    private Vector3 moveDirection = Vector3.zero;
    private Vector3 targetDirection = Vector3.zero;
    private float fallVelocity = 0f;

    [HideInInspector] public CharacterController charController;

    public float distanceToGround = 0.1f;

    private bool isGrounded;

    private Collider myCollider;

    void Awake()
    {
        charController = GetComponent<CharacterController>();
        myCollider = GetComponent<Collider>(); // CharacterController가 자체적으로 캡슐콜라이더를 가지고있음.
    }

    void Start()
    {
        // extents는 콜라이더 size의 절반크기
        // 센터에서 절반만큼 내려가기 위하여
        distanceToGround = myCollider.bounds.extents.y;
    }

    void Update()
    {
        isGrounded = OnGroundCheck();

        moveDirection = Vector3.Lerp(moveDirection, targetDirection, Time.deltaTime * lerpTime);
        moveDirection.y = fallVelocity;

        charController.Move(moveDirection * Time.deltaTime);

        if (!isGrounded) fallVelocity -= 90f * gravityMultiplier * Time.deltaTime;
    }

    public bool OnGroundCheck()
    {
        RaycastHit hit;

        if (charController.isGrounded)
        {
            return true;
        }

        // center는 콜라이더의 중심값
        // distanceToGround + 0.1f는 센터에서 절반만큼 내려와서 0.1 더 내려가면 땅에 닿음
        if (Physics.Raycast(myCollider.bounds.center, -Vector3.up, out hit, distanceToGround + 0.1f))
        {
            return true;
        }

        return false;
    }

    public void Move(Vector3 dir)
    {
        targetDirection = dir;
    }

    public void Stop()
    {
        moveDirection = Vector3.zero;
        targetDirection = Vector3.zero;
    }

    public void Jump(float jumpSpeed)
    {
        if (isGrounded)
        {
            fallVelocity = jumpSpeed;
        }
    }
}
