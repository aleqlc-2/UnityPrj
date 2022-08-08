using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using ZXing;

public class QrScanner : MonoBehaviour
{
	public static QrScanner instance;

    [SerializeField] ARCameraManager CameraManager;
    [SerializeField] private TextMeshProUGUI text;
	public Image qrImage;
	[SerializeField] private Button btnImage;

	[SerializeField] private Image outlineImage;
	
	//[SerializeField] private Image testImage1;
	//[SerializeField] private Image testImage2;
	//[SerializeField] private Image testImage3;
	//[SerializeField] private Image testImage4;

	public Image qrBtnParent;

	//// 디버깅텍스트
	//[SerializeField] private TextMeshProUGUI TouchStartText;
	//[SerializeField] private TextMeshProUGUI CaptureText;
	//[SerializeField] private TextMeshProUGUI AddImageStartText;
	//[SerializeField] private TextMeshProUGUI AddImageEndText;
	//[SerializeField] private TextMeshProUGUI ImageStartChangeText;
	//[SerializeField] private TextMeshProUGUI ImageAddChangeText;
	//[SerializeField] private TextMeshProUGUI ImageUpdateChangeText;
	//[SerializeField] private TextMeshProUGUI ImageRemoveChangeText;

	private string resultTxt;

	void Awake()
	{
		if (instance == null) instance = this;
	}

	void Update()
    {
		if (CameraManager.TryAcquireLatestCpuImage(out XRCpuImage image))
		{
            using (image)
			{
                var conversionParams = new XRCpuImage.ConversionParams(image, TextureFormat.R8, XRCpuImage.Transformation.MirrorY);
                var dataSize = image.GetConvertedDataSize(conversionParams);
                var grayscalePixels = new byte[dataSize];

				unsafe
				{
                    fixed (void* ptr = grayscalePixels)
					{
						image.Convert(conversionParams, new System.IntPtr(ptr), dataSize);
					}
				}

				IBarcodeReader barcodeReader = new BarcodeReader();
				var result = barcodeReader.Decode(grayscalePixels, image.width, image.height, RGBLuminanceSource.BitmapFormat.Gray8);

				if (result != null)
				{
					text.text = result.Text;

					ResultPoint[] point = result.ResultPoints;
					float tempX = (int)point[0].X;
					float tempY = (int)point[0].Y;
					float tempXX = (int)point[1].X;
					float tempYY = (int)point[1].Y;
					float tempXXX = (int)point[2].X;
					float tempYYY = (int)point[2].Y;

					Vector2 pointVec0 = new Vector2(tempX, tempY);
					Vector2 pointVec1 = new Vector2(tempXX, tempYY);
					Vector2 pointVec2 = new Vector2(tempXXX, tempYYY);

					Vector2 tempPosition = new Vector2((int)((tempX + tempXX) / 2f), (int)((tempYY + tempYYY) / 2f));
					//Vector2 tempScale = new Vector2(Mathf.Abs(Vector2.Distance(pointVec0, pointVec1)), Mathf.Abs(Vector2.Distance(pointVec1, pointVec2)));
					// ?.rectTransform.sizeDelta = ?;

					float ratioPosX = tempPosition.x / Screen.width;
					float ratioPosY = tempPosition.y / Screen.height;

					Vector3 resultVec = new Vector3(-1200 * ratioPosY * 3f, 2160 * ratioPosX * 9f, 0) - new Vector3();
					
					outlineImage.rectTransform.localPosition = resultVec;

					switch (result.Text)
					{
						case "그림1":
							resultTxt = "/qr1.jpg";
							StartCoroutine(LoadNativeGallery.instance.LoadImage(resultTxt));
							LoadNativeGallery.instance.GotoNative();
							break;

						case "그림2":
							resultTxt = "/qr2.jpg";
							StartCoroutine(LoadNativeGallery.instance.LoadImage(resultTxt));
							LoadNativeGallery.instance.GotoNative();
							break;

						case "그림3":
							resultTxt = "/qr3.jpg";
							StartCoroutine(LoadNativeGallery.instance.LoadImage(resultTxt));
							LoadNativeGallery.instance.GotoNative();
							break;

						case "그림4":
							resultTxt = "/qr4.jpg";
							StartCoroutine(LoadNativeGallery.instance.LoadImage(resultTxt));
							LoadNativeGallery.instance.GotoNative();
							break;

						case "그림5":
							resultTxt = "/qr5.jpg";
							StartCoroutine(LoadNativeGallery.instance.LoadImage(resultTxt));
							LoadNativeGallery.instance.GotoNative();
							break;
					}
				}
			}
		}
	}

	public void TurnOffImage()
	{
		qrBtnParent.gameObject.SetActive(false);
	}
}
