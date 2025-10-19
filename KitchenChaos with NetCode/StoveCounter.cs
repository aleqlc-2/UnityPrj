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
		public State state; // 위에 선언한 enum의 객체
	}

	// 상속받은 IHasProgress인터페이스 멤버 구현하여 progressNormalized 사용가능
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
		// state가 변할때마다 Invoke, OnStateChangedEventArgs클래스의 state = 지역변수state
		OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = state.Value });

		if (state.Value == State.Idle || state.Value == State.Burned)
		{
			OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs { progressNormalized = 0f });
		}
	}

	private void Update()
	{
		if (!IsServer) return; // NetworkVariable<float> fryingTimer을 서버만 계산하도록

		if (HasKitchenObject()) // Stove테이블 위에 오브젝트가 올라갔고
		{
			switch (state.Value)
			{
				case State.Idle:
					break;

				case State.Frying:
					fryingTimer.Value += Time.deltaTime; // NetworkVariable<>는 .Value붙여줘야함

					if (fryingTimer.Value > fryingRecipeSO.fryingTimerMax) // 굽는시간이 충족되면
					{
						KitchenObject.DestroyKitchenObject(GetKitchenObject()); // Uncooked 오브젝트 파괴하고
						KitchenObject.SpawnKitchenObject(fryingRecipeSO.output, this); // Cooked 오브젝트 생성, 테이블이 들고있을것이므로 this를 던짐
						state.Value = State.Fried;
						burningTimer.Value = 0f;

						SetBurningRecipeSOClientRpc(KitchenGameMultiplayer.Instance.GetKitchenObjectSOIndex(GetKitchenObject().GetKitchenObjectSO()));
					}
					break;

				case State.Fried:
					burningTimer.Value += Time.deltaTime;

					OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs { progressNormalized = burningTimer.Value / burningRecipeSO.burningTimerMax });

					if (burningTimer.Value > burningRecipeSO.burningTimerMax) // 타는시간이 충족되면
					{
						KitchenObject.DestroyKitchenObject(GetKitchenObject()); // Cooked 오브젝트 파괴하고
						KitchenObject.SpawnKitchenObject(burningRecipeSO.output, this); // Burned 오브젝트 생성, 테이블이 들고있을것이므로 this를 던짐
						state.Value = State.Burned;
					}
					break;

				case State.Burned:
					break;
			}
		}
	}

	public override void Interact(Player player) // 오버라이드는 public
    {
		if (!HasKitchenObject()) // E눌렀을때 Stove테이블위에는 오브젝트가없는데
		{
			if (player.HasKitchenObject()) // 플레이어가 오브젝트를 들고있으면
			{
				if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO())) // fry가능한 오브젝트만 StoveCounter에 놓을수 있도록
				{
					KitchenObject kitchenObject = player.GetKitchenObject(); // 플레이어가 들고있던 Uncooked Meat를
					kitchenObject.SetKitchenObjectParent(this); // Stove테이블에 오브젝트를 놓는다, 테이블이 들고있을것이므로 this를 던짐

					InteractLogicPlaceObjectOnCounterServerRpc(KitchenGameMultiplayer.Instance.GetKitchenObjectSOIndex(kitchenObject.GetKitchenObjectSO()));
				}
			}
			else // 플레이어가 오브젝트를 들고있지 않으면
			{

			}
		}
		else // E눌렀을때 Stove테이블에 이미 오브젝트 있는데
		{
			if (player.HasKitchenObject()) // 플레이어가 오브젝트를 들고있을때
			{
				if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) // 그 오브젝트에 부착된 스크립트가 PlateKitchenObject이고(접시이고)
				{
					if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO())) // 재료로 올라갈수 있는 오브젝트라면 접시로 옮기고
					{
						KitchenObject.DestroyKitchenObject(GetKitchenObject()); // stoveCounter에 할당된 지역변수 null로

						SetStateIdleServerRpc();
					}
				}
			}
			else // 플레이어가 오브젝트를 들고있지 않을때
			{
				GetKitchenObject().SetKitchenObjectParent(player); // 다시 플레이어가 오브젝트를 집는다, 플레이어가 들고있을것이므로 player를 던짐
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
		fryingTimer.Value = 0f; // 굽는시간변수 초기화
		state.Value = State.Frying;
		SetFryingRecipeSOClientRpc(kitchenObjectSOIndex);
	}

	[ClientRpc]
	private void SetFryingRecipeSOClientRpc(int kitchenObjectSOIndex)
	{
		KitchenObjectSO kitchenObjectSO = KitchenGameMultiplayer.Instance.GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);
		fryingRecipeSO = GetFryingRecipeSOWithInput(kitchenObjectSO); // Stove에 올려진 오브젝트를 지역변수에 할당
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
