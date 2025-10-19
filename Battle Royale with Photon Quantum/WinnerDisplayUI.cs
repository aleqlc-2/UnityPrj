using Quantum;
using TMPro;
using UnityEngine;

public class WinnerDisplayUI : QuantumSceneViewComponent
{
	[SerializeField] private GameObject backgroundImage;
	[SerializeField] private TMP_Text winnerText;

	public override void OnActivate(Frame frame)
	{
		QuantumEvent.Subscribe<EventGameOver>(this, GameOver);
	}

	private void GameOver(EventGameOver callback)
	{
		var f = callback.Game.Frames.Predicted;
		var playerRef = f.Get<PlayerLink>(callback.Winner).Player;
		var playerData = f.GetPlayerData(playerRef);
		winnerText.gameObject.SetActive(true);
		winnerText.SetText($"Winner is {playerData.PlayerNickname}");
	}

	public override void OnDeactivate()
	{
		QuantumEvent.UnsubscribeListener(this);
	}
}
