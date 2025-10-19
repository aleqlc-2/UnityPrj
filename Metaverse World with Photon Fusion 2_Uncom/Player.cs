using UnityEngine;
using Fusion;
using Photon.Chat.Demo;

public class Player : NetworkBehaviour
{
	public float PlayerSpeed = 5.0f;
	public CharacterController _controller;
	public float RotationSpeed = 10f;
	public GameObject mainCamera;
	public float JumpForce = 6f;
	public float GravityValue = -9.81f;
	private Vector3 _velocity;
	private bool _jumpPressed;
	public Animator animator;
	private bool _wasGrounded;

	public GameObject cameraTargetObject;

	[Networked] public NetworkString<_16> NickName { get; set; }
	public NamePlate namePlate;

	public override void Spawned()
	{
		if (HasStateAuthority)
		{
			mainCamera = Camera.main.gameObject;
			mainCamera.GetComponent<CameraScript>().targetObject = cameraTargetObject;

			GameObject.Find("Scripts").GetComponent<NamePickGui>().player = this;

			PlayerData.NickName = $"Player{Random.Range(0, 10000)}";
			NickName = PlayerData.NickName;
		}

		namePlate.SetNickName(NickName.Value);
	}

	public void ChatPlayerNameSet(string chatPlayerName)
	{
		if (HasStateAuthority)
		{
			PlayerData.NickName = chatPlayerName;
			NickName = PlayerData.NickName;
			RPC_ChatPlayerNameSet();
		}
	}

	[Rpc]
	public void RPC_ChatPlayerNameSet()
	{
		namePlate.SetNickName(NickName.Value);
	}

	private void Awake()
	{
		_controller.enabled = false;
	}

	private void Start()
	{
		_controller.enabled = true;
	}

	private void Update()
	{
		if (Input.GetButtonDown("Jump"))
		{
			_jumpPressed = true;
		}
	}

	public override void FixedUpdateNetwork()
	{
		if (HasStateAuthority == false) return;

		float horizontalInput = Input.GetAxis("Horizontal");
		float verticalInput = Input.GetAxis("Vertical");

		if (horizontalInput != 0 || verticalInput != 0)
		{
			animator.SetBool("IsWalking", true);
		}
		else
		{
			animator.SetBool("IsWalking", false);
		}

		Quaternion cameraRotationY = Quaternion.Euler(0, mainCamera.transform.rotation.eulerAngles.y, 0); // move에 -를 붙이는대신 카메라각도를 이용한다
		Vector3 move = cameraRotationY * new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * Runner.DeltaTime * PlayerSpeed;
		

		if (move != Vector3.zero)
		{
			Quaternion targetRotation = Quaternion.LookRotation(move);
			transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, RotationSpeed * Runner.DeltaTime);
		}

		_velocity.y += GravityValue * Runner.DeltaTime; // 땅에 안닿았을때나 점프중일때 중력적용 떨어짐

		if (_controller.isGrounded)
		{
			_velocity = new Vector3(0, -1, 0); // 땅에 닿았을때는 고정값

			if (_wasGrounded && !_jumpPressed)
			{
				animator.SetBool("IsJump", false);
			}
		}
		else
		{
			animator.SetBool("IsJump", true);
		}

		if (_jumpPressed && _controller.isGrounded)
		{
			_velocity.y += JumpForce;
		}

		_controller.Move(move + _velocity * Runner.DeltaTime);
		_wasGrounded = _controller.isGrounded;
		_jumpPressed = false;
	}
}
