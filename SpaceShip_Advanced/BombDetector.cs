using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombDetector : MonoBehaviour
{
    [SerializeField] private GameObject bombEffect = null;

	private void OnTriggerEnter2D(Collider2D other)
	{
        if (other.gameObject.CompareTag("Bomb"))
		{
			other.gameObject.SetActive(false);
			bombEffect.gameObject.SetActive(true);
			ObjPoolManager.Instance.ReturnEnemiesByBomb(GameManager.Instance.GetState());
			StartCoroutine(TurnOffEffect());
		}
    }

	private IEnumerator TurnOffEffect()
	{
		yield return new WaitForSeconds(1f);
		bombEffect.gameObject.SetActive(false);
	}
}
