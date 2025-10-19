using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
	private void Start()
	{
		Destroy(gameObject, 3f);
	}
}
