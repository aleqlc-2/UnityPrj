using System;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class GameHUD : NetworkBehaviour
{
    [SerializeField] private TMP_Text lobbyCodeText;

    private NetworkVariable<FixedString32Bytes> lobbyCode = new NetworkVariable<FixedString32Bytes>("");

	public override void OnNetworkSpawn()
	{
        if (IsClient)
        {
            lobbyCode.OnValueChanged += HandleLobbyCodeChanged;
        }

        if (!IsHost) return;

        lobbyCode.Value = HostSingleton.Instance.GameManager.JoinCode;
	}

	public override void OnNetworkDespawn()
	{
		if (IsClient)
		{
			lobbyCode.OnValueChanged -= HandleLobbyCodeChanged;
			HandleLobbyCodeChanged(string.Empty, lobbyCode.Value);
		}
	}

	public void LeaveGame() // �÷��̾� ������
    {
        if (NetworkManager.Singleton.IsHost)
        {
            HostSingleton.Instance.GameManager.Shutdown(); // ȣ��Ʈ��
        }

        ClientSingleton.Instance.GameManager.Disconnect(); // Ŭ����
    }
	
	// ȭ�� ���ϴܿ� �����ڵ� ���
	private void HandleLobbyCodeChanged(FixedString32Bytes oldCode, FixedString32Bytes newCode)
	{
		lobbyCodeText.text = newCode.ToString();
	}
}
