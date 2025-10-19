using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DeliveryManager : NetworkBehaviour
{
	public static DeliveryManager Instance { get; private set; }

	[SerializeField] private RecipeListSO recipeListSO; // RecipeListSO��ũ��Ʈ�� �̿��� �����Ϳ��� ���� ��ũ���ͺ� ������Ʈ�� �Ҵ��ϴµ� �ڷ����� ��ũ��Ʈ

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
		// Host�� Server�� Client���� ������ �����Ѵ�.(Update, SpawnNewWaitingRecipeClientRpc �ڵ� ��� Host�� ����)
		if (!IsServer) return;

		// Host�� �ϼ��� ������ �������� Index�� �������� ����
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

	// Rpc�� ���� RecipeSO�� ������ �� ������ int�� �ε����� Ŭ���̾�Ʈ�� �����ϸ� Ŭ���̾�Ʈ���� �����Ǹ� �����Ѵ�
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
			RecipeSO waitingRecipeSO = waitingRecipeSOList[i]; // �ϼ��� ����

			// �ϼ��� ���Ŀ� �� ����� ���� = ���ÿ� ����ִ� ����� ����
			if (waitingRecipeSO.kitchenObjectSOList.Count == plateKitchenObject.GetKitchenObjectSOList().Count)
			{
				bool plateContentsMatchesRecipe = true;
				foreach (KitchenObjectSO recipeKitchenObjectSO in waitingRecipeSO.kitchenObjectSOList) // �ϼ��� ���Ŀ� �� ����
				{
					bool ingredientFound = false;
					foreach (KitchenObjectSO plateKitchenObjectSO in plateKitchenObject.GetKitchenObjectSOList()) // ���ÿ� ����ִ� ����߿�
					{
						if (plateKitchenObjectSO == recipeKitchenObjectSO) // ������ ������
						{
							ingredientFound = true;
							break; // ���� foreach�� break�ϰ� �ܺ� foreach�� �ٽõ�
						}
					}

					if (!ingredientFound) // ���ÿ� �ش�Ǵ� ��ᰡ ������
					{
						plateContentsMatchesRecipe = false;
					}
				}

				if (plateContentsMatchesRecipe) // ��� ��ᰡ ���ÿ� �����ϸ�
				{
					DeliverCorrectRecipeServerRpc(i); // Host�� ����
					return;
				}
			}
		}

		// �߸��� �������϶� Host�� ����
		DeliverIncorrectRecipeServerRpc();
	}

	// Host�� ����(Host�� ��Ȯ�� ������ �����Ҷ�)�ϰ� Server�� ��� Client���� ��ε�ĳ����
	// Client�� ��Ȯ�� �����Ǹ� ������������ �ùٸ��� �����ϰ� ���� RequireOwnership = false�� ������
	[ServerRpc(RequireOwnership = false)]
	private void DeliverCorrectRecipeServerRpc(int waitingRecipeSOListIndex)
	{
		DeliverCorrectRecipeClientRpc(waitingRecipeSOListIndex);
	}

	// ��� Ŭ���̾�Ʈ���� ��Ȯ�� ���������� ���޵ȴ�.
	[ClientRpc]
	private void DeliverCorrectRecipeClientRpc(int waitingRecipeSOListIndex)
	{
		Debug.Log("�÷��̾ ��Ȯ�� �����Ǹ� �����Խ��ϴ�.");
		successfulRecipesAmount++; // ��������� ������ ��������
		waitingRecipeSOList.RemoveAt(waitingRecipeSOListIndex); // �ش� ������ ��⸮��Ʈ���� ����
		OnRecipeCompleted?.Invoke(this, EventArgs.Empty);
		OnRecipeSuccess?.Invoke(this, EventArgs.Empty);
	}

	// Host�� ����(Host�� �߸��� ������ �����Ҷ�)�ϰ� Server�� ��� Client���� ��ε�ĳ����
	// Client�� �߸��� �����Ǹ� ������������ �ùٸ��� �����ϰ� ���� RequireOwnership = false�� ������
	[ServerRpc(RequireOwnership = false)]
	private void DeliverIncorrectRecipeServerRpc()
	{
		DeliverIncorrectRecipeClientRpc();
	}

	// ��� Ŭ���̾�Ʈ���� �߸��� ���������� ���޵ȴ�.
	[ClientRpc]
	private void DeliverIncorrectRecipeClientRpc()
	{
		Debug.Log("�÷��̾ �߸��� �����Ǹ� �����Խ��ϴ�.");
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
