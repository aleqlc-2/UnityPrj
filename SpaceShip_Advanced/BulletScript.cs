using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BulletScript : MonoBehaviour
{
	private float speed = 15f;
	
	[HideInInspector] public bool is_EnemyBullet = false;

	private float deactivate_Timer = 2f;

	// Unity messages
	private void Start()
	{
		if (is_EnemyBullet)
		{
			speed *= -1f;
		}

		if (!is_EnemyBullet)
		{
			StartCoroutine(DeactivateGameObject());
		}
	}

	private void Update()
	{
		Move();
	}

	// Functions
	private void Move()
	{
		Vector3 temp = transform.position;
		temp.y += speed * Time.deltaTime;
		transform.position = temp;
	}

	private IEnumerator DeactivateGameObject()
	{
		yield return new WaitForSeconds(deactivate_Timer);
		ObjPoolManager.Instance.ReturnPlayerBullet(gameObject);
	}

	private void OnTriggerEnter2D(Collider2D target)
	{
		if (target.tag == "Enemy")
		{
			if (gameObject.activeSelf) gameObject.SetActive(false);
		}
	}
}
