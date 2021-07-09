using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance;

	public enum State
	{
		STAGE1,
		STAGE2,
		BOSS,
		GAMEOVERUI
	}

	private State state = 0;

	public State GetState()
	{
		return state;
	}

	public void SetState(State value)
	{
		state = value;
	}

	private Coroutine coroutine = null;
	private Coroutine bossCoroutine = null;

	[SerializeField] private GameObject text = null;
	[SerializeField] private GameObject player = null;
	[SerializeField] private GameObject heartsParent = null;
	[SerializeField] private GameObject bombsParent = null;

	[SerializeField] private EnemySpawner enemySpawner = null;
	[SerializeField] private Boss boss = null;
	[SerializeField] private BossHealth bossHealth = null;

	private int bombs;
	public int Bombs
	{
		get { return bombs; }
		set { bombs = value; }
	}

	private int lives;
	public int Lives
	{
		get { return lives; }
		set { lives = value; }
	}

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
	}

	private void Start()
	{
		lives = heartsParent.transform.childCount;
		bombs = bombsParent.transform.childCount;
		player.SetActive(true);
		state = State.STAGE1;
	}

	private void Update()
	{
		if (lives <= 0) state = State.GAMEOVERUI;

		switch (state)
		{
			case State.STAGE1:
				if (coroutine == null)
					coroutine = StartCoroutine(STStage());
				break;

			case State.STAGE2:
				if (coroutine == null)
					coroutine = StartCoroutine(STStage());
				break;

			case State.BOSS:
				if (coroutine == null)
					coroutine = StartCoroutine(BossStage());
				break;

			case State.GAMEOVERUI:
					GameOver();
				break;

			default:
				break;
		}
	}

	private IEnumerator STStage()
	{
		ResetLife();

		yield return StartCoroutine(UIManager.instance.coUI(state));
		
		coroutine = StartCoroutine(enemySpawner.SpawnEnemies((int)state));
		yield return StartCoroutine(coTimer());
		StopCoroutine(coroutine);
		ObjPoolManager.Instance.ReturnEnemiesByStage((int)state);
		yield return null;

		if (state == State.STAGE1)
			changeState(State.STAGE2);
		else if (state == State.STAGE2)
			changeState(State.BOSS);

		yield return null;
	}

	private void changeState(State state)
	{
		if (coroutine != null)
		{
			StopCoroutine(coroutine);
			coroutine = null;
		}

		this.state = state;
	}

	private IEnumerator BossStage()
	{
		ResetLife();
		yield return StartCoroutine(UIManager.instance.coUI(state));
		enemySpawner.ShowBoss();
		bossCoroutine = StartCoroutine(boss.fire());
	}

	private IEnumerator coTimer()
	{
		float duration = 20f;

		var timerText = text.GetComponent<Text>();

		while (duration > 0f)
		{
			duration -= Time.deltaTime;
			timerText.text = string.Format("{0:00}", duration);
			yield return null;
		}

		yield return null;
	}

	private void ResetLife()
	{
		lives = heartsParent.transform.childCount;
		UIManager.instance.heartImageReset();
	}

	public void Restart()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	public IEnumerator NextLevel()
	{
		yield return null;
		// yield return new WaitForSeconds(3f);
		// SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
	}

	private void GameOver()
	{
		if (bossCoroutine != null)
		{
			StopCoroutine(bossCoroutine);
		}
		UIManager.instance.Level1OverUI();
		UIManager.instance.ShowButton();
	}

	public void Quit()
	{
		Application.Quit();
	}

	private void OnDestroy()
	{
		if (Instance != null)
			Instance = null;
	}
}