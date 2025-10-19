using System;
using UnityEngine;

public class PlayerColorDisplay : MonoBehaviour
{
    [SerializeField] private TeamColourLookup teamColourLookup;
    [SerializeField] private TankPlayer player;
    [SerializeField] private SpriteRenderer[] playerSprites; // tank body, turret

	private void Start()
	{
		HandleTeamChanged(-1, player.TeamIndex.Value);

		player.TeamIndex.OnValueChanged += HandleTeamChanged;
	}

	private void OnDestroy()
	{
		player.TeamIndex.OnValueChanged -= HandleTeamChanged;
	}

	private void HandleTeamChanged(int oldTeamIndex, int newTeamIndex)
	{
		Color teamColor = teamColourLookup.GetTeamColor(newTeamIndex);
		foreach (SpriteRenderer sprite in playerSprites)
		{
			sprite.color = teamColor;
		}
	}
}
