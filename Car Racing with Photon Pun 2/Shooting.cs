using Photon.Pun.Demo.Asteroids;
using System;
using System.Collections;
using UnityEngine;
using Photon.Pun;

public class Shooting : MonoBehaviourPun
{
	public GameObject BulletPrefab;
	public Transform firePosition;
	public Camera PlayerCamera;

	public DeathRacePlayer deathRacePlayerProperties;

	private float fireRate = 0.1f;
	private float fireTimer = 0.0f;
	private bool useLaser;
	public LineRenderer lineRenderer;

	private void Start()
	{
		fireRate = deathRacePlayerProperties.fireRate;

		if (deathRacePlayerProperties.weaponName == "Laser Gun")
		{
			useLaser = true;
		}
		else
		{
			useLaser = false;
		}
	}

	private void Update()
	{
		if (!photonView.IsMine)
		{
			return;
		}

		if (Input.GetKey("space"))
		{
			if (fireTimer > fireRate)
			{
				photonView.RPC("Fire", RpcTarget.All, firePosition.position);
				fireTimer = 0.0f;
			}
		}

		if (fireTimer < fireRate)
		{
			fireTimer += Time.deltaTime;
		}
	}

	[PunRPC]
	public void Fire(Vector3 firePosition)
	{
		if (useLaser)
		{
			RaycastHit hit;
			Ray ray = PlayerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f));

			if (Physics.Raycast(ray, out hit, 200))
			{
				if (!lineRenderer.enabled)
				{
					lineRenderer.enabled = true;
				}

				lineRenderer.startWidth = 0.3f;
				lineRenderer.endWidth = 0.1f;

				lineRenderer.SetPosition(0, firePosition);
				lineRenderer.SetPosition(1, hit.point);

				if (hit.collider.gameObject.CompareTag("Player"))
				{
					if (hit.collider.gameObject.GetComponent<PhotonView>().IsMine)
					{
						hit.collider.gameObject.GetComponent<PhotonView>().RPC("DoDamage", RpcTarget.AllBuffered, deathRacePlayerProperties.damage);
					}
				}

				StopAllCoroutines();
				StartCoroutine(DisableLaserAfterSecs(0.3f));
			}
		}
		else
		{
			Ray ray = PlayerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f));

			GameObject bulletGameObject = Instantiate(BulletPrefab, firePosition, Quaternion.identity);
			bulletGameObject.GetComponent<BulletScript>().Initialize(ray.direction, deathRacePlayerProperties.bulletSpeed, deathRacePlayerProperties.damage);
		}
	}

	private IEnumerator DisableLaserAfterSecs(float seconds)
	{
		yield return new WaitForSeconds(seconds);
		lineRenderer.enabled = false;
	}
}
