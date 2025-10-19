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

	public void LeaveGame() // 플레이어 떠날때
    {
        if (NetworkManager.Singleton.IsHost)
        {
            HostSingleton.Instance.GameManager.Shutdown(); // 호스트측
        }

        ClientSingleton.Instance.GameManager.Disconnect(); // 클라측
    }
	
	// 화면 좌하단에 입장코드 출력
	private void HandleLobbyCodeChanged(FixedString32Bytes oldCode, FixedString32Bytes newCode)
	{
		lobbyCodeText.text = newCode.ToString();
	}
}
