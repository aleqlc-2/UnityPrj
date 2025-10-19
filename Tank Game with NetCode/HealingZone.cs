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
				HealPower.Value = maxHealPower; // 쿨타임 지나면 힐존 게이지 다시 풀로 채움
			else
				return; // 안지났으면 아래코드안가고 리턴
		}

		tickTimer += Time.deltaTime;
		if (tickTimer >= 1 / healTickRate) // 1초마다 치료
		{
			foreach (TankPlayer player in playersInZone) // 힐존에 있는 모든 플레이어 치료
			{
				if (HealPower.Value == 0) break; // 힐존의 게이지 0이면 치료안함
				if (player.Health.CurrentHealth.Value == player.Health.MaxHealth) continue; // 체력이 MAX인 플레이어 치료안함
				if (player.Wallet.TotalCoins.Value < coinsPerTick) continue; // 코인없는 플레이어 치료안함

				player.Wallet.SpendCoins(coinsPerTick); // 플레이어 코인 소모
				player.Health.RestoreHealth(healthPerTick); // 플레이어 치료
				HealPower.Value -= 1; // 힐존 게이지 -1
				if (HealPower.Value == 0) // 힐존 게이지 전부 소모하면
				{
					remainingCooldown = healCooldown; // 힐존 쿨타임 진입
				}
			}

			tickTimer = tickTimer % (1 / healTickRate); // 1로 나눈 나머지는 소수점(0에 가까운 숫자로 초기화)
		}
	}

	private void HandleHealPowerChanged(int oldHealPower, int newHealPower)
	{
		healPowerBar.fillAmount = (float)newHealPower / maxHealPower;
	}
}
