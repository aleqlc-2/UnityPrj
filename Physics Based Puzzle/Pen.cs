using UnityEngine;

public class Pen : MonoBehaviour
{
    public GameObject dotPrefab;

	private void Update()
	{
		if (Input.GetMouseButton(0) && !GameManager.instance.gameOver)
		{
			Vector2 mousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
			Vector2 objPosition = Camera.main.ScreenToWorldPoint(mousePosition);

			Instantiate(dotPrefab, objPosition, Quaternion.identity);
		}
	}
}
