using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DeliveryManager : NetworkBehaviour
{
	public static DeliveryManager Instance { get; private set; }

	[SerializeField] private RecipeListSO recipeListSO; // RecipeListSO스크립트를 이용해 에디터에서 만든 스크립터블 오브젝트를 할당하는데 자료형이 스크립트

	private List<RecipeSO> waitingRecipeSOList;
	private float spawnRecipeTimer = 4f;
	private float spawnRecipeTimerMax = 4f;
	private int waitingRecipesMax = 4;

	public event EventHandler OnRecipeSpawned;
	public event EventHandler OnRecipeCompleted;
	public event EventHandler OnRecipeSuccess;
	public event EventHandler OnRecipeFailed;

	private int successfulRecipesAmount;

	private void Awake()
	{
		Instance = this;
		waitingRecipeSOList = new List<RecipeSO>();
	}

	private void Update()
	{
		// Host가 Server와 Client에게 양쪽을 접근한다.(Update, SpawnNewWaitingRecipeClientRpc 코드 모두 Host가 실행)
		if (!IsServer) return;

		// Host가 완성될 음식의 레시피의 Index를 랜덤으로 생성
		spawnRecipeTimer -= Time.deltaTime;
		if (spawnRecipeTimer <= 0f)
		{
			spawnRecipeTimer = spawnRecipeTimerMax;

			if (KitchenGameManager.Instance.IsGamePlaying() && waitingRecipeSOList.Count < waitingRecipesMax)
			{
				int waitingRecipeSOIndex = UnityEngine.Random.Range(0, recipeListSO.recipeSOList.Count);
				SpawnNewWaitingRecipeClientRpc(waitingRecipeSOIndex);
			}
		}
	}

	// Rpc를 통해 RecipeSO를 전달할 수 없으니 int로 인덱스만 클라이언트로 전달하면 클라이언트에서 레시피를 생성한다
	[ClientRpc]
	private void SpawnNewWaitingRecipeClientRpc(int waitingRecipeSOIndex)
	{
		RecipeSO waitingRecipeSO = recipeListSO.recipeSOList[waitingRecipeSOIndex];
		waitingRecipeSOList.Add(waitingRecipeSO);
		OnRecipeSpawned?.Invoke(this, EventArgs.Empty);
	}

	public void DeliverRecipe(PlateKitchenObject plateKitchenObject)
	{
		for (int i = 0; i < waitingRecipeSOList.Count; i++)
		{
			RecipeSO waitingRecipeSO = waitingRecipeSOList[i]; // 완성된 음식

			// 완성된 음식에 들어간 재료의 개수 = 접시에 들어있는 재료의 개수
			if (waitingRecipeSO.kitchenObjectSOList.Count == plateKitchenObject.GetKitchenObjectSOList().Count)
			{
				bool plateContentsMatchesRecipe = true;
				foreach (KitchenObjectSO recipeKitchenObjectSO in waitingRecipeSO.kitchenObjectSOList) // 완성된 음식에 들어간 재료와
				{
					bool ingredientFound = false;
					foreach (KitchenObjectSO plateKitchenObjectSO in plateKitchenObject.GetKitchenObjectSOList()) // 접시에 들어있는 재료중에
					{
						if (plateKitchenObjectSO == recipeKitchenObjectSO) // 같은게 있으면
						{
							ingredientFound = true;
							break; // 내부 foreach만 break하고 외부 foreach는 다시돔
						}
					}

					if (!ingredientFound) // 접시에 해당되는 재료가 없으면
					{
						plateContentsMatchesRecipe = false;
					}
				}

				if (plateContentsMatchesRecipe) // 모든 재료가 접시에 존재하면
				{
					DeliverCorrectRecipeServerRpc(i); // Host가 실행
					return;
				}
			}
		}

		// 잘못된 레시피일때 Host가 실행
		DeliverIncorrectRecipeServerRpc();
	}

	// Host가 실행(Host가 정확한 레시피 전달할때)하고 Server가 모든 Client에게 브로드캐스팅
	// Client가 정확한 레시피를 전달했을때도 올바르게 동작하게 위해 RequireOwnership = false를 적어줌
	[ServerRpc(RequireOwnership = false)]
	private void DeliverCorrectRecipeServerRpc(int waitingRecipeSOListIndex)
	{
		DeliverCorrectRecipeClientRpc(waitingRecipeSOListIndex);
	}

	// 모든 클라이언트에게 정확한 레시피임이 전달된다.
	[ClientRpc]
	private void DeliverCorrectRecipeClientRpc(int waitingRecipeSOListIndex)
	{
		Debug.Log("플레이어가 정확한 레시피를 가져왔습니다.");
		successfulRecipesAmount++; // 게임종료시 보여줄 성공개수
		waitingRecipeSOList.RemoveAt(waitingRecipeSOListIndex); // 해당 음식은 대기리스트에서 제거
		OnRecipeCompleted?.Invoke(this, EventArgs.Empty);
		OnRecipeSuccess?.Invoke(this, EventArgs.Empty);
	}

	// Host가 실행(Host가 잘못된 레시피 전달할때)하고 Server가 모든 Client에게 브로드캐스팅
	// Client가 잘못된 레시피를 전달했을때도 올바르게 동작하게 위해 RequireOwnership = false를 적어줌
	[ServerRpc(RequireOwnership = false)]
	private void DeliverIncorrectRecipeServerRpc()
	{
		DeliverIncorrectRecipeClientRpc();
	}

	// 모든 클라이언트에게 잘못된 레시피임이 전달된다.
	[ClientRpc]
	private void DeliverIncorrectRecipeClientRpc()
	{
		Debug.Log("플레이어가 잘못된 레시피를 가져왔습니다.");
		OnRecipeFailed?.Invoke(this, EventArgs.Empty);
	}

	public List<RecipeSO> GetWaitingRecipeSOList()
	{
		return waitingRecipeSOList;
	}

	public int GetSuccessfulRecipesAmount()
	{
		return successfulRecipesAmount;
	}
}
