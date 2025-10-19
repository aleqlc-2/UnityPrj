using System;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;

public class StoveCounter : BaseCounter, IHasProgress
{
    [SerializeField] private FryingRecipeSO[] fryingRecipeSOArray;
	[SerializeField] private BurningRecipeSO[] burningRecipeSOArray;
	private NetworkVariable<float> fryingTimer = new NetworkVariable<float>(0f); // NetworkVariable<>
	private NetworkVariable<float> burningTimer = new NetworkVariable<float>(0f); // NetworkVariable<>
	private FryingRecipeSO fryingRecipeSO;
	private BurningRecipeSO burningRecipeSO;

	public enum State
	{
		Idle,
		Frying,
		Fried,
		Burned,
	}
	private NetworkVariable<State> state = new NetworkVariable<State>(State.Idle); // NetworkVariable<>

	public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
	public class OnStateChangedEventArgs : EventArgs
	{
		public State state; // ���� ������ enum�� ��ü
	}

	// ��ӹ��� IHasProgress�������̽� ��� �����Ͽ� progressNormalized ��밡��
	public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;

	public override void OnNetworkSpawn()
	{
		fryingTimer.OnValueChanged += FryingTimer_OnValueChanged;
		burningTimer.OnValueChanged += BurningTimer_OnValueChanged;
		state.OnValueChanged += State_OnValueChanged;
	}

	private void FryingTimer_OnValueChanged(float previousValue, float newValue)
	{
		float fryingTimerMax = fryingRecipeSO != null ? fryingRecipeSO.fryingTimerMax : 1f;
		OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs { progressNormalized = fryingTimer.Value / fryingTimerMax });
	}

	private void BurningTimer_OnValueChanged(float previousValue, float newValue)
	{
		float burningTimerMax = burningRecipeSO != null ? burningRecipeSO.burningTimerMax : 1f;
		OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs { progressNormalized = burningTimer.Value / burningTimerMax });
	}

	private void State_OnValueChanged(State previousState, State newState)
	{
		// state�� ���Ҷ����� Invoke, OnStateChangedEventArgsŬ������ state = ��������state
		OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = state.Value });

		if (state.Value == State.Idle || state.Value == State.Burned)
		{
			OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs { progressNormalized = 0f });
		}
	}

	private void Update()
	{
		if (!IsServer) return; // NetworkVariable<float> fryingTimer�� ������ ����ϵ���

		if (HasKitchenObject()) // Stove���̺� ���� ������Ʈ�� �ö󰬰�
		{
			switch (state.Value)
			{
				case State.Idle:
					break;

				case State.Frying:
					fryingTimer.Value += Time.deltaTime; // NetworkVariable<>�� .Value�ٿ������

					if (fryingTimer.Value > fryingRecipeSO.fryingTimerMax) // ���½ð��� �����Ǹ�
					{
						KitchenObject.DestroyKitchenObject(GetKitchenObject()); // Uncooked ������Ʈ �ı��ϰ�
						KitchenObject.SpawnKitchenObject(fryingRecipeSO.output, this); // Cooked ������Ʈ ����, ���̺��� ����������̹Ƿ� this�� ����
						state.Value = State.Fried;
						burningTimer.Value = 0f;

						SetBurningRecipeSOClientRpc(KitchenGameMultiplayer.Instance.GetKitchenObjectSOIndex(GetKitchenObject().GetKitchenObjectSO()));
					}
					break;

				case State.Fried:
					burningTimer.Value += Time.deltaTime;

					OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs { progressNormalized = burningTimer.Value / burningRecipeSO.burningTimerMax });

					if (burningTimer.Value > burningRecipeSO.burningTimerMax) // Ÿ�½ð��� �����Ǹ�
					{
						KitchenObject.DestroyKitchenObject(GetKitchenObject()); // Cooked ������Ʈ �ı��ϰ�
						KitchenObject.SpawnKitchenObject(burningRecipeSO.output, this); // Burned ������Ʈ ����, ���̺��� ����������̹Ƿ� this�� ����
						state.Value = State.Burned;
					}
					break;

				case State.Burned:
					break;
			}
		}
	}

	public override void Interact(Player player) // �������̵�� public
    {
		if (!HasKitchenObject()) // E�������� Stove���̺������� ������Ʈ�����µ�
		{
			if (player.HasKitchenObject()) // �÷��̾ ������Ʈ�� ���������
			{
				if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO())) // fry������ ������Ʈ�� StoveCounter�� ������ �ֵ���
				{
					KitchenObject kitchenObject = player.GetKitchenObject(); // �÷��̾ ����ִ� Uncooked Meat��
					kitchenObject.SetKitchenObjectParent(this); // Stove���̺� ������Ʈ�� ���´�, ���̺��� ����������̹Ƿ� this�� ����

					InteractLogicPlaceObjectOnCounterServerRpc(KitchenGameMultiplayer.Instance.GetKitchenObjectSOIndex(kitchenObject.GetKitchenObjectSO()));
				}
			}
			else // �÷��̾ ������Ʈ�� ������� ������
			{

			}
		}
		else // E�������� Stove���̺� �̹� ������Ʈ �ִµ�
		{
			if (player.HasKitchenObject()) // �÷��̾ ������Ʈ�� ���������
			{
				if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) // �� ������Ʈ�� ������ ��ũ��Ʈ�� PlateKitchenObject�̰�(�����̰�)
				{
					if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO())) // ���� �ö󰥼� �ִ� ������Ʈ��� ���÷� �ű��
					{
						KitchenObject.DestroyKitchenObject(GetKitchenObject()); // stoveCounter�� �Ҵ�� �������� null��

						SetStateIdleServerRpc();
					}
				}
			}
			else // �÷��̾ ������Ʈ�� ������� ������
			{
				GetKitchenObject().SetKitchenObjectParent(player); // �ٽ� �÷��̾ ������Ʈ�� ���´�, �÷��̾ ����������̹Ƿ� player�� ����
				SetStateIdleServerRpc();
			}
		}
	}

	[ServerRpc(RequireOwnership = false)]
	private void SetStateIdleServerRpc()
	{
		state.Value = State.Idle;
	}

	[ServerRpc(RequireOwnership = false)]
	private void InteractLogicPlaceObjectOnCounterServerRpc(int kitchenObjectSOIndex)
	{
		fryingTimer.Value = 0f; // ���½ð����� �ʱ�ȭ
		state.Value = State.Frying;
		SetFryingRecipeSOClientRpc(kitchenObjectSOIndex);
	}

	[ClientRpc]
	private void SetFryingRecipeSOClientRpc(int kitchenObjectSOIndex)
	{
		KitchenObjectSO kitchenObjectSO = KitchenGameMultiplayer.Instance.GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);
		fryingRecipeSO = GetFryingRecipeSOWithInput(kitchenObjectSO); // Stove�� �÷��� ������Ʈ�� ���������� �Ҵ�
	}

	[ClientRpc]
	private void SetBurningRecipeSOClientRpc(int kitchenObjectSOIndex)
	{
		KitchenObjectSO kitchenObjectSO = KitchenGameMultiplayer.Instance.GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);
		burningRecipeSO = GetBurningRecipeSOWithInput(kitchenObjectSO);
	}

	private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSO)
	{
		FryingRecipeSO fryingRecipeSO = GetFryingRecipeSOWithInput(inputKitchenObjectSO);

		return fryingRecipeSO != null;
	}

	private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO)
	{
		FryingRecipeSO fryingRecipeSO = GetFryingRecipeSOWithInput(inputKitchenObjectSO);

		if (fryingRecipeSO != null)
		{
			return fryingRecipeSO.output;
		}
		else
		{
			return null;
		}
	}

	private FryingRecipeSO GetFryingRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO)
	{
		foreach (FryingRecipeSO FryingRecipeSO in fryingRecipeSOArray)
		{
			if (FryingRecipeSO.input == inputKitchenObjectSO)
			{
				return FryingRecipeSO;
			}
		}

		return null;
	}

	private BurningRecipeSO GetBurningRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO)
	{
		foreach (BurningRecipeSO burningRecipeSO in burningRecipeSOArray)
		{
			if (burningRecipeSO.input == inputKitchenObjectSO)
			{
				return burningRecipeSO;
			}
		}

		return null;
	}

	public bool IsFried()
	{
		return state.Value == State.Fried;
	}
}
