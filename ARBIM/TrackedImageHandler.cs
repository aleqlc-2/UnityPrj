using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;
using TMPro;

public class TrackedImageHandler : MonoBehaviour
{
	[SerializeField] private ARTrackedImageManager trackImageManager;

	private bool onTrackedImage = false;
	public bool OnTrackedImage
	{
		get { return onTrackedImage; }
		set { onTrackedImage = value; }
	}

	[SerializeField] private Image preview;

	// 마커
	[SerializeField] private GameObject buildingOnPlaneWithLight;
	private GameObject spawnedBuildingOnPlaneWithLight;
	public GameObject SpawnedBuildingOnPlaneWithLight
	{
		get { return spawnedBuildingOnPlaneWithLight; }
		set { spawnedBuildingOnPlaneWithLight = value; }
	}

	//// 디버깅텍스트
	//[SerializeField] private TextMeshProUGUI TouchStartText;
	//[SerializeField] private TextMeshProUGUI CaptureText;
	//[SerializeField] private TextMeshProUGUI AddImageStartText;
	//[SerializeField] private TextMeshProUGUI AddImageEndText;
	//[SerializeField] private TextMeshProUGUI ImageStartChangeText;
	//[SerializeField] private TextMeshProUGUI ImageAddChangeText;
	//[SerializeField] private TextMeshProUGUI ImageUpdateChangeText;
	//[SerializeField] private TextMeshProUGUI ImageRemoveChangeText;

	void OnEnable()
	{
		trackImageManager.trackedImagesChanged += ImageChanged;
	}

	public void ClickCaptureButton()
	{
		StartCoroutine(CaptureImage());
	}

	public IEnumerator CaptureImage()
	{
		yield return new WaitForEndOfFrame();

		var texture2D = ScreenCapture.CaptureScreenshotAsTexture();

		AddImage(texture2D);
		//CaptureText.text = "Capture : done";
	}

	private void AddImage(Texture2D imageToAdd)
	{
		//AddImageStartText.text = "AddImageStart : done";
		var library = trackImageManager.CreateRuntimeLibrary();

		if (library is MutableRuntimeReferenceImageLibrary mutableLibrary)
		{
			mutableLibrary.ScheduleAddImageWithValidationJob(imageToAdd, "testimg", 0.5f);
			trackImageManager.referenceLibrary = library; // mutableLibrary넣어도 같음

			Sprite sprite = Sprite.Create(imageToAdd, new Rect(0, 0, imageToAdd.width, imageToAdd.height), new Vector2(0.5f, 0.5f));
			preview.sprite = sprite;
			//AddImageEndText.text = "AddImageEnd : done";
		}
	}

	private void ImageChanged(ARTrackedImagesChangedEventArgs eventArgs)
	{
		//ImageStartChangeText.text = "ImageStartChangeText : done";

		foreach (ARTrackedImage image in eventArgs.added)
		{
			spawnedBuildingOnPlaneWithLight = Instantiate(buildingOnPlaneWithLight);
			spawnedBuildingOnPlaneWithLight.transform.GetChild(1).gameObject.SetActive(false); // Light 비활성화
			UIManager.instance.BuildingOnPlaneWithLight = spawnedBuildingOnPlaneWithLight;
			UIManager.instance.InitializeBuildingOnPlaneWithLight();
			onTrackedImage = true;
			//ImageAddChangeText.text = "ImageAddChange : done";
		}

		foreach (ARTrackedImage image in eventArgs.updated)
		{
			spawnedBuildingOnPlaneWithLight.transform.position = image.transform.position;

			//ImageUpdateChangeText.text = "ImageUpdateChange : done";
		}

		foreach (ARTrackedImage image in eventArgs.removed)
		{
			Destroy(spawnedBuildingOnPlaneWithLight);
			onTrackedImage = false;
			//ImageRemoveChangeText.text = "ImageRemoveChange : done";
		}
	}

	void OnDisable()
	{
		trackImageManager.trackedImagesChanged -= ImageChanged;
	}
}
