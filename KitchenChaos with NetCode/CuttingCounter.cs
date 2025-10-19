using System;
using Unity.Netcode;
using UnityEngine;

public class CuttingCounter : BaseCounter, IHasProgress
{
	public static event EventHandler OnAnyCut;

	[SerializeField] private CuttingRecipeSO[] cuttingRecipeSOArray;

	private int cuttingProgress;

	public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;

	public event EventHandler OnCut;

	// CuttingCounter테이블 앞에서 E키 눌렀을때
	public override void Interact(Player player)
	{
		if (!HasKitchenObject()) // E눌렀을때 Cutting테이블위에는 오브젝트가없는데
		{
			if (player.HasKitchenObject()) // 플레이어가 오브젝트를 들고있으면
			{
				if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO())) // slice가능한 오브젝트만 CuttingCounter에 놓을수 있도록
				{
					KitchenObject kitchenObject = player.GetKitchenObject();
					player.GetKitchenObject().SetKitchenObjectParent(this); // Cutting테이블에 오브젝트를 놓는다, 테이블이 들고있을것이므로 this를 던짐
					InteractLogicPlaceObjectOnCounterServerRpc(); // 이전에 존재한 커팅이 있으면 호스트와 클라이언트에게 누적되었던 커팅회수를 초기화하기위해
				}
			}
			else // 플레이어가 오브젝트를 들고있지 않으면
			{

			}
		}
		else // Cutting테이블에 이미 오브젝트 있는데
		{
			if (player.HasKitchenObject()) // 플레이어가 오브젝트를 들고있을때
			{
				if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) // 그 오브젝트에 부착된 스크립트가 PlateKitchenObject이고(접시이고)
				{
					if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO())) // 재료로 올라갈수 있는 오브젝트라면
					{
						KitchenObject.DestroyKitchenObject(GetKitchenObject()); // ClearCounter에 있던 오브젝트 파괴
					}
				}
			}
			else // 플레이어가 오브젝트를 들고있지 않을때
			{
				GetKitchenObject().SetKitchenObjectParent(player); // 다시 플레이어가 오브젝트를 집는다, 플레이어가 들고있을것이므로 player를 던짐
			}
		}
	}

	[ServerRpc(RequireOwnership = false)]
	private void InteractLogicPlaceObjectOnCounterServerRpc()
	{
		InteractLogicPlaceObjectOnCounterClientRpc();
	}

	[ClientRpc]
	private void InteractLogicPlaceObjectOnCounterClientRpc()
	{
		cuttingProgress = 0;
		OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs { progressNormalized = 0f });
	}

	// CuttingCounter테이블 앞에서 F키 눌렀을때
	public override void InteractAlternate(Player player)
	{
		// HasRecipeWithInput(GetKitchenObject().GetKitchenOjbectSO()) 체크안하면 slice된거 앞에서 또 F누르면 에러뜸
		if (HasKitchenObject() && HasRecipeWithInput(GetKitchenObject().GetKitchenObjectSO())) // CuttingCounter테이블에 slice가능하며 아직 slice안된 오브젝트가 있으면
		{
			CutObjectServerRpc();
			TestCuttingProgressDoneServerRpc();
		}
	}

	[ServerRpc(RequireOwnership = false)]
	private void CutObjectServerRpc()
	{
		if (HasKitchenObject() && HasRecipeWithInput(GetKitchenObject().GetKitchenObjectSO())) // 체크 브로드캐스팅
		{
			CutObjectClientRpc();
		}
	}

	[ClientRpc]
	private void CutObjectClientRpc()
	{
		cuttingProgress++;
		OnCut?.Invoke(this, EventArgs.Empty);
		//Debug.Log(OnAnyCut.GetInvocationList().Length); // 게임씬에서 다른씬 갔다와도 InvocationList가 누적되는 문제발생
		OnAnyCut?.Invoke(this, EventArgs.Empty);
	}

	// 커팅횟수는 [ClientRpc]로 모든 클라이언트가 올렸는데 커팅횟수가 충족됐을때 모든 클라이언트가 파괴를 실행하면 널참조가 발생하므로 서버혼자 파괴한다
	[ServerRpc(RequireOwnership = false)]
	private void TestCuttingProgressDoneServerRpc()
	{
		if (HasKitchenObject() && HasRecipeWithInput(GetKitchenObject().GetKitchenObjectSO())) // 체크 브로드캐스팅
		{
			CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

			ShowProgressUIClientRpc();

			//OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs { progressNormalized = (float)cuttingProgress / cuttingRecipeSO.cuttingProgressMax });

			if (cuttingProgress >= cuttingRecipeSO.cuttingProgressMax) // 커팅횟수 충족되면
			{
				KitchenObjectSO outputKitchenObjectSO = GetOutputForInput(GetKitchenObject().GetKitchenObjectSO()); // slice된 오브젝트 가져와서
				KitchenObject.DestroyKitchenObject(GetKitchenObject()); // slice안된 오브젝트는 파괴하고
				KitchenObject.SpawnKitchenObject(outputKitchenObjectSO, this); // slice된 오브젝트를 테이블이 들고있을것이므로 this를 던짐
			}
		}
	}

	[ClientRpc]
	private void ShowProgressUIClientRpc()
	{
		CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());
		OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs { progressNormalized = (float)cuttingProgress / cuttingRecipeSO.cuttingProgressMax });
	}

	private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSO)
	{
		CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(inputKitchenObjectSO);

		return cuttingRecipeSO != null;
	}

	private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO)
	{
		CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(inputKitchenObjectSO);

		if (cuttingRecipeSO != null)
		{
			return cuttingRecipeSO.output;
		}
		else
		{
			return null;
		}
	}

	private CuttingRecipeSO GetCuttingRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO)
	{
		foreach (CuttingRecipeSO cuttingRecipeSO in cuttingRecipeSOArray)
		{
			if (cuttingRecipeSO.input == inputKitchenObjectSO)
			{
				return cuttingRecipeSO;
			}
		}

		return null;
	}

	new public static void ResetStaticData() // 부모클래스 BaseCounter에 같은이름의 static메서드가 있으므로 new붙여줘야함
	{
		OnAnyCut = null;
	}
}
