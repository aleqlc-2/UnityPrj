using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScrollViewContentHandler : MonoBehaviour
{
    public static ScrollViewContentHandler instance;

	[SerializeField] private TextMeshProUGUI fpsLogTxt;

	private TextMeshProUGUI spawnedFpsLogTxt;
	public TextMeshProUGUI SpawnedFpsLogTxt
	{
		get { return spawnedFpsLogTxt; }
		set { spawnedFpsLogTxt = value; }
	}

	void Awake()
	{
        if (instance == null)
            instance = this;
	}
	
	public void MakeFpsTxt()
	{
		spawnedFpsLogTxt = Instantiate(fpsLogTxt, this.gameObject.transform);
	}
}
