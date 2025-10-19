using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : NetworkBehaviour
{
    [Header("Reference")]
    [SerializeField] private Health health;
    [SerializeField] private Image healthBarImage;

	public override void OnNetworkSpawn()
	{
		if (!IsClient) return; // 호스트와 클라 모두 true임

		health.CurrentHealth.OnValueChanged += HandleHealthChanged;
		HandleHealthChanged(0, health.CurrentHealth.Value);
	}

	public override void OnNetworkDespawn()
	{
		if (!IsClient) return;

		health.CurrentHealth.OnValueChanged -= HandleHealthChanged;
	}

	private void HandleHealthChanged(int oldHealth, int newHealth)
	{
		healthBarImage.fillAmount = (float)newHealth / health.MaxHealth;
	}
}
