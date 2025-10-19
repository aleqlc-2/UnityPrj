using System;
using Unity.Netcode;
using UnityEngine;

// �� Class�� ����� ��ũ��Ʈ�� �޸� ������Ʈ�� BaseCounter�� �޸��ſ� ����. �� ������Ʈ���� counterTopPoint �����Ϳ��� �Ҵ�
public class BaseCounter : NetworkBehaviour, IKitchenObjectParent
{
	public static event EventHandler OnAnyObjectPlacedHere;

	[SerializeField] private Transform counterTopPoint;
	private KitchenObject kitchenObject; // protected�� �ϸ� �� Ŭ������ ��ӹ��� Ŭ�������� ����������� ��밡��

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
