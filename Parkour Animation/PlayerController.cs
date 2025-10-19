using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    [SerializeField] private float rotationSpeed = 500f;
	public float RotationSpeed => rotationSpeed;

	[Header("Ground Check Settings")]
	[SerializeField] private float groundCheckRadius = 0.2f;
	[SerializeField] private Vector3 groundCheckOffset = new Vector3(0f, 0.1f, 0.07f);
	[SerializeField] private LayerMask groundLayer;
	private bool isGrounded;

	private Quaternion targetRotation;
	private float ySpeed;

	private CameraController cameraController;
	private Animator animator;
	private CharacterController characterController;

	private bool hasControl = true;
	public bool HasControl
	{
		get => hasControl;
		set => hasControl = value;
	}

	private EnvironmentScanner environmentScanner;

	public LedgeData LedgeData { get; set; }
	public bool IsHanging { get; set; }

	public bool IsOnLedge { get; set; }

	private Vector3 desiredMoveDir;
	private Vector3 moveDir;
	private Vector3 velocity;

	public bool InAction { get; private set; }

	private void Awake()
	{
		cameraController = Camera.main.GetComponent<CameraController>();
		animator = GetComponent<Animator>();
		characterController = GetComponent<CharacterController>();
		environmentScanner = GetComponent<EnvironmentScanner>();
	}

	private void Update()
	{
		float h = Input.GetAxis("Horizontal");
		float v = Input.GetAxis("Vertical");
		float moveAmount = Mathf.Clamp01(Mathf.Abs(h) + Mathf.Abs(v));

		var moveInput = new Vector3(h, 0f, v).normalized;

		// 예를들어 직진하고있는데 카메라의 방향이 바뀌면 그쪽으로 직진하기위해 cameraController.PlanarRotation를 곱함
		desiredMoveDir = cameraController.PlanarRotation * moveInput;
		moveDir = desiredMoveDir.normalized;

		if (!hasControl) return;
		if (IsHanging) return;

		velocity = Vector3.zero;
		GroundCheck();
		animator.SetBool("isGrounded", isGrounded);
		Debug.Log("IsGrounded = " + isGrounded);
		if (isGrounded)
		{
			ySpeed = -0.5f;
			velocity = desiredMoveDir * moveSpeed;

			environmentScanner.ObstacleLedgeCheck(desiredMoveDir, out LedgeData ledgeData);
			if (IsOnLedge)
			{
				LedgeData = ledgeData;
				LedgeMovement();
				Debug.Log("On Ledge");
			}
		}
		else
		{
			ySpeed += Physics.gravity.y * Time.deltaTime;
			velocity = transform.forward * moveSpeed / 2;
		}

		velocity.y = ySpeed;
		characterController.Move(velocity * Time.deltaTime);

		if (moveAmount > 0 && moveDir.magnitude > 0.2f)
		{
			targetRotation = Quaternion.LookRotation(desiredMoveDir); // 카메라가 바라보는방향으로 직진하면 캐릭터 그쪽으로 회전
		}

		transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

		animator.SetFloat("moveAmount", velocity.magnitude / moveSpeed, 0.2f, Time.deltaTime);
	}

	private void GroundCheck()
	{
		// transform.TransformPoint는 local을 world로
		isGrounded = Physics.CheckSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadius, groundLayer);
	}

	private void LedgeMovement()
	{
		float signedAngle = Vector3.SignedAngle(LedgeData.surfaceHit.normal, desiredMoveDir, Vector3.up);
		float angle = Mathf.Abs(signedAngle);

		if (Vector3.Angle(desiredMoveDir, transform.forward) >= 80)
		{
			velocity = Vector3.zero;
			return;
		}

		if (angle < 60)
		{
			velocity = Vector3.zero;
			moveDir = Vector3.zero;
		}
		else if (angle < 90)
		{
			var left = Vector3.Cross(Vector3.up, LedgeData.surfaceHit.normal);
			var dir = left * Mathf.Sign(signedAngle);

			velocity = velocity.magnitude * dir;
			moveDir = dir;
		}
	}

	public IEnumerator DoAction(string animName, MatchTargetParams matchParams = null, Quaternion targetRotation = new Quaternion(),
								bool rotate = false, float postDelay = 0f, bool mirror = false)
	{
		InAction = true;

		animator.SetBool("mirrorAction", mirror);
		animator.CrossFadeInFixedTime(animName, 0.2f);
		yield return null;
		var animState = animator.GetNextAnimatorStateInfo(0);
		if (!animState.IsName(animName))
			Debug.LogError("The parkour animation is wrong!");

		float rotateStartTime = (matchParams != null) ? matchParams.startTime : 0f;

		float timer = 0f;
		while (timer <= animState.length)
		{
			timer += Time.deltaTime;
			float normalizedTime = timer / animState.length;

			if (rotate && normalizedTime > rotateStartTime)
				transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

			if (matchParams != null)
				MatchTarget(matchParams);

			if (animator.IsInTransition(0) && timer > 0.5f) break;

			yield return null;
		}

		yield return new WaitForSeconds(postDelay);

		InAction = false;
	}

	public void SetControl(bool hasControl)
	{
		this.hasControl = hasControl;
		characterController.enabled = hasControl;

		if (!hasControl)
		{
			animator.SetFloat("moveAmount", 0f);
			targetRotation = transform.rotation;
		}
	}

	public void ResetTargetRotation()
	{
		targetRotation = transform.rotation;
	}

	public void EnableCharacterController(bool enabled)
	{
		characterController.enabled = enabled;
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color(0f, 1f, 0f, 0.5f);
		Gizmos.DrawSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadius);
	}

	private void MatchTarget(MatchTargetParams mp)
	{
		if (animator.isMatchingTarget) return;

		animator.MatchTarget(mp.pos, transform.rotation, mp.bodyPart, new MatchTargetWeightMask(mp.posWeight, 0), mp.startTime, mp.targetTime);
	}
}


public class MatchTargetParams
{
	public Vector3 pos;
	public AvatarTarget bodyPart;
	public Vector3 posWeight;
	public float startTime;
	public float targetTime;
}