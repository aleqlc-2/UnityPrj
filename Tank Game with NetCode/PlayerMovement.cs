using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader inputReader; // SO
    [SerializeField] private Transform bodyTransform;
    [SerializeField] private Rigidbody2D rb;
	
	[Header("Settings")]
	[SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float turningRate = 270f;
	
	private Vector2 previousMovementInput;
	private Vector3 previousPos;

	[SerializeField] private ParticleSystem dustCloud;
	[SerializeField] private float particleEmissionValue = 10;
	private ParticleSystem.EmissionModule emissionModule;
	private const float ParticleStopThreshold = 0.005f;

	private void Awake()
	{
		emissionModule = dustCloud.emission;
	}

	public override void OnNetworkSpawn()
	{
		if (!IsOwner) return; // 호스트건 클라건 각자 아래코드 실행

		inputReader.MoveEvent += HandleMove;
	}

	public override void OnNetworkDespawn()
	{
		if (!IsOwner) return; // 호스트건 클라건 각자 아래코드 실행

		inputReader.MoveEvent -= HandleMove;
	}

	private void FixedUpdate()
	{
		// ParticleStopThreshold 이상 움직이면 track파티클 시작
		if ((transform.position - previousPos).sqrMagnitude > ParticleStopThreshold)
		{
			emissionModule.rateOverTime = particleEmissionValue;
		}
		else
		{
			emissionModule.rateOverTime = 0;
		}

		previousPos = transform.position;

		if (!IsOwner) return; // 호스트건 클라건 각자 아래코드 실행

		// 위아래키 누를때 이동
		rb.linearVelocity = (Vector2)bodyTransform.up * previousMovementInput.y * movementSpeed; 
	}

	private void Update()
	{
		if (!IsOwner) return; // 호스트건 클라건 각자 아래코드 실행

		// 좌우키 누를때 바퀴회전
		float zRotation = previousMovementInput.x * -turningRate * Time.deltaTime;
		bodyTransform.Rotate(0f, 0f, zRotation);
	}

	private void HandleMove(Vector2 movementInput)
	{
		previousMovementInput = movementInput;
	}
}
