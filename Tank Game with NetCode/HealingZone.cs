using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class HealingZone : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private Image healPowerBar;

	[Header("Settings")]
	[SerializeField] private int maxHealPower = 30;
	[SerializeField] private float healCooldown = 10f;
	[SerializeField] private float healTickRate = 1f;
	[SerializeField] private int coinsPerTick = 10;
	[SerializeField] private int healthPerTick = 10;

	private float remainingCooldown;
	private float tickTimer;
	private List<TankPlayer> playersInZone = new List<TankPlayer>();
	private NetworkVariable<int> HealPower = new NetworkVariable<int>();

	public override void OnNetworkSpawn()
	{
		if (IsClient)
		{
			HealPower.OnValueChanged += HandleHealPowerChanged;
			HandleHealPowerChanged(0, HealPower.Value);
		}

		if (IsServer)
		{
			HealPower.Value = maxHealPower;
		}
	}

	public override void OnNetworkDespawn()
	{
		if (IsClient)
		{
			HealPower.OnValueChanged -= HandleHealPowerChanged;
		}
	}

	private void OnTriggerEnter2D(Collider2D col)
	{
		if (!IsServer) return;
		if (!col.attachedRigidbody.TryGetComponent<TankPlayer>(out TankPlayer player)) return;

		playersInZone.Add(player);
	}

	private void OnTriggerExit2D(Collider2D col)
	{
		if (!IsServer) return;
		if (!col.attachedRigidbody.TryGetComponent<TankPlayer>(out TankPlayer player)) return;

		playersInZone.Remove(player);
	}

	private void Update()
	{
		if (!IsServer) return;

		if (remainingCooldown > 0f)
		{
			remainingCooldown -= Time.deltaTime;

			if (remainingCooldown <= 0f)
				HealPower.Value = maxHealPower; // ��Ÿ�� ������ ���� ������ �ٽ� Ǯ�� ä��
			else
				return; // ���������� �Ʒ��ڵ�Ȱ��� ����
		}

		tickTimer += Time.deltaTime;
		if (tickTimer >= 1 / healTickRate) // 1�ʸ��� ġ��
		{
			foreach (TankPlayer player in playersInZone) // ������ �ִ� ��� �÷��̾� ġ��
			{
				if (HealPower.Value == 0) break; // ������ ������ 0�̸� ġ�����
				if (player.Health.CurrentHealth.Value == player.Health.MaxHealth) continue; // ü���� MAX�� �÷��̾� ġ�����
				if (player.Wallet.TotalCoins.Value < coinsPerTick) continue; // ���ξ��� �÷��̾� ġ�����

				player.Wallet.SpendCoins(coinsPerTick); // �÷��̾� ���� �Ҹ�
				player.Health.RestoreHealth(healthPerTick); // �÷��̾� ġ��
				HealPower.Value -= 1; // ���� ������ -1
				if (HealPower.Value == 0) // ���� ������ ���� �Ҹ��ϸ�
				{
					remainingCooldown = healCooldown; // ���� ��Ÿ�� ����
				}
			}

			tickTimer = tickTimer % (1 / healTickRate); // 1�� ���� �������� �Ҽ���(0�� ����� ���ڷ� �ʱ�ȭ)
		}
	}

	private void HandleHealPowerChanged(int oldHealPower, int newHealPower)
	{
		healPowerBar.fillAmount = (float)newHealPower / maxHealPower;
	}
}
