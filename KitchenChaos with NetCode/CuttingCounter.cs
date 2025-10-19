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

	// CuttingCounter���̺� �տ��� EŰ ��������
	public override void Interact(Player player)
	{
		if (!HasKitchenObject()) // E�������� Cutting���̺������� ������Ʈ�����µ�
		{
			if (player.HasKitchenObject()) // �÷��̾ ������Ʈ�� ���������
			{
				if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO())) // slice������ ������Ʈ�� CuttingCounter�� ������ �ֵ���
				{
					KitchenObject kitchenObject = player.GetKitchenObject();
					player.GetKitchenObject().SetKitchenObjectParent(this); // Cutting���̺� ������Ʈ�� ���´�, ���̺��� ����������̹Ƿ� this�� ����
					InteractLogicPlaceObjectOnCounterServerRpc(); // ������ ������ Ŀ���� ������ ȣ��Ʈ�� Ŭ���̾�Ʈ���� �����Ǿ��� Ŀ��ȸ���� �ʱ�ȭ�ϱ�����
				}
			}
			else // �÷��̾ ������Ʈ�� ������� ������
			{

			}
		}
		else // Cutting���̺� �̹� ������Ʈ �ִµ�
		{
			if (player.HasKitchenObject()) // �÷��̾ ������Ʈ�� ���������
			{
				if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) // �� ������Ʈ�� ������ ��ũ��Ʈ�� PlateKitchenObject�̰�(�����̰�)
				{
					if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO())) // ���� �ö󰥼� �ִ� ������Ʈ���
					{
						KitchenObject.DestroyKitchenObject(GetKitchenObject()); // ClearCounter�� �ִ� ������Ʈ �ı�
					}
				}
			}
			else // �÷��̾ ������Ʈ�� ������� ������
			{
				GetKitchenObject().SetKitchenObjectParent(player); // �ٽ� �÷��̾ ������Ʈ�� ���´�, �÷��̾ ����������̹Ƿ� player�� ����
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

	// CuttingCounter���̺� �տ��� FŰ ��������
	public override void InteractAlternate(Player player)
	{
		// HasRecipeWithInput(GetKitchenObject().GetKitchenOjbectSO()) üũ���ϸ� slice�Ȱ� �տ��� �� F������ ������
		if (HasKitchenObject() && HasRecipeWithInput(GetKitchenObject().GetKitchenObjectSO())) // CuttingCounter���̺� slice�����ϸ� ���� slice�ȵ� ������Ʈ�� ������
		{
			CutObjectServerRpc();
			TestCuttingProgressDoneServerRpc();
		}
	}

	[ServerRpc(RequireOwnership = false)]
	private void CutObjectServerRpc()
	{
		if (HasKitchenObject() && HasRecipeWithInput(GetKitchenObject().GetKitchenObjectSO())) // üũ ��ε�ĳ����
		{
			CutObjectClientRpc();
		}
	}

	[ClientRpc]
	private void CutObjectClientRpc()
	{
		cuttingProgress++;
		OnCut?.Invoke(this, EventArgs.Empty);
		//Debug.Log(OnAnyCut.GetInvocationList().Length); // ���Ӿ����� �ٸ��� ���ٿ͵� InvocationList�� �����Ǵ� �����߻�
		OnAnyCut?.Invoke(this, EventArgs.Empty);
	}

	// Ŀ��Ƚ���� [ClientRpc]�� ��� Ŭ���̾�Ʈ�� �÷ȴµ� Ŀ��Ƚ���� ���������� ��� Ŭ���̾�Ʈ�� �ı��� �����ϸ� �������� �߻��ϹǷ� ����ȥ�� �ı��Ѵ�
	[ServerRpc(RequireOwnership = false)]
	private void TestCuttingProgressDoneServerRpc()
	{
		if (HasKitchenObject() && HasRecipeWithInput(GetKitchenObject().GetKitchenObjectSO())) // üũ ��ε�ĳ����
		{
			CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

			ShowProgressUIClientRpc();

			//OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs { progressNormalized = (float)cuttingProgress / cuttingRecipeSO.cuttingProgressMax });

			if (cuttingProgress >= cuttingRecipeSO.cuttingProgressMax) // Ŀ��Ƚ�� �����Ǹ�
			{
				KitchenObjectSO outputKitchenObjectSO = GetOutputForInput(GetKitchenObject().GetKitchenObjectSO()); // slice�� ������Ʈ �����ͼ�
				KitchenObject.DestroyKitchenObject(GetKitchenObject()); // slice�ȵ� ������Ʈ�� �ı��ϰ�
				KitchenObject.SpawnKitchenObject(outputKitchenObjectSO, this); // slice�� ������Ʈ�� ���̺��� ����������̹Ƿ� this�� ����
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

	new public static void ResetStaticData() // �θ�Ŭ���� BaseCounter�� �����̸��� static�޼��尡 �����Ƿ� new�ٿ������
	{
		OnAnyCut = null;
	}
}
