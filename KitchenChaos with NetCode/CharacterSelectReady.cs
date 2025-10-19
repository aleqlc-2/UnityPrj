using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

// SetPlayerReadyServerRpc�Լ� �����Ϸ��� NetworkBehaviour ����ؾ��ϰ�
// NetworkBehaviour����� ��ũ��Ʈ ������ ������Ʈ�� �����Ϳ��� Network Object��ũ��Ʈ �����ؾ���
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

	// �÷��̾� 1���� Ʃ�丮����¿��� EŰ ���������� ��� Ŭ���̾�Ʈ�� ����������� Ȯ��
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

		// ��� �÷��̾ ����Ǹ� ī��Ʈ�ٿ� ����
		if (allClientsReady)
		{
			KitchenGameLobby.Instance.DeleteLobby();
			Loader.LoadNetwork(Loader.Scene.GameScene);
		}

		Debug.Log("��� �÷��̾� �غ� : " + allClientsReady); // [ServerRpc(RequireOwnership = false)]�̹Ƿ� ���������� debug �߻�.
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
