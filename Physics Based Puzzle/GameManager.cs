using NUnit.Framework.Constraints;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
	int score;
	int lives = 3;
	public TextMeshProUGUI scoreText, livesText;
	public GameObject ball;
	Rigidbody2D ballRb;
	Vector2 ballStartPos;
	float distanceToCamera;

	float distanceToNewSpawn = 6f;
	float traveledDistance;
	float lastYPos;

	public List<GameObject> trapPrefabs = new List<GameObject>();
	public Transform spawnPoint;
	List<GameObject> spawnedTraps = new List<GameObject>();

	public GameObject gameOverPanel;
	public GameObject winPanel;
	[HideInInspector] public bool gameOver;
	public GameObject fx_Destroy;

	public static GameManager instance;

	public enum GameModes
	{
		LEVEL,
		ENDURANCE
	}
	public GameModes gameMode;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		gameOver = false;
		gameOverPanel.SetActive(false);
		winPanel.SetActive(false);

		ballRb = ball.GetComponent<Rigidbody2D>();
		ballStartPos = ball.transform.position;
		distanceToCamera = ballStartPos.y;
		lastYPos = ballStartPos.y;

		if (gameMode == GameModes.ENDURANCE)
		{
			UpdateTextElements();
			StartCoroutine("DeleteTraps");
		}

		if (gameMode == GameModes.LEVEL)
		{
			winPanel.SetActive(false);
		}
	}

	public void WinGame()
	{
		winPanel.SetActive(true);
	}

	public void GameOver()
	{
		if (gameMode == GameModes.ENDURANCE)
		{
			Instantiate(fx_Destroy, ball.transform.position, Quaternion.identity);

			ballStartPos.y = Camera.main.transform.position.y + distanceToCamera;
			ball.transform.position = ballStartPos;
			ballRb.linearVelocity = Vector2.zero;
			ballRb.angularVelocity = 0f;

			lives--;
			DeleteTrapsAboveCam(0);

			UpdateTextElements();

			if (lives <= 0)
			{
				gameOver = true;
				ballRb.bodyType = RigidbodyType2D.Kinematic;
				StopCoroutine("DeleteTraps");
				gameOverPanel.SetActive(true);
			}
		}

		if (gameMode == GameModes.LEVEL)
		{
			Instantiate(fx_Destroy, ball.transform.position, Quaternion.identity);
			ballRb.bodyType = RigidbodyType2D.Kinematic;
			ballRb.linearVelocity = Vector2.zero;
			ballRb.angularVelocity = 0f;
			gameOver = true;
			gameOverPanel.SetActive(true);
		}
	}

	public void AddScore()
	{
		score++;
		UpdateTextElements();
	}

	void UpdateTextElements()
	{
		scoreText.text = "Score: " + score.ToString("D4");
		livesText.text = "Lives: " + lives;
	}

	private void LateUpdate()
	{
		if (gameMode == GameModes.ENDURANCE)
		{
			if (ball.transform.position.y <= Camera.main.transform.position.y)
			{
				Vector3 oldCamPos = Camera.main.transform.position;
				Vector3 newCamPos = new Vector3(oldCamPos.x, oldCamPos.y - 1f, oldCamPos.z);
				Camera.main.transform.position = Vector3.Lerp(oldCamPos, newCamPos, 2f * Time.deltaTime);
			}

			traveledDistance = lastYPos - ball.transform.position.y;
			if (traveledDistance >= distanceToNewSpawn)
			{
				lastYPos = ball.transform.position.y;
				traveledDistance = 0;

				CreateNewTrap();

				print("Now here we would create a new Trap");
			}
		}
	}

	void CreateNewTrap()
	{
		int index = Random.Range(0, trapPrefabs.Count);

		Vector3 spawnPos = new Vector3(-2.36f, lastYPos - distanceToNewSpawn, 1);

		GameObject newTrap = Instantiate(trapPrefabs[index], spawnPos, Quaternion.identity);
		spawnedTraps.Add(newTrap);
	}

	void DeleteTrapsAboveCam(float distance)
	{
		for (int i = spawnedTraps.Count-1; i >= 0; i--)
		{
			if (spawnedTraps[i].transform.position.y > Camera.main.transform.position.y + distance)
			{
				Destroy(spawnedTraps[i]);
				spawnedTraps.RemoveAt(i);
			}
		}
	}

	IEnumerator DeleteTraps()
	{
		while (true)
		{
			yield return new WaitForSeconds(5f);
			DeleteTrapsAboveCam(5f);
		}
	}
}
