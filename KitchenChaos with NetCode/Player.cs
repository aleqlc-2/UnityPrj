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
	// [SerializeField] private GameInput gameInput; // �������� Network Manager���� Start HostŬ���ϸ� ������. GameInput.Instance�� �̱������� �����ؾ���
	private Vector3 lastInteractDir;
	[SerializeField] private LayerMask countersLayerMask;
	[SerializeField] private LayerMask collisionsLayerMask;
	private BaseCounter selectedCounter;
	[SerializeField] private Transform kitchenObjectHoldPoint;
	private KitchenObject kitchenObject;
	[SerializeField] private PlayerVisual playerVisual;

	[SerializeField] private List<Vector3> spawnPositionList;

	public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged; // EventHandler�� ��������Ʈ
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

	// Ư�� Player�� �����ɶ����� �����Ǵ� �÷��̾� �����Ͽ� ȣ��Ʈ�� Ŭ���̾�Ʈ ��ο��Լ� ȣ��
	// Network Object�� �̱����� ���⼭ �Ҵ������ �ٸ�Ŭ�������� �̱���ȣ���Ҷ� �ο����ȶ�
	public override void OnNetworkSpawn()
	{
		if (IsOwner) // ȣ��Ʈ�� Ŭ��� �����϶��� true
		{
			LocalInstance = this;
		}

		// (int)OwnerClientId�� NetworkObject�� ���� ������Ʈ�� ������ ������� 0,1,2,3..������ �ö󰡹Ƿ�
		// ������ϰ� �ٽ� ������ �ε����� �ϳ� �� �þ�Ƿ� GetPlayerDataIndexFromClientId �Լ��� ���� ��Ȯ�� �ε����� �����´�
		transform.position = spawnPositionList[KitchenGameMultiplayer.Instance.GetPlayerDataIndexFromClientId(OwnerClientId)];
		OnAnyPlayerSpawned?.Invoke(this, EventArgs.Empty);

		// NetworkManager.Singleton.OnClientDisconnectCallback�� Netcode���� ����
		// IsServer�� ���� ȣ��Ʈ���Ը� �ݹ��� ����Ͽ� Ư�� �÷��̾ disconnect�ɶ� ȣ��Ʈ�� �ݹ��� �ϵ��� �Ѵ�
		if (IsServer)
		{
			NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
		}
	}

	private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
	{
		if (clientId == OwnerClientId && HasKitchenObject()) // disconnect�� �÷��̾�� ����ִ� ������Ʈ�� �ִ�ä�� disconnect�ƴٸ�
		{
			KitchenObject.DestroyKitchenObject(GetKitchenObject()); // ����ִ� ������Ʈ ������ �ı�
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

		// �տ� ��ü ������ X�� Z�������� �̵��������� üũ
		if (!canMove)
		{
			Vector3 moveDirX = new Vector3(moveDir.x, 0f, 0f).normalized;
			canMove = moveDir.x != 0 && !Physics.BoxCast(transform.position, Vector3.one * playerRadius, moveDirX, Quaternion.identity, moveSpeed * Time.deltaTime, collisionsLayerMask);
			if (canMove) // X�̵� �������� üũ
			{
				moveDir = moveDirX;
			}
			else // Z�̵� �������� üũ
			{
				Vector3 moveDirZ = new Vector3(0f, 0f, moveDir.z).normalized;
				canMove = moveDir.z != 0 && !Physics.BoxCast(transform.position, Vector3.one * playerRadius, moveDirZ, Quaternion.identity, moveSpeed * Time.deltaTime, collisionsLayerMask);
				if (canMove)
				{
					moveDir = moveDirZ;
				}
				else // ��,�� ��� �̵� �Ұ�
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
			if (raycastHit.transform.TryGetComponent(out BaseCounter baseCounter)) // �ش� ������Ʈ ã���� true ��ã���� false
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

	// �������̽��� NetworkObject�� ������ �޼���
	public NetworkObject GetNetworkObject()
	{
		return NetworkObject;
	}
}
