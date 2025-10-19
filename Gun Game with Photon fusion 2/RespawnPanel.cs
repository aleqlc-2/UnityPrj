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
		if (Utils.IsLocalPlayer(Object)) // ���� �÷��̾�Ը� ���̵���
		{
			var timerIsRunning = playerController.RespawnTimer.IsRunning;
			childObj.SetActive(timerIsRunning); // 5�ʵ��� UIȰ��ȭ

			if (timerIsRunning && playerController.RespawnTimer.RemainingTime(Runner).HasValue) // ���� 5�ʰ� �ȉ�ٸ�
			{
				var time = playerController.RespawnTimer.RemainingTime(Runner).Value; // �����ð� �����ͼ�
				var roundInt = Mathf.RoundToInt(time); // �ø��ؼ�
				respawnAmountText.text = roundInt.ToString(); // ���������� �����ð��� UI�� �����ش�
			}
		}
	}
}
