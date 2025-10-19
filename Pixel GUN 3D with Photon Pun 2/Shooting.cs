using UnityEngine;
using Photon.Pun;

public class Shooting : MonoBehaviour
{
	[SerializeField] private Camera fpsCamera;

	public float fireRate = 0.1f;
	private float fireTimer;

	private void Update()
	{
		if (fireTimer < fireRate)
		{
			fireTimer += Time.deltaTime;
		}

		if (Input.GetButton("Fire1") && fireTimer > fireRate)
		{
			fireTimer = 0.0f;

			RaycastHit _hit;
			Ray ray = fpsCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f)); // Ä«¸Þ¶ó Áß¾ÓÀ¸·Î ray¸¦ ½ô

			if (Physics.Raycast(ray, out _hit, 100))
			{
				Debug.Log(_hit.collider.gameObject.name);

				if (_hit.collider.gameObject.CompareTag("Player") && !_hit.collider.gameObject.GetComponent<PhotonView>().IsMine)
				{
					_hit.collider.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, 10f);
				}
			}
		}
	}
}
