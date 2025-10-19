using System;
using Unity.Netcode;
using UnityEngine;

public class TrashCounter : BaseCounter
{
	public static event EventHandler OnAnyObjectTrashed;

	public override void Interact(Player player)
	{
		if (player.HasKitchenObject())
		{
			KitchenObject.DestroyKitchenObject(player.GetKitchenObject());
			InteractLogicServerRpc();
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
		OnAnyObjectTrashed?.Invoke(this, EventArgs.Empty);
	}

	new public static void ResetStaticData() // 부모클래스 BaseCounter에 같은이름의 static메서드가 있으므로 new붙여줘야함
	{
		OnAnyObjectTrashed = null;
	}
}
