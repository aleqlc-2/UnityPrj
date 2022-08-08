using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;

public class FileConverter : MonoBehaviour
{
	private string filePath = "202test1.txt";
	private string host = "211.248.99.88";
	private string port = "21";

	void Start()
    {
		// File.Create("Assets/Resources/NewFile.txt");

		//string filePath = "Assets/Resources/202test1.txt";
		//string json = File.ReadAllText(filePath);
		//Debug.Log(File.ReadAllText(filePath));
	}

    void Update()
    {
		if (Input.GetKeyDown(KeyCode.K))
		{
			//string myFile = "C:\\Users\\Desktop\\ConvertResult\\202_84A_Type_skp_d555.txt";
			//string newFile = "C:\\Users\\Desktop\\ConvertResult\\202_84A_Type_skp_d555.fbx";

			string myFile = "Assets/Resources/202FileConvertTest.txt";
			string newFile = "Assets/Resources/202FileConvertTest.fbx";

			FileInfo f = new FileInfo(myFile);
			f.MoveTo(Path.ChangeExtension(newFile, ".fbx"));
		}

		if (Input.GetKeyDown(KeyCode.H))
		{
			StartCoroutine(GetData());
		}
	}

	private IEnumerator GetData()
	{
		Debug.Log("inMethod");
		var www = UnityWebRequest.Get("http://" + host + ":" + port + "/" + filePath);
		yield return www.SendWebRequest();

		while (!www.isDone)
		{
			Debug.Log("waiting...");
			yield return null;
		}

		if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
		{
			Debug.Log("inError");
			Debug.LogError(www.error);
		}
		else
		{
			Debug.Log("inPerfect");
			Debug.Log(www.uri.ToString());
			Debug.Log(www.downloadHandler.text);
		}
	}
}
