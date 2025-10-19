using UnityEngine;

public class Trap : MonoBehaviour
{
	private void OnTriggerEnter2D(Collider2D col)
	{
		GameManager.instance.GameOver();
	}
}
