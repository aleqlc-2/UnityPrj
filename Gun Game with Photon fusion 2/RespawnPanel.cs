using Fusion;
using TMPro;
using UnityEngine;

public class RespawnPanel : NetworkBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private TextMeshProUGUI respawnAmountText;
    [SerializeField] private GameObject childObj;

	public override void Spawned()
	{
		Runner.SetIsSimulated(Object ,true);
	}

	public override void FixedUpdateNetwork()
	{
		if (Utils.IsLocalPlayer(Object)) // 죽은 플레이어에게만 보이도록
		{
			var timerIsRunning = playerController.RespawnTimer.IsRunning;
			childObj.SetActive(timerIsRunning); // 5초동안 UI활성화

			if (timerIsRunning && playerController.RespawnTimer.RemainingTime(Runner).HasValue) // 아직 5초가 안됬다면
			{
				var time = playerController.RespawnTimer.RemainingTime(Runner).Value; // 남은시간 가져와서
				var roundInt = Mathf.RoundToInt(time); // 올림해서
				respawnAmountText.text = roundInt.ToString(); // 리스폰까지 남은시간을 UI에 보여준다
			}
		}
	}
}
