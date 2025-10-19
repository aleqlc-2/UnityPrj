using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float rotationSpeed = 500f;

    [Header("Ground Check Settings")]
    [SerializeField] float groundCheckRadius = 0.2f;
    [SerializeField] Vector3 groundCheckOffset;
    [SerializeField] LayerMask groundLayer;

    bool isGrounded;

    float ySpeed;
    Quaternion targetRotation;

    public Vector3 InputDir { get; private set; }

    CameraController cameraController;
    Animator animator;
    CharacterController characterController;
    MeeleFighter meeleFighter;

    private CombatController combatController;

	public static PlayerController i { get; private set; }

	private void Awake()
    {
        cameraController =  Camera.main.GetComponent<CameraController>();
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
		meeleFighter = GetComponent<MeeleFighter>();
        combatController = GetComponent<CombatController>();

	}

    private void Update()
    {
        if (meeleFighter.InAction || meeleFighter.Health <= 0)
        {
            targetRotation = transform.rotation;
            animator.SetFloat("forwardSpeed", 0f); // ���ݾִϸ��̼� �����߿��� �̵��ִϸ��̼� ����
			return; // ���ݾִϸ��̼� �����߿��� �̵��Ұ�
		}
            
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        float moveAmount = Mathf.Clamp01(Mathf.Abs(h) + Mathf.Abs(v));

        var moveInput = (new Vector3(h, 0, v)).normalized;
        var moveDir = cameraController.PlanarRotation * moveInput; // ī�޶� �����ִ����� ���̵ǰ��ϱ����� cameraController.PlanarRotation�� ����
        InputDir = moveDir;

		GroundCheck();
        if (isGrounded)
        {
            ySpeed = -0.5f;
        }
        else
        {
            ySpeed += Physics.gravity.y * Time.deltaTime;
        }

        var velocity = moveDir * moveSpeed;
        
        if (combatController.CombatMode)
        {
            velocity /= 4f; // ������忡�� �̵��ӵ� ����

            var targetVec = combatController.TargetEnemy.transform.position - transform.position;
            targetVec.y = 0;

			if (moveAmount > 0)
			{
				targetRotation = Quaternion.LookRotation(moveDir);
				transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
			}

			// Dot(a,b) = |a||b|cos(a,b�� ������ ũ��)(|forward vector| = 1�̹Ƿ� Dot(velocity,forward vector) = |v|cos)
			// forward speed = velocity �ڻ���, sideward speed = velocity ����
			float forwardSpeed = Vector3.Dot(velocity, transform.forward);
			animator.SetFloat("forwardSpeed", forwardSpeed / moveSpeed, 0.2f, Time.deltaTime);

			float angle = Vector3.SignedAngle(transform.forward, velocity, Vector3.up);
			float strafeSpeed = Mathf.Sin(angle * Mathf.Deg2Rad);
			animator.SetFloat("strafeSpeed", strafeSpeed, 0.2f, Time.deltaTime);
		}
        else
        {
			if (moveAmount > 0)
			{
				targetRotation = Quaternion.LookRotation(moveDir);
			}

			transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

			animator.SetFloat("forwardSpeed", moveAmount, 0.2f, Time.deltaTime);
		}

		velocity.y = ySpeed;
		characterController.Move(velocity * Time.deltaTime);
	}

    void GroundCheck()
    {
        // �־��� ���� �ش��ϴ� Sphere������ �ݶ��̴��� ������ true��ȯ
        isGrounded = Physics.CheckSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadius, groundLayer);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 0, 0.5f);
        Gizmos.DrawSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadius);
    }

    public Vector3 GetIntentDirection()
    {
        return InputDir != Vector3.zero ? InputDir : transform.forward;
    }
}
