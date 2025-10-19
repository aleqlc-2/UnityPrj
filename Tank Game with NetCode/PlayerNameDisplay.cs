using System;
using TMPro;
using Unity.Collections;
using UnityEngine;

public class PlayerNameDisplay : MonoBehaviour
{
    [SerializeField] private TankPlayer player;
    [SerializeField] private TMP_Text playerNameText; // TMP_Text는 TextMeshPro와 TextMeshProUGUI의 부모클래스

	private void Start()
	{
		HandlePlayerNameChanged(string.Empty, player.PlayerName.Value);
		player.PlayerName.OnValueChanged += HandlePlayerNameChanged;
	}

	private void HandlePlayerNameChanged(FixedString32Bytes oldName, FixedString32Bytes newName)
	{
		playerNameText.text = newName.ToString();
	}

	private void OnDestroy()
	{
		player.PlayerName.OnValueChanged -= HandlePlayerNameChanged;
	}
}
