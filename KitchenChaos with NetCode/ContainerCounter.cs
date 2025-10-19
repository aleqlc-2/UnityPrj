using System;
using Unity.Netcode;
using UnityEngine;

public class ContainerCounter : BaseCounter
{
	[SerializeField] private KitchenObjectSO kitchenObjectSO; // 스크립터블로 만든거 에디터에서 할당

	public event EventHandler OnPlayerGrabbedObject;

	public override void Interact(Player player)
	{
		if (!player.HasKitchenObject())
		{
			KitchenObject.SpawnKitchenObject(kitchenObjectSO, player); // player가 들고있을것이므로 player를 던짐
			InteractLogicServerRpc(); // ContainerCounter 열리는 애니메이션 모든 클라이언트에게 보이기위해
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
