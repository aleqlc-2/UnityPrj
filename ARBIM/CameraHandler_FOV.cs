using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class CameraHandler_FOV : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
	public static CameraHandler_FOV instance;

	public Camera arCamera;

	public Slider fovSlider;

	[SerializeField] private TextMeshProUGUI ImageAddChangeText; // FOV

	private bool isSensorLensTesting = false;
	public bool IsSensorLensTesting
	{
		get { return isSensorLensTesting; }
		set { isSensorLensTesting = value; }
	}

	void Awake()
	{
		if (instance == null) instance = this;
	}

	void Start()
	{
		fovSlider.minValue = 0.1f;
		fovSlider.maxValue = 178.9f;
	}

	public void OnBeginDrag(PointerEventData eventData)
	{

	}

	public void OnDrag(PointerEventData eventData)
	{
		fovSlider.value += eventData.delta.x;
		arCamera.fieldOfView = fovSlider.value;
		ImageAddChangeText.text = arCamera.fieldOfView.ToString();

		isSensorLensTesting = true;
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		isSensorLensTesting = false;
	}
}
