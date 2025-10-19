using Quantum;
using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CountdownUI : QuantumSceneViewComponent
{
    [SerializeField] private TMP_Text timeRemainingText;
    [SerializeField] private Image timeProgressImage;
	[SerializeField] private TMP_Text gameStateText;

	private QuantumRunner _quantumRunner;

	public override void OnActivate(Frame frame)
	{
		QuantumEvent.Subscribe<EventShrinkingCircleChangedState>(this, ShrinkingCircleChangedState);
		if (PredictedFrame.GetSingleton<GameManager>().CurrentGameState == GameState.WaitingForPlayer)
		{
			gameStateText.SetText("Waiting for players!");
		}

		_quantumRunner = QuantumRunner.Default;
	}

	private void ShrinkingCircleChangedState(EventShrinkingCircleChangedState callback)
	{
		gameStateText.SetText(ShrinkingCircleStateToMessage());
	}

	public override void OnUpdateView()
	{
		var f = PredictedFrame;
		var gameManager = PredictedFrame.GetSingleton<GameManager>();

		if (gameManager.CurrentGameState == GameState.WaitingForPlayer)
		{
			var data = f.FindAsset<GameManagerConfig>(gameManager.GameManagerConfig);
			var time = gameManager.TimeToWaitForPlayers.AsFloat;
			timeRemainingText.SetText(time.ToString("F2", CultureInfo.InvariantCulture));
			timeProgressImage.fillAmount = time / data.TimeToWaitForPlayers.AsFloat;
		}
		else
		{
			DisableRoomJoining();

			var shrinkingCircle = f.GetSingleton<ShrinkingCircle>();
			var time = MathF.Max(0, shrinkingCircle.CurrentTimeToNextState.AsFloat);
			var currentState = shrinkingCircle.CurrentState;
			timeRemainingText.text = time.ToString("F2", CultureInfo.InvariantCulture);
			timeProgressImage.fillAmount = (time / currentState.TimeToNextState.AsFloat);
		}
	}

	private void DisableRoomJoining()
	{
		if (_quantumRunner.NetworkClient == null) return;
		if (_quantumRunner.NetworkClient.CurrentRoom.IsVisible == false) return;
		_quantumRunner.NetworkClient.CurrentRoom.IsVisible = false;
	}

	private string ShrinkingCircleStateToMessage()
	{
		var shrinkingCircleState = PredictedFrame.GetSingleton<ShrinkingCircle>().CurrentState.CircleStateUnion.Field;

		switch (shrinkingCircleState)
		{
			case CircleStateUnion.PRESHRINKSTATE:
				return "Go to the safe area!";

			case CircleStateUnion.SHRINKSTATE:
				return "Area shrinking!";

			case CircleStateUnion.COOLDOWNSTATE:
				return "Cooldown!";

			case CircleStateUnion.INITIALSTATE:
				return "Go to the safe area!";

			default: throw new ArgumentOutOfRangeException("Unknown state!");
		}
	}
}
