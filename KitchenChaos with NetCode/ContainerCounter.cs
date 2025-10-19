using System;
using Unity.Netcode;
using UnityEngine;

public class ContainerCounter : BaseCounter
{
	[SerializeField] private KitchenObjectSO kitchenObjectSO; // ��ũ���ͺ�� ����� �����Ϳ��� �Ҵ�

	public event EventHandler OnPlayerGrabbedObject;

	public override void Interact(Player player)
	{
		if (!player.HasKitchenObject())
		{
			KitchenObject.SpawnKitchenObject(kitchenObjectSO, player); // player�� ����������̹Ƿ� player�� ����
			InteractLogicServerRpc(); // ContainerCounter ������ �ִϸ��̼� ��� Ŭ���̾�Ʈ���� ���̱�����
		}
	}

	[ServerRpc(RequireOwnership = false)]
	private void InteractLogicServerRpc()
	{
		InteractLogicClientRpc();
	}

	[ClientRpc]
	private void InteractLogicClientRpc()
	{
		OnPlayerGrabbedObject?.Invoke(this, EventArgs.Empty);
	}
}
