using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

// SetPlayerReadyServerRpc함수 동작하려면 NetworkBehaviour 상속해야하고
// NetworkBehaviour상속한 스크립트 부착된 오브젝트는 에디터에서 Network Object스크립트 부착해야함
public class CharacterSelectReady : NetworkBehaviour
{
	public static CharacterSelectReady Instance { get; private set; }

	private Dictionary<ulong, bool> playerReadyDictionary;

	public event EventHandler OnReadyChanged;

	private void Awake()
	{
		Instance = this;

		playerReadyDictionary = new Dictionary<ulong, bool>();
	}

	public void SetPlayerReady()
	{
		SetPlayerReadyServerRpc();
	}

	// 플레이어 1명이 튜토리얼상태에서 E키 누를때마다 모든 클라이언트가 레디상태인지 확인
	[ServerRpc(RequireOwnership = false)]
	private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
	{
		SetPlayerReadyClientRpc(serverRpcParams.Receive.SenderClientId);

		playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;
		bool allClientsReady = true;
		foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
		{
			if (!playerReadyDictionary.ContainsKey(clientId) || !playerReadyDictionary[clientId])
			{
				allClientsReady = false;
				break;
			}
		}

		// 모든 플레이어가 레디되면 카운트다운 시작
		if (allClientsReady)
		{
			KitchenGameLobby.Instance.DeleteLobby();
			Loader.LoadNetwork(Loader.Scene.GameScene);
		}

		Debug.Log("모든 플레이어 준비 : " + allClientsReady); // [ServerRpc(RequireOwnership = false)]이므로 서버에서만 debug 발생.
	}

	[ClientRpc]
	private void SetPlayerReadyClientRpc(ulong clientId)
	{
		playerReadyDictionary[clientId] = true;

		OnReadyChanged?.Invoke(this, EventArgs.Empty);
	}

	public bool IsPlayerReady(ulong clientId)
	{
		return playerReadyDictionary.ContainsKey(clientId) && playerReadyDictionary[clientId];
	}
}
