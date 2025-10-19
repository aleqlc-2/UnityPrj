using UnityEngine;

public class Score : MonoBehaviour
{
    bool scoredAlready;

	private void OnTriggerEnter2D(Collider2D col)
	{
		if (GameManager.instance.gameMode == GameManager.GameModes.ENDURANCE)
		{
			if (!scoredAlready)
			{
				GameManager.instance.AddScore();
				scoredAlready = true;
			}
		}

		if (GameManager.instance.gameMode == GameManager.GameModes.LEVEL)
		{
			if (!GameManager.instance.gameOver)
			{
				GameManager.instance.WinGame();
			}
		}
	}
}
