using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlateKitchenObject : KitchenObject
{
	public event EventHandler<OnIngredientAddedEventArgs> OnIngredientAdded; // 재료가 접시에 더해졌을때 Invoke할 이벤트
	public class OnIngredientAddedEventArgs : EventArgs
	{
		public KitchenObjectSO kitchenObjectSO;
	}

	[SerializeField] private List<KitchenObjectSO> validKitchenObjectSOList; // 재료로 올라갈수 있는 오브젝트들의 스크립터블 오브젝트

    private List<KitchenObjectSO> kitchenObjectSOList; // 접시에 추가된 재료를 넣어놓은 list

	protected override void Awake() // KitchenObject를 상속했으므로 override해야함
	{
		base.Awake(); // KitchenObject의 Awake실행
		kitchenObjectSOList = new List<KitchenObjectSO>();
	}

	public bool TryAddIngredient(KitchenObjectSO kitchenObjectSO) // 재료더하기 시도
    {
		if (!validKitchenObjectSOList.Contains(kitchenObjectSO)) // 재료로 올라갈수 없는 오브젝트일때
		{
			return false; // false 리턴
		}

		if (kitchenObjectSOList.Contains(kitchenObjectSO)) // 재료로 올라갈수 있는데 이미 추가된 재료면
		{
			return false; // false 리턴
		}
		else // 재료로 올라갈수 있는데 추가안된 재료면
		{
			AddIngredientServerRpc(KitchenGameMultiplayer.Instance.GetKitchenObjectSOIndex(kitchenObjectSO));
			
			return true; // true 리턴
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
		kitchenObjectSOList.Add(kitchenObjectSO); // ClearCounter에 있던 오브젝트의 스크립터블오브젝트를 리스트에 넣고
		OnIngredientAdded?.Invoke(this, new OnIngredientAddedEventArgs { kitchenObjectSO = kitchenObjectSO });
	}

	public List<KitchenObjectSO> GetKitchenObjectSOList()
	{
		return kitchenObjectSOList;
	}
}
