using Photon.Pun;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    private float bulletDamage;

    public void Initialize(Vector3 direction, float speed, float damage)
    {
        bulletDamage = damage;

		transform.forward = direction;
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.linearVelocity = direction * speed;
    }

	private void OnTriggerEnter(Collider other)
	{
        Destroy(gameObject);

        if (other.gameObject.CompareTag("Player"))
        {
            if (other.gameObject.GetComponent<PhotonView>().IsMine)
            {
				other.gameObject.GetComponent<PhotonView>().RPC("DoDamage", RpcTarget.AllBuffered, bulletDamage);
			}
        }
	}
}
