using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class CameraHandler_FocalLength : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
	public static CameraHandler_FocalLength instance;

	public Camera arCamera;

	public Slider focalLengthSlider;

	[SerializeField] private TextMeshProUGUI ImageRemoveChangeText; // focallength

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
		focalLengthSlider.minValue = 0;
		focalLengthSlider.maxValue = 1250;
	}

	public void OnBeginDrag(PointerEventData eventData)
	{

	}

	public void OnDrag(PointerEventData eventData)
	{
		focalLengthSlider.value += eventData.delta.x;
		arCamera.focalLength = focalLengthSlider.value;
		ImageRemoveChangeText.text = arCamera.focalLength.ToString();

		isSensorLensTesting = true;
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		isSensorLensTesting = false;
	}
}
