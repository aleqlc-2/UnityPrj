using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class CameraHandler : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
	public static CameraHandler instance;

	public Camera arCamera;

    public Slider sensorSlider;

    // 디버깅텍스트
    [SerializeField] private TextMeshProUGUI CaptureText; // 센서

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
		sensorSlider.minValue = 0;
		sensorSlider.maxValue = 200;
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		
	}

	public void OnDrag(PointerEventData eventData)
	{
		sensorSlider.value += eventData.delta.x;
		arCamera.sensorSize = new Vector2(sensorSlider.value, sensorSlider.value);
		CaptureText.text = arCamera.sensorSize.ToString();

		isSensorLensTesting = true;
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		isSensorLensTesting = false;
	}
}
