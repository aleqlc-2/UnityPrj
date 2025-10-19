using System;
using Unity.Netcode;
using UnityEngine;

// 이 Class를 상속한 스크립트가 달린 오브젝트에 BaseCounter가 달린거와 같음. 그 오브젝트에서 counterTopPoint 에디터에서 할당
public class BaseCounter : NetworkBehaviour, IKitchenObjectParent
{
	public static event EventHandler OnAnyObjectPlacedHere;

	[SerializeField] private Transform counterTopPoint;
	private KitchenObject kitchenObject; // protected로 하면 이 클래스를 상속받은 클래스에서 변수선언없이 사용가능

	public virtual void Interact(Player player)
    {

    }

	public virtual void InteractAlternate(Player player)
	{

	}

	public void SetKitchenObject(KitchenObject kitchenObject)
	{
		this.kitchenObject = kitchenObject;

		if (kitchenObject != null)
		{
			OnAnyObjectPlacedHere?.Invoke(this, EventArgs.Empty);
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

	public Transform GetKitchenObjectFollowTransform()
	{
		return counterTopPoint;
	}

	public static void ResetStaticData()
	{
		OnAnyObjectPlacedHere = null;
	}

	public NetworkObject GetNetworkObject()
	{
		return NetworkObject;
	}
}
