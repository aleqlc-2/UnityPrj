using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Quantum;
using System;
using Codice.Client.Common;
using TMPro;

public unsafe class PlayerView : QuantumEntityViewComponent
{
	private static readonly int MoveX = Animator.StringToHash("moveX");
	private static readonly int MoveZ = Animator.StringToHash("moveZ");

    [SerializeField] private Animator animator;

	[SerializeField] private GameObject overheadUi;
	private bool _isLocalPlayer;
	private Renderer[] _renderers;
	[SerializeField] private TMP_Text username;
	[SerializeField, Range(1, 2)] private float animationSpeedMultiplier = 1;

	private void Awake()
	{
		_renderers = GetComponentsInChildren<Renderer>(true);
	}

	public override void OnUpdateView()
	{
		UpdateAnimator();
	}

	private void UpdateAnimator()
	{
		if (PredictedFrame.Exists(EntityRef) == false) return;
		var input = PredictedFrame.GetPlayerInput(PredictedFrame.Get<PlayerLink>(EntityRef).Player);

		var currentRotation = PredictedFrame.Get<Transform2D>(EntityRef).Rotation;
		var rotatedDirection = input->Direction.Rotate(-currentRotation).ToUnityVector2() * animationSpeedMultiplier;

		animator.SetFloat(MoveX, rotatedDirection.x);
		animator.SetFloat(MoveZ, rotatedDirection.y);
	}

	public override void OnActivate(Frame frame)
	{
		var playerLink = frame.Get<PlayerLink>(EntityRef);
		_isLocalPlayer = _game.PlayerIsLocal(playerLink.Player);
		var playerData = frame.GetPlayerData(playerLink.Player);
		username.text = playerData.PlayerNickname;
		var layer = UnityEngine.LayerMask.NameToLayer(_isLocalPlayer ? "Player_Local" : "Player_Remote");

		foreach (var renderer in _renderers)
		{
			renderer.gameObject.layer = layer;
			renderer.enabled = true;
		}

		overheadUi.SetActive(true);
		QuantumEvent.Subscribe<EventOnPlayerEnteredGrass>(this, OnPlayerEnteredGrass);
		QuantumEvent.Subscribe<EventOnPlayerExitGrass>(this, OnPlayerExitGrass);
	}
	
	public override void OnDeactivate()
	{
		QuantumEvent.UnsubscribeListener(this);
	}

	// 부쉬에 들어가면 다른 플레이어에게 안보이게
	private void OnPlayerEnteredGrass(EventOnPlayerEnteredGrass callback)
	{
		ToggleRendererVisibility(callback.Player, false);
	}

	// 부쉬에서 나오면 다른 플레이어에게 보임
	private void OnPlayerExitGrass(EventOnPlayerExitGrass callback)
	{
		ToggleRendererVisibility(callback.Player, true);
	}

	private void ToggleRendererVisibility(PlayerRef player, bool visible)
	{
		if (player != PredictedFrame.Get<PlayerLink>(EntityRef).Player) return;
		if (_isLocalPlayer) return;

		foreach (var renderer in _renderers)
		{
			renderer.enabled = visible;
		}

		overheadUi.SetActive(visible);
	}	
}
