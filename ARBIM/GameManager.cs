using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

	// FPS
	[Range(1, 100)] public int fFont_Size;
	[Range(0, 1)] public float red, green, blue;
	private float deltaTime = 0.0f;

	private int logCount = 0;
	public int LogCount
	{
		get { return logCount; }
		set { logCount = value; }
	}

	private float sumMsec;
	public float SumMsec
	{
		get { return sumMsec; }
		set { sumMsec = value; }
	}

	private float sumFps;
	public float SumFps
	{
		get { return sumFps; }
		set { sumFps = value; }
	}

	private float avgMsec;
	private float avgFps;

	private StringBuilder stringBuilder;

	//// µð¹ö±ëÅØ½ºÆ®
	//public TextMeshProUGUI touchBeganTxt;
	//public TextMeshProUGUI touch0Txt;
	//public TextMeshProUGUI touch1Txt;
	//public TextMeshProUGUI touchMovedTxt;

	void Awake()
	{
		if (instance == null)
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else if (instance != this)
		{
			Destroy(gameObject);
		}
	}

	void Start()
	{
		fFont_Size = fFont_Size == 0 ? 50 : fFont_Size;
		StartCoroutine(CalculateFPS());
		stringBuilder = new StringBuilder(1000);
	}

	void Update()
	{
		AccumulateDeltaTime();
	}

	private void AccumulateDeltaTime()
	{
		deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
	}

	private IEnumerator CalculateFPS()
	{
		yield return new WaitForSeconds(1f);

		while (true)
		{
			float msec = deltaTime * 1000.0f;
			float fps = 1.0f / deltaTime;
			ScrollViewContentHandler.instance.MakeFpsTxt();
			UIManager.instance.FpsLogTxt = ScrollViewContentHandler.instance.SpawnedFpsLogTxt;
			UIManager.instance.FpsLogTxt.text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
			stringBuilder.AppendLine(string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps));
			sumMsec += msec;
			sumFps += fps;
			logCount++;

			yield return new WaitForSeconds(5f);
		}
	}

	public void BtnExit()
	{
		StartCoroutine(ClickBtnExit());
	}

	public IEnumerator ClickBtnExit()
	{
		avgMsec = sumMsec / logCount;
		avgFps = SumFps / logCount;

		string avgResult = "Æò±Õ : " + string.Format("{0:0.0} ms ({1:0.} fps)", avgMsec, avgFps);
		stringBuilder.AppendLine(avgResult);

		string tempFpsLog = stringBuilder.ToString();
		Debug.Log(tempFpsLog);
		FileManager.instance.WriteTxt(tempFpsLog);

		yield return new WaitForSeconds(3f);

		Application.Quit();
	}

	//void OnGUI()
	//{
	//	int w = Screen.width, h = Screen.height;

	//	float msec = deltaTime * 1000.0f;
	//	float fps = 1.0f / deltaTime;

	//	string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);

	//	GUIStyle style = new GUIStyle();
	//	style.alignment = TextAnchor.UpperLeft;
	//	style.fontSize = (h * 2 / fFont_Size) * 2;
	//	style.normal.textColor = Color.white;

	//	Rect rect = new Rect(0, 0, w, h * 0.02f);

	//	GUI.Label(rect, text, style);
	//}
}
