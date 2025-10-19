using System;
using Unity.Cinemachine;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

// Player프리펩에 부착되어 시네머신 카메라가 각각 플레이어를 따라가도록 분리해주는 스크립트
public class TankPlayer : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private CinemachineCamera virtualCamera;
	[SerializeField] private SpriteRenderer minimapIconRenderer;
	[SerializeField] private Texture2D crosshair;
	[field: SerializeField] public Health Health { get; private set; }
	[field: SerializeField] public CoinWallet Wallet { get; private set; }

    [Header("Settings")]
    [SerializeField] private int ownerPriority = 15;
	[SerializeField] private Color ownerColor;

	public NetworkVariable<FixedString32Bytes> PlayerName = new NetworkVariable<FixedString32Bytes>();
	public NetworkVariable<int> TeamIndex = new NetworkVariable<int>();

	public static event Action<TankPlayer> OnPlayerSpawned;
	public static event Action<TankPlayer> OnPlayerDespawned;

	public override void OnNetworkSpawn()
	{
		if (IsServer)
		{
			UserData userData = null;
			if (IsHost)
			{
				userData = HostSingleton.Instance.GameManager.NetworkServer.GetUserDataByClientId(OwnerClientId);
			}
			else
			{
				userData = ServerSingleton.Instance.GameManager.NetworkServer.GetUserDataByClientId(OwnerClientId);
			}
			
			PlayerName.Value = userData.userName;
			TeamIndex.Value = userData.teamIndex;

			OnPlayerSpawned?.Invoke(this);
		}

		if (IsOwner)
		{
			virtualCamera.Priority = ownerPriority;

			minimapIconRenderer.color = ownerColor; // 방장은 미니맵에 보이는 색깔 다르게

			Cursor.SetCursor(crosshair, new Vector2(crosshair.width / 2, crosshair.height / 2), CursorMode.Auto); // hotspot을 텍스쳐 중앙으로 설정(Aiming)
		}
	}

	public override void OnNetworkDespawn()
	{
		if (IsServer)
		{
			OnPlayerDespawned?.Invoke(this);
		}
	}
}
