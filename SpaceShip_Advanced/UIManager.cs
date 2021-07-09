using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
	public static UIManager instance = null;

	[SerializeField] private Text count3 = null;
	[SerializeField] private Text count2 = null;
	[SerializeField] private Text count1 = null;
	[SerializeField] private Text Ready = null;
	[SerializeField] private Text Go = null;
	[SerializeField] private Text Stage1 = null;
	[SerializeField] private Text Stage2 = null;
	[SerializeField] private Text BOSS = null;
	[SerializeField] private Text Clear = null;
	[SerializeField] private Text Level1Over = null;

	[SerializeField] private Button btnRestart = null;
	[SerializeField] private Button btnQuit = null;

	[SerializeField] private GameObject hearts = null;

	[SerializeField] private SpriteRenderer render = null;

	[SerializeField] private BossHealth bossHealth = null;

	private void Awake()
	{
		if (instance == null)
			instance = this;
	}

	private void Start()
    {
		bossHealth.SetCallback(StartHighlight);
	}

	public IEnumerator coUI(GameManager.State state)
	{
		if (state != GameManager.State.STAGE1)
		{
			Clear.gameObject.SetActive(true);
			yield return new WaitForSeconds(2f);
			Clear.gameObject.SetActive(false);
		}

		Stage1.gameObject.SetActive(state == GameManager.State.STAGE1);
		Stage2.gameObject.SetActive(state == GameManager.State.STAGE2);
		BOSS.gameObject.SetActive(state == GameManager.State.BOSS);
		yield return new WaitForSeconds(2f);
		Stage1.gameObject.SetActive(false);
		Stage2.gameObject.SetActive(false);
		BOSS.gameObject.SetActive(false);

		if (state == GameManager.State.STAGE1)
		{
			count3.gameObject.SetActive(true);
			yield return new WaitForSeconds(1f);
			count3.gameObject.SetActive(false);
			count2.gameObject.SetActive(true);
			yield return new WaitForSeconds(1f);
			count2.gameObject.SetActive(false);
			count1.gameObject.SetActive(true);
			yield return new WaitForSeconds(1f);
			count1.gameObject.SetActive(false);
		}

		Ready.gameObject.SetActive(true);
		yield return new WaitForSeconds(1f);
		Ready.gameObject.SetActive(false);

		Go.gameObject.SetActive(true);
		yield return new WaitForSeconds(1f);
		Go.gameObject.SetActive(false);

		yield return null;
	}

	public void TurnOffHeart()
	{
		if (GameManager.Instance.Lives >= 0)
		{
			hearts.transform.GetChild(GameManager.Instance.Lives).gameObject.SetActive(false); // 하트 이미지 1개 비활성화
		}
	}

	public void ShowButton()
	{
		btnRestart.gameObject.SetActive(true);
		btnQuit.gameObject.SetActive(true);
	}

	public void Level1OverUI()
	{
		Level1Over.gameObject.SetActive(true);
		StartCoroutine(GameManager.Instance.NextLevel());
	}

	public void heartImageReset()
	{
		for (int i = 0; i < hearts.transform.childCount; i++)
		{
			if (!hearts.transform.GetChild(i).gameObject.activeSelf)
			{
				hearts.transform.GetChild(i).gameObject.SetActive(true);
			}
		}
	}

	private void StartHighlight()
	{
		StartCoroutine(Highlight());
	}

	public void StopHighlight()
	{
		StopCoroutine(Highlight());
	}

	private IEnumerator Highlight()
	{
		Color start = Color.white;
		Color end = Color.red * 1.4f;

		float t = 0f;

		while (t < 0.1f)
		{
			render.material.color = Color.Lerp(start, end, t / 0.1f);
			t += Time.deltaTime;
			yield return null;
		}

		t = 0f;

		while (t < 0.1f)
		{
			render.material.color = Color.Lerp(end, start, t / 0.1f);
			t += Time.deltaTime;
			yield return null;
		}
	}

	private void OnDestroy()
	{
		if (instance != null)
			Destroy(this);
	}
}
