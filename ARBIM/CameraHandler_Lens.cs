using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class CameraHandler_Lens : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
	public static CameraHandler_Lens instance;

	public Camera arCamera;

	public Slider lensSlider;

	[SerializeField] private TextMeshProUGUI AddImageEndText; // ∑ª¡Ó

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
		lensSlider.minValue = 0;
		lensSlider.maxValue = 10;
	}

	public void OnBeginDrag(PointerEventData eventData)
	{

	}

	public void OnDrag(PointerEventData eventData)
	{
		lensSlider.value += eventData.delta.x;
		arCamera.lensShift = new Vector2(lensSlider.value, lensSlider.value);
		AddImageEndText.text = arCamera.lensShift.ToString();

		isSensorLensTesting = true;
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		isSensorLensTesting = false;
	}
}
