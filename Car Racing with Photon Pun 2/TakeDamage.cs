using System;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.Collections;

public class TakeDamage : MonoBehaviourPun
{
    public float startHealth = 100f;
    private float health;
    public Image healthBar;

	private Rigidbody rb;

	public GameObject PlayerGraphics;
	public GameObject PlyaerUI;
	public GameObject PlayerWeaponHolder;
	public GameObject DeathPanelUIPrefab;
	public GameObject DeathPanelUIGameObject;

	private void Start()
	{
		health = startHealth;
		healthBar.fillAmount = health / startHealth;
		rb = GetComponent<Rigidbody>();
	}

	[PunRPC]
	public void DoDamage(float damage)
	{
		health -= damage;
		healthBar.fillAmount = health / startHealth;
		if (health <= 0f)
		{
			Die();
		}
	}

	private void Die()
	{
		rb.linearVelocity = Vector3.zero;
		rb.angularVelocity = Vector3.zero;

		PlayerGraphics.SetActive(false);
		PlyaerUI.SetActive(false);
		PlayerWeaponHolder.SetActive(false);

		if (photonView.IsMine)
		{
			StartCoroutine(Respawn());
		}
	}

	private IEnumerator Respawn()
	{
		GameObject canvasGameObject = GameObject.Find("Canvas");

		if (DeathPanelUIGameObject == null)
		{
			DeathPanelUIGameObject = Instantiate(DeathPanelUIPrefab, canvasGameObject.transform);
		}
		else
		{
			DeathPanelUIGameObject.SetActive(true);
		}

		Text respawnTimeText = DeathPanelUIGameObject.transform.Find("ResapwnTimeText").GetComponent<Text>();
		float respawnTime = 8.0f;
		respawnTimeText.text = respawnTime.ToString(".00");

		while (respawnTime > 0.0f)
		{
			yield return new WaitForSeconds(1.0f);
			respawnTime -= 1.0f;
			respawnTimeText.text = respawnTime.ToString(".00f");

			GetComponent<CarMovement>().enabled = false;
			GetComponent<Shooting>().enabled = false;
		}

		DeathPanelUIGameObject.SetActive(false);

		GetComponent<CarMovement>().enabled = true;
		GetComponent<Shooting>().enabled = true;

		int randomPoint = UnityEngine.Random.Range(-20, 20);
		transform.position = new Vector3(randomPoint, 0, randomPoint);
		photonView.RPC("Reborn", RpcTarget.AllBuffered);
	}

	[PunRPC]
	public void Reborn()
	{
		health = startHealth;
		healthBar.fillAmount = health / startHealth;

		PlayerGraphics.SetActive(true);
		PlyaerUI.SetActive(true);
		PlayerWeaponHolder.SetActive(true);
	}
}
