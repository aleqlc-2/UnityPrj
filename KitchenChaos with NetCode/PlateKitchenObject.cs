using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlateKitchenObject : KitchenObject
{
	public event EventHandler<OnIngredientAddedEventArgs> OnIngredientAdded; // ��ᰡ ���ÿ� ���������� Invoke�� �̺�Ʈ
	public class OnIngredientAddedEventArgs : EventArgs
	{
		public KitchenObjectSO kitchenObjectSO;
	}

	[SerializeField] private List<KitchenObjectSO> validKitchenObjectSOList; // ���� �ö󰥼� �ִ� ������Ʈ���� ��ũ���ͺ� ������Ʈ

    private List<KitchenObjectSO> kitchenObjectSOList; // ���ÿ� �߰��� ��Ḧ �־���� list

	protected override void Awake() // KitchenObject�� ��������Ƿ� override�ؾ���
	{
		base.Awake(); // KitchenObject�� Awake����
		kitchenObjectSOList = new List<KitchenObjectSO>();
	}

	public bool TryAddIngredient(KitchenObjectSO kitchenObjectSO) // �����ϱ� �õ�
    {
		if (!validKitchenObjectSOList.Contains(kitchenObjectSO)) // ���� �ö󰥼� ���� ������Ʈ�϶�
		{
			return false; // false ����
		}

		if (kitchenObjectSOList.Contains(kitchenObjectSO)) // ���� �ö󰥼� �ִµ� �̹� �߰��� ����
		{
			return false; // false ����
		}
		else // ���� �ö󰥼� �ִµ� �߰��ȵ� ����
		{
			AddIngredientServerRpc(KitchenGameMultiplayer.Instance.GetKitchenObjectSOIndex(kitchenObjectSO));
			
			return true; // true ����
		}
	}

	[ServerRpc(RequireOwnership = false)]
	private void AddIngredientServerRpc(int kitchenObjectSOIndex)
	{
		AddIngredientClientRpc(kitchenObjectSOIndex);
	}

	[ClientRpc]
	private void AddIngredientClientRpc(int kitchenObjectSOIndex)
	{
		KitchenObjectSO kitchenObjectSO = KitchenGameMultiplayer.Instance.GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);
		kitchenObjectSOList.Add(kitchenObjectSO); // ClearCounter�� �ִ� ������Ʈ�� ��ũ���ͺ������Ʈ�� ����Ʈ�� �ְ�
		OnIngredientAdded?.Invoke(this, new OnIngredientAddedEventArgs { kitchenObjectSO = kitchenObjectSO });
	}

	public List<KitchenObjectSO> GetKitchenObjectSOList()
	{
		return kitchenObjectSOList;
	}
}
