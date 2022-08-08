using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARCore;
using UnityEngine.UI;
using TMPro;

public class ArCameraHandler : MonoBehaviour
{
	[SerializeField] private Camera arCamera;
	[SerializeField] private ARCameraManager arCameraManager;

	private float fov;
	private float distance = 0f; // mm
	private float sensorSize = 7.470588f; // mm
	private float lensSize = 26f; // mm
	[SerializeField] private LineRenderer lineRenderer;
	[SerializeField] private Material lineMat;

	//// 디버깅텍스트
	//[SerializeField] private TextMeshProUGUI TouchStartText;
	//[SerializeField] private TextMeshProUGUI CaptureText;
	//[SerializeField] private TextMeshProUGUI AddImageStartText;
	//[SerializeField] private TextMeshProUGUI AddImageEndText;
	//[SerializeField] private TextMeshProUGUI ImageStartChangeText;
	//[SerializeField] private TextMeshProUGUI ImageAddChangeText;
	//[SerializeField] private TextMeshProUGUI ImageUpdateChangeText;
	//[SerializeField] private TextMeshProUGUI ImageRemoveChangeText;

	void Update()
	{
		Ray ray = Camera.main.ScreenPointToRay(new Vector3(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2, 0));
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Building"))) // NameToLayer 왜 안되지
		{
			lineRenderer.enabled = true;
			lineRenderer.startWidth = .0025f;
			lineRenderer.endWidth = .05f;
			lineRenderer.material = lineMat;
			lineRenderer.positionCount = 2;
			lineRenderer.SetPosition(0, Camera.main.transform.position - new Vector3(0, .05f, 0));
			lineRenderer.SetPosition(1, hit.point);

			distance = hit.distance * 1000f;
			fov = distance * sensorSize / lensSize; // 보여지는 영역의 크기
			//TouchStartText.text = "fov : " + fov.ToString();
			//CaptureText.text = "distance : " + distance + "mm";
		}
	}
}

