using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

// 단일 json 테스트
public class TargetClass
{
	public int id = 1;
    public string name = "asd";
}

// 여러 json 테스트
[System.Serializable]
public class TargetClass_Multi
{
	public List<int> id = new List<int>();
	public List<string> name = new List<string>();
	public int power;
	public string stat;
}


public class JsonConverter : MonoBehaviour
{
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.J))
		{
			ConvertToJson();
		}

		if (Input.GetKeyDown(KeyCode.C))
		{
			ConvertToClass();
		}

		if (Input.GetKeyDown(KeyCode.M))
		{
			ConvertToMultiClassAndMakeJson();
		}
	}

	// 너무 늦게 생성됨
	public void ConvertToJson()
	{
		// json 디버그출력
		TargetClass targetClass = new TargetClass();
		string json = JsonUtility.ToJson(targetClass);
		Debug.Log(json);

		// json파일생성
		string fileName = "Target";
		string path = Application.dataPath + "/" + fileName + ".Json"; // Assets 폴더에 생성
		if (!File.Exists(path))
		{
			return;
		}
		File.WriteAllText(path, json);

		//// byte로 직접써서 json파일생성
		//TargetClass targetClass = new TargetClass();
		//string json = JsonUtility.ToJson(targetClass);
		//Debug.Log(json);
		//string fileName = "Target";
		//string path = Application.dataPath + "/" + fileName + ".Json"; // Assets 폴더에 생성
		//FileStream fileStream = new FileStream(path, FileMode.Create);
		//byte[] data = Encoding.UTF8.GetBytes(json);
		//fileStream.Write(data, 0, data.Length);
		//fileStream.Close();
	}

	// path에 있는 Json파일을 Class로 변환
	public void ConvertToClass()
	{
		//// 직접읽기
		//string fileName = "Target";
		//string path = Application.dataPath + "/" + fileName + ".Json";
		//string json = File.ReadAllText(path);

		//TargetClass targetClass = JsonUtility.FromJson<TargetClass>(json);
		//Debug.Log(targetClass.id); // int
		//Debug.Log(targetClass.name); // string

		// 바이트로 읽기
		string fileName = "Target";
		string path = Application.dataPath + "/" + fileName + ".Json";
		if (!File.Exists(path))
		{
			return;
		}
		FileStream fileStream = new FileStream(path, FileMode.Open);
		byte[] data = new byte[fileStream.Length];
		fileStream.Read(data, 0, data.Length);
		fileStream.Close();
		string json = Encoding.UTF8.GetString(data);
		TargetClass targetClass = JsonUtility.FromJson<TargetClass>(json);

		Debug.Log(targetClass.id);
		Debug.Log(targetClass.name);
	}

	public void ConvertToMultiClassAndMakeJson()
	{
		string fileName = "Target";
		string path = Application.dataPath + "/" + fileName + ".Json";
		string loadJson = File.ReadAllText(path); // path에 있는 json파일 읽어옴
		Debug.Log(loadJson);
		if (!File.Exists(path))
		{
			return;
		}
		
		// 이 방식은 loadJson이 {"키1" : ["값1", "값2", "값3"], "키2" : [int값1, int값2, int값3], "키3" : int값, "키4" : int값} 일때
		// 불러온 loadJson파일을 targetClass_Multi에 저장
		TargetClass_Multi targetClass_Multi = JsonUtility.FromJson<TargetClass_Multi>(loadJson);

		// json파일로부터 불러온 데이터를 class의 각각의 변수에 저장
		if (targetClass_Multi != null)
		{
			Debug.Log(targetClass_Multi.id.Count);
			for (int i = 0; i < targetClass_Multi.id.Count; i++)
			{
				Debug.Log(targetClass_Multi.id[i] + " : " + targetClass_Multi.name[i]);
			}

			Debug.Log(targetClass_Multi.power);
			Debug.Log(targetClass_Multi.stat);
		}

		//string json = JsonUtility.ToJson(targetClass_Multi, true); // class를 다시 json으로
		//File.WriteAllText(path, json); // path에 json파일 생성
	}
}
