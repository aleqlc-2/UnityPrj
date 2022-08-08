using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;
using UnityEngine.Android;
using System.Diagnostics;

public class LoadNativeGallery : MonoBehaviour
{
	public static LoadNativeGallery instance;

	//// 디버깅텍스트
	//[SerializeField] private TextMeshProUGUI TouchStartText;
	//[SerializeField] private TextMeshProUGUI CaptureText;
	//[SerializeField] private TextMeshProUGUI AddImageStartText;
	//[SerializeField] private TextMeshProUGUI AddImageEndText;
	//[SerializeField] private TextMeshProUGUI ImageStartChangeText;
	//[SerializeField] private TextMeshProUGUI ImageAddChangeText;
	//[SerializeField] private TextMeshProUGUI ImageUpdateChangeText;
	//[SerializeField] private TextMeshProUGUI ImageRemoveChangeText;

	void Awake()
	{
		if (instance == null) instance = this;
	}

	void Start()
	{

	}

	public void GotoNative()
	{
		if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead))
		{
			Permission.RequestUserPermission(Permission.ExternalStorageRead);
		}

		// 팝업창

		// NativeGallery.GetImageFromGallery(CallbackForGallery);

		NativeGallery.GetImageFromGallery((image) =>
		{
			// image가 qr1.jpg까지포함된 fullPATH
			// selectedImage.Name이 qr1.jpg
			FileInfo selectedImage = new FileInfo(image);


			//if (!string.IsNullOrEmpty(selectedImage.ToString()))
			//	StartCoroutine(LoadImage(image));
		});

		//NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
		//{
		//	if (path != null)
		//	{
		//		Texture2D texture = NativeGallery.LoadImageAtPath(path, 10000000);

		//		if (texture == null) return;

		//		texture.LoadImage(tempImage);

		//		imgRaw.texture = texture;
		//	}
		//});
	}

	private void CallbackForGallery(string path)
	{
		
	}

	public IEnumerator LoadImage(string imagePath)
	{
		string PATH = Application.persistentDataPath + imagePath;

		byte[] imageData = File.ReadAllBytes(PATH);

		//string imageName = Path.GetFileName(imagePath).Split('.')[0];
		//string saveImagePath = Application.persistentDataPath + "/qr1.jpg";

		//File.WriteAllBytes(saveImagePath + imageName + ".jpg", imageData);

		//var tempImage = File.ReadAllBytes(saveImagePath);

		Texture2D texture = new Texture2D(0, 0);

		texture.LoadImage(imageData);

		Rect rect = new Rect(0, 0, texture.width, texture.height);
		//QrScanner.instance.qrBtnParent.gameObject.SetActive(true);
		//QrScanner.instance.qrImage.sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));

		//imgRaw.texture = texture;

		yield return null;
	}

}
