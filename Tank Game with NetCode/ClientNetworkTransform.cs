using Unity.Netcode.Components;

public class ClientNetworkTransform : NetworkTransform
{
	public override void OnNetworkSpawn()
	{
		base.OnNetworkSpawn();
		CanCommitToTransform = IsOwner;
	}

	protected override void Update()
	{
		base.CanCommitToTransform = IsOwner;
		base.Update();
		if (NetworkManager != null)
		{
			if (NetworkManager.IsConnectedClient || NetworkManager.IsListening)
			{
				if (base.CanCommitToTransform)
				{
					base.TryCommitTransformToServer(transform, NetworkManager.LocalTime.Time);
				}
			}
		}
	}

	protected override bool OnIsServerAuthoritative()
	{
		return false;
	}
}
