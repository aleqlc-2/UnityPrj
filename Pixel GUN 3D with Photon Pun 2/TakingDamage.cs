using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class TakingDamage : MonoBehaviourPunCallbacks
{
	[SerializeField] private Image healthBar;

    private float health;
	public float startHealth = 100f;

	private void Start()
	{
		health = startHealth;
		healthBar.fillAmount = health / startHealth;
	}

	private void Update()
	{
		
	}

	[PunRPC]
	public void TakeDamage(float _damage)
	{
		health -= _damage;
		healthBar.fillAmount = health / startHealth;

		if (health <= 0f)
		{
			Die();
		}
	}

	private void Die()
	{
		if (photonView.IsMine)
		{
			PixelGunGameManager.instance.LeaveRoom();
		}
	}
}
