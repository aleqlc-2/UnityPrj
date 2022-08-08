using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using TMPro;
using UnityEngine.Android;

public class FileManager : MonoBehaviour
{
    public static FileManager instance;

	////디버깅텍스트
	//public TextMeshProUGUI touchBeganTxt;
	//public TextMeshProUGUI touch0Txt;
	//public TextMeshProUGUI touch1Txt;
	//public TextMeshProUGUI touchMovedTxt;

	private void Awake()
	{
        if (instance == null) instance = this;
	}

    public void WriteTxt(string message)
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
		{
            Permission.RequestUserPermission(Permission.ExternalStorageWrite);
		}

        FileStream fileStream = new FileStream(Application.persistentDataPath + "/MyGame3.txt", FileMode.OpenOrCreate, FileAccess.Write);
        StreamWriter writer = new StreamWriter(fileStream);

        writer.WriteLine(message);
        writer.Flush();
        writer.Close();
    }

	//public string ReadTxt()
	//{
	//	string filePath = Path.Combine(Application.persistentDataPath, "Example.txt");

	//	FileInfo fileInfo = new FileInfo(filePath);
	//	string value = "";

	//	if (fileInfo.Exists)
	//	{
	//		StreamReader reader = new StreamReader(filePath);
	//		value = reader.ReadToEnd();
	//		reader.Close();
	//	}

	//	else
	//		value = "파일이 없습니다.";

	//	return value;
	//}
}
