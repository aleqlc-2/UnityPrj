using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.IO;
using System.Text;
using UnityEngine.Networking;
using System.Runtime.Serialization.Formatters.Binary;

public class FtpManager : MonoBehaviour
{
    private string ftpPath = "ftp://jhchoi@127.0.0.1/";
    private string fileName = "FromUnityFbx1.fbx"; // 업로드할시 새로 생성할 파일명, 만약 이미 있는이름이면 덮어씌어짐
    private string userName = "jhchoi";
    private string pwd = "fgftd4";
    private string UploadDirectory = "UploadTest1";

	private void Start()
	{
        Debug.Log(Application.persistentDataPath);
	}
	void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
		{
            FtpMakeFolder();
        }

        if (Input.GetKeyDown(KeyCode.U))
		{
            UploadFile(ftpPath, fileName, userName, pwd, UploadDirectory);
		}

        if (Input.GetKeyDown(KeyCode.D))
		{
            FtpDownload();
		}

        if (Input.GetKeyDown(KeyCode.T))
		{
            ConvertInUnity();
		}

        if (Input.GetKeyDown(KeyCode.I))
		{
            FileMoveAndInstantiate();
        }

        if (Input.GetKeyDown(KeyCode.V))
		{
            UploadFileFromUnity(ftpPath, fileName, userName, pwd, UploadDirectory);
        }
    }

    // 폴더만들기
    private void FtpMakeFolder()
    {
        string userName = "jhchoi";
        string pwd = "fgftd4";
        string folderName = "testFolder"; // 새로만들 폴더이름

        FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://jhchoi@127.0.0.1/UploadTest1/test.txt/" + folderName);
        request.Credentials = new NetworkCredential(user, pwd);
        request.UsePassive = true;
        request.UseBinary = true;
        request.KeepAlive = false;
        request.Method = WebRequestMethods.Ftp.MakeDirectory; // 폴더만들기

        try
        {
            FtpWebResponse res = (FtpWebResponse)request.GetResponse(); // 실제로 만듬
            //m_lUploadList.Add(URL + folderName); // 만든거 리스트에 추가
        }
        catch (WebException ex)
        {
            // 예외처리.
            FtpWebResponse response = (FtpWebResponse)ex.Response;

            switch (response.StatusCode)
            {
                case FtpStatusCode.ActionNotTakenFileUnavailable:
                    {
                        Debug.Log("CreateFolders ] Probably the folder already exist : " + folderName);
                    }
                    break;
            }
        }
    }

    // 파일업로드(로컬pc -> ftp)
    private string UploadFile(string ftpPath, string fileName, string userName, string pwd, string UploadDirectory = "")
	{
        string PureFileName = new FileInfo(fileName).Name;
        string uploadPath = string.Format("{0}/{1}/{2}", ftpPath, UploadDirectory, PureFileName);
        Debug.Log(uploadPath);
        FtpWebRequest req = (FtpWebRequest)WebRequest.Create(uploadPath);
        req.Proxy = null;
        req.Method = WebRequestMethods.Ftp.UploadFile;
        req.Credentials = new NetworkCredential(userName, pwd);
        req.UseBinary = true;
        req.UsePassive = true;

        byte[] data = File.ReadAllBytes(@"C:\Users\Desktop\ConvertResult\202_84A_Type_skp_d.txt"); // ftp서버로 업로드할 로컬파일주소
        req.ContentLength = data.Length;
        Stream stream = req.GetRequestStream();
        stream.Write(data, 0, data.Length);
        stream.Close();

        FtpWebResponse res = (FtpWebResponse)req.GetResponse();
        return res.StatusDescription;
	}

    // 파일다운로드(ftp -> 로컬pc)
    private void FtpDownload()
    {
        FtpWebRequest ftpWebRequest = (FtpWebRequest)WebRequest.Create("ftp://jhchoi@127.0.0.1/UploadTest1/fbxToTxt.txt"); // ftp서버내의 다운로드대상 파일주소
        ftpWebRequest.Credentials = new NetworkCredential(userName, pwd);
        ftpWebRequest.Method = WebRequestMethods.Ftp.DownloadFile;

        using (var localfile = File.Open(Application.persistentDataPath + "/fbxtotxt.txt", FileMode.Create)) // 로컬의 어느파일을 생성하여 쓸것인지
        using (var ftpStream = ftpWebRequest.GetResponse().GetResponseStream())
        {
            byte[] buffer = new byte[1024];
            int n;
            while ((n = ftpStream.Read(buffer, 0, buffer.Length)) > 0)
            {
                localfile.Write(buffer, 0, n);
            }
        }
    }

    // 유니티 내부에서 ftp로 업로드
    private string UploadFileFromUnity(string ftpPath, string fileName, string userName, string pwd, string UploadDirectory = "")
    {
        string PureFileName = new FileInfo(fileName).Name;
        string uploadPath = string.Format("{0}/{1}/{2}", ftpPath, UploadDirectory, PureFileName);
        Debug.Log(uploadPath);
        FtpWebRequest req = (FtpWebRequest)WebRequest.Create(uploadPath);
        req.Proxy = null;
        req.Method = WebRequestMethods.Ftp.UploadFile;
        req.Credentials = new NetworkCredential(userName, pwd);
        req.UseBinary = true;
        req.UsePassive = true;

        byte[] data = File.ReadAllBytes("Assets/Resources/ConvertResult.fbx"); // ftp서버로 업로드할 유니티내부의 파일주소
        req.ContentLength = data.Length;
        Stream stream = req.GetRequestStream();
        stream.Write(data, 0, data.Length);
        stream.Close();

        FtpWebResponse res = (FtpWebResponse)req.GetResponse();
        return res.StatusDescription;
    }

    // 유니티 내부에서 파일형식변환 후 Resources폴더에 저장
    private void ConvertInUnity()
	{
		// 유니티안에서 파일형식 변환
		string myFile = Application.persistentDataPath + "/fbxtotxt.txt";
		string newFile = Application.persistentDataPath + "/fbxtotxt.fbx";
		FileInfo f = new FileInfo(myFile);
		f.MoveTo(Path.ChangeExtension(newFile, ".fbx"));
	}

    // 변환시간 오래걸리므로 ftp서버에 변환된 파일 저장하고 fbx를 www로 불러오기
    private void FileMoveAndInstantiate()
	{
        File.Move(Application.persistentDataPath + "/fbxtotxt.fbx", "Assets/Resources/fbxtotxt.fbx");
        GameObject go = Resources.Load("fbxtotxt") as GameObject;
        Instantiate(go);
    }
}
