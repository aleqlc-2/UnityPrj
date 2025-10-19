using System.Collections;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public TileBoard board;
	public CanvasGroup gameOverCanvasGroup;

	public TextMeshProUGUI scoreText;
	public TextMeshProUGUI bestScoreText;
	private int score;

	private void Start()
	{
		NewGame();
	}

	public void NewGame()
	{
		SetScore(0);
		bestScoreText.text = LoadBestScore().ToString();

		gameOverCanvasGroup.alpha = 0f; // 안보이게
		gameOverCanvasGroup.interactable = false; // try again 버튼 안눌리게

		board.ClearBoard();
		board.CreateTile();
		board.CreateTile();
		board.enabled = true;
	}

	public void GameOver()
	{
		board.enabled = false;
		gameOverCanvasGroup.interactable = true; // try again 버튼 눌리게

		StartCoroutine(Fade(gameOverCanvasGroup, 1f, 1f));
	}

	private IEnumerator Fade(CanvasGroup gameOverCanvasGroup, float to, float delay)
	{
		yield return new WaitForSeconds(delay);
		float elapsed = 0f;
		float duration = 0.5f;
		float from = gameOverCanvasGroup.alpha;

		while (elapsed < duration)
		{
			gameOverCanvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
			elapsed += Time.deltaTime;
			yield return null;
		}

		gameOverCanvasGroup.alpha = to;
	}

	public void IncreaseScore(int points)
	{
		SetScore(score + points);
	}

	private void SetScore(int score)
	{
		this.score = score;
		scoreText.text = score.ToString();
		SaveBestScore();
	}

	private void SaveBestScore()
	{
		int bestScore = LoadBestScore();

		if (score > bestScore)
		{
			PlayerPrefs.SetInt("bestScore", score);
		}
	}

	private int LoadBestScore()
	{
		return PlayerPrefs.GetInt("bestScore", 0);
	}
}
