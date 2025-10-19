using System;
using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class Player : NetworkBehaviour, IKitchenObjectParent
{
	public static Player LocalInstance { get; private set; }

	public static event EventHandler OnAnyPlayerSpawned;
	public static event EventHandler OnAnyPickedSomething;

	[SerializeField] private float moveSpeed = 7f;
	private float rotateSpeed = 10f;
	private bool isWalking = false;
	// [SerializeField] private GameInput gameInput; // 에디터의 Network Manager에서 Start Host클릭하면 에러뜸. GameInput.Instance로 싱글턴으로 접근해야함
	private Vector3 lastInteractDir;
	[SerializeField] private LayerMask countersLayerMask;
	[SerializeField] private LayerMask collisionsLayerMask;
	private BaseCounter selectedCounter;
	[SerializeField] private Transform kitchenObjectHoldPoint;
	private KitchenObject kitchenObject;
	[SerializeField] private PlayerVisual playerVisual;

	[SerializeField] private List<Vector3> spawnPositionList;

	public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged; // EventHandler가 델리게이트
	public class OnSelectedCounterChangedEventArgs : EventArgs
	{
		public BaseCounter selectedCounter;
	}

	public event EventHandler OnPickedSomething;

	private void Start()
	{
		GameInput.Instance.OnInteractAction += GameInput_OnInteractAction;
		GameInput.Instance.OnInteractAlternateAction += GameInput_OnInteractAlternateAction;

		PlayerData playerData = KitchenGameMultiplayer.Instance.GetPlayerDataFromClientId(OwnerClientId);
		playerVisual.SetPlayerColor(KitchenGameMultiplayer.Instance.GetPlayerColor(playerData.colorId));
	}

	private void Update()
	{
		if (!IsOwner)
		{
			return;
		}

		HandleMovement();
		HandleInteractions();
	}

	// 특정 Player가 생성될때마다 생성되는 플레이어 포함하여 호스트와 클라이언트 모두에게서 호출
	// Network Object는 싱글턴을 여기서 할당해줘야 다른클래스에서 싱글턴호출할때 널에러안뜸
	public override void OnNetworkSpawn()
	{
		if (IsOwner) // 호스트건 클라건 본인일때만 true
		{
			LocalInstance = this;
		}

		// (int)OwnerClientId는 NetworkObject를 가진 오브젝트가 생성된 순서대로 0,1,2,3..식으로 올라가므로
		// 강퇴당하고 다시 들어오면 인덱스가 하나 더 늘어나므로 GetPlayerDataIndexFromClientId 함수를 통해 정확한 인덱스를 가져온다
		transform.position = spawnPositionList[KitchenGameMultiplayer.Instance.GetPlayerDataIndexFromClientId(OwnerClientId)];
		OnAnyPlayerSpawned?.Invoke(this, EventArgs.Empty);

		// NetworkManager.Singleton.OnClientDisconnectCallback는 Netcode에서 제공
		// IsServer를 통해 호스트에게만 콜백을 등록하여 특정 플레이어가 disconnect될때 호스트만 콜백을 하도록 한다
		if (IsServer)
		{
			NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
		}
	}

	private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
	{
		if (clientId == OwnerClientId && HasKitchenObject()) // disconnect된 플레이어고 들고있는 오브젝트가 있는채로 disconnect됐다면
		{
			KitchenObject.DestroyKitchenObject(GetKitchenObject()); // 들고있던 오브젝트 서버가 파괴
		}
	}

	private void GameInput_OnInteractAction(object sender, System.EventArgs e)
	{
		if (!KitchenGameManager.Instance.IsGamePlaying()) return;

		if (selectedCounter != null)
		{
			selectedCounter.Interact(this);
		}
	}

	private void GameInput_OnInteractAlternateAction(object sender, EventArgs e)
	{
		if (!KitchenGameManager.Instance.IsGamePlaying()) return;

		if (selectedCounter != null)
		{
			selectedCounter.InteractAlternate(this);
		}
	}
	
	public bool IsWalking()
	{
		return isWalking;
	}

	private void HandleMovement()
	{
		Vector2 inputVector = GameInput.Instance.GetMovementVectorNormalized();
		Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

		float playerRadius = 0.7f;
		bool canMove = !Physics.BoxCast(transform.position, Vector3.one * playerRadius, moveDir, Quaternion.identity, moveSpeed * Time.deltaTime, collisionsLayerMask);

		if (canMove)
		{
			transform.position += moveDir * moveSpeed * Time.deltaTime;
		}

		// 앞에 물체 있으면 X나 Z방향으로 이동가능한지 체크
		if (!canMove)
		{
			Vector3 moveDirX = new Vector3(moveDir.x, 0f, 0f).normalized;
			canMove = moveDir.x != 0 && !Physics.BoxCast(transform.position, Vector3.one * playerRadius, moveDirX, Quaternion.identity, moveSpeed * Time.deltaTime, collisionsLayerMask);
			if (canMove) // X이동 가능한지 체크
			{
				moveDir = moveDirX;
			}
			else // Z이동 가능한지 체크
			{
				Vector3 moveDirZ = new Vector3(0f, 0f, moveDir.z).normalized;
				canMove = moveDir.z != 0 && !Physics.BoxCast(transform.position, Vector3.one * playerRadius, moveDirZ, Quaternion.identity, moveSpeed * Time.deltaTime, collisionsLayerMask);
				if (canMove)
				{
					moveDir = moveDirZ;
				}
				else // 앞,옆 모두 이동 불가
				{

				}
			}
		}

		isWalking = moveDir != Vector3.zero;

		transform.forward = Vector3.Slerp(transform.forward, moveDir, rotateSpeed * Time.deltaTime);
	}

	private void HandleInteractions()
	{
		Vector2 inputVector = GameInput.Instance.GetMovementVectorNormalized();
		Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);
		if (moveDir != Vector3.zero)
		{
			lastInteractDir = moveDir;
		}

		float interactDistance = 2f;
		if (Physics.Raycast(transform.position, lastInteractDir, out RaycastHit raycastHit, interactDistance, countersLayerMask))
		{
			if (raycastHit.transform.TryGetComponent(out BaseCounter baseCounter)) // 해당 컴포넌트 찾으면 true 못찾으면 false
			{
				// has ClearCounter
				if (baseCounter != selectedCounter)
				{
					SetSelectedCounter(baseCounter);
				}
			}
			else
			{
				SetSelectedCounter(null);
			}
		}
		else
		{
			SetSelectedCounter(null);
		}
	}

	private void SetSelectedCounter(BaseCounter clearCounter)
	{
		this.selectedCounter = clearCounter;
		OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs { selectedCounter = selectedCounter });
	}

	public Transform GetKitchenObjectFollowTransform()
	{
		return kitchenObjectHoldPoint;
	}

	public void SetKitchenObject(KitchenObject kitchenObject)
	{
		this.kitchenObject = kitchenObject;
		
		if (kitchenObject != null)
		{
			OnPickedSomething?.Invoke(this, EventArgs.Empty);
			OnAnyPickedSomething?.Invoke(this, EventArgs.Empty);
		}
	}

	public KitchenObject GetKitchenObject()
	{
		return kitchenObject;
	}

	public void ClearKitchenObject()
	{
		kitchenObject = null;
	}

	public bool HasKitchenObject()
	{
		return kitchenObject != null;
	}

	public static void ResetStaticData()
	{
		OnAnyPlayerSpawned = null;
	}

	// 인터페이스를 NetworkObject로 던지는 메서드
	public NetworkObject GetNetworkObject()
	{
		return NetworkObject;
	}
}
