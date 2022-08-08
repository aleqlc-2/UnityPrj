using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.EventSystems;
using TMPro;

public class TouchManager : MonoBehaviour
{
	public static TouchManager instance;

	[SerializeField] private UIManager uiManager;
	[SerializeField] private MeshHandler meshHandler;
	[SerializeField] private TrackedImageHandler trackedImageHandler;

	[SerializeField] ARPlaneManager arPlaneManager;
	private List<ARPlane> planeList;

	private ARRaycastManager raycastMgr;
	private List<ARRaycastHit> hits = new List<ARRaycastHit>();

	public LineRenderer lineRenderer;
	public Material lineMat;

	private GameObject placeObject;
	public GameObject PlaceObject
	{
		get { return placeObject; }
		set { placeObject = value; }
	}

	private GameObject tempObj;

	// 신회전
	private Vector2 limitAngle = new Vector2(359.0f, 25.0f);
	private float rotateSpeed = 0.2f;
	[SerializeField] private Transform cachedTransform = null;
	private Vector2 curPos = Vector2.zero;
	private Vector2 prevPos = Vector2.zero;
	private float yaw = 0.0f;
	private float pitch = 0.0f;

	// 줌인, 줌아웃
	private float initialDistance;
	private Vector3 initialScale;

	// 세손가락 터치 드래그
	private float Speed = 30;
	private Vector2 avgPrePos;
	private Vector2 avgNowPos;
	private Vector3 movePos;
	private Vector3 deltaPosVec3;
	private Vector3 deltaPosVec3_2;

	//// 디버깅텍스트
	//public TextMeshProUGUI touchBeganTxt;
	//public TextMeshProUGUI touch0Txt;
	//public TextMeshProUGUI touch1Txt;
	//public TextMeshProUGUI touchMovedTxt;

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
		if (instance == null)
			instance = this;
	}

	void Start()
	{
		raycastMgr = GetComponent<ARRaycastManager>();
		planeList = new List<ARPlane>();
	}

	void Update()
	{
	//	if (tempObj != null)
	//	{
	//		TouchStartText.text = tempObj.transform.position.ToString();
	//		AddImageEndText.text = tempObj.transform.rotation.ToString();
	//	}
			
		Camera.main.usePhysicalProperties = true; // 이거 Update에 써줘야 피지컬카메라 켜짐

		if (CameraHandler.instance.IsSensorLensTesting || CameraHandler.instance.IsSensorLensTesting ||
			CameraHandler.instance.IsSensorLensTesting || CameraHandler.instance.IsSensorLensTesting)
		{
			return;
		}

		if (trackedImageHandler.OnTrackedImage) // 이미지트래킹중
		{
			// 물체회전
			if (Input.touchCount == 1) // 손가락 1개가 눌렸을 때
			{
				if (uiCheckPointer()) return;

				var touchZero = Input.GetTouch(0);

				if (touchZero.phase == TouchPhase.Ended || touchZero.phase == TouchPhase.Canceled) return;

				if (touchZero.phase == TouchPhase.Began)
				{
					curPos = touchZero.position;
				}
				else if (touchZero.phase == TouchPhase.Moved)
				{
					prevPos = curPos;
					curPos = touchZero.position;

					yaw += (prevPos.x - curPos.x) * rotateSpeed;
					pitch += (prevPos.y - curPos.y) * rotateSpeed;

					//yaw = ClampAngle(yaw, limitAngle.x);
					pitch = ClampAngle(pitch, limitAngle.y);

					trackedImageHandler.SpawnedBuildingOnPlaneWithLight.transform.eulerAngles = new Vector3(pitch, yaw);
				}
			}
			
			// 줌인, 줌아웃
			if (Input.touchCount == 2) // 손가락 2개가 눌렸을 때
			{
				if (uiCheckPointer()) return;

				Zoom(trackedImageHandler.SpawnedBuildingOnPlaneWithLight);
			}

			if (Input.touchCount == 4)
			{
				if (uiCheckPointer()) return;

				Destroy(trackedImageHandler.SpawnedBuildingOnPlaneWithLight);

				// 생성클릭하시오 텍스트삭제
			}
		}
		else // 평면인식
		{
			foreach (var plane in arPlaneManager.trackables)
			{
				planeList.Add(plane);

				if (planeList.Count > 0)
				{
					if (UIManager.instance.Coroutine == null) UIManager.instance.StartCorouTextBlink();
				}
			}

			if (Input.touchCount == 0)
			{
				
			}


			if (Input.touchCount == 1)
			{
				if (uiCheckPointer()) return;

				Touch touchZero = Input.GetTouch(0);

				if (touchZero.phase == TouchPhase.Ended || touchZero.phase == TouchPhase.Canceled) return;

				if (touchZero.phase == TouchPhase.Began)
				{
					App.instance.LoadAsset();

					curPos = touchZero.position;

					if (raycastMgr.Raycast(touchZero.position, hits, TrackableType.PlaneWithinPolygon) && tempObj == null)
					{
						tempObj = Instantiate(placeObject, hits[0].pose.position, placeObject.transform.rotation);
						tempObj.transform.position += new Vector3(0f, 8f, 0f);
						cachedTransform = tempObj.transform;

						meshHandler.OffShadowCast(tempObj);
						uiManager.BuildingOnPlaneWithLight = tempObj;
						UIManager.instance.InitializeBuildingOnPlaneWithLight();
						UIManager.instance.IsCreatedBuilding = true;

						foreach (var plane in arPlaneManager.trackables)
						{
							plane.gameObject.SetActive(false);
						}

						planeList.Clear();
						arPlaneManager.enabled = false;
					}
					else
					{
						return;
					}
				}
				else if (touchZero.phase == TouchPhase.Moved && tempObj != null) // touch moved
				{
					prevPos = curPos;
					curPos = touchZero.position;

					yaw += (prevPos.x - curPos.x) * rotateSpeed;
					pitch += (prevPos.y - curPos.y) * rotateSpeed;

					//yaw = ClampAngle(yaw, limitAngle.x);
					pitch = ClampAngle(pitch, limitAngle.y);

					cachedTransform.localEulerAngles = new Vector3(pitch, yaw);
				}
			}

			// 줌인, 줌아웃
			if (Input.touchCount == 2) //손가락 2개가 눌렸을 때
			{
				if (uiCheckPointer()) return;

				if (tempObj != null) Zoom(tempObj);
			}

			if (Input.touchCount == 3)
			{
				if (uiCheckPointer()) return;

				if (tempObj != null) DragBuildingOnPlane(tempObj);
			}

			if (Input.touchCount == 4)
			{
				if (uiCheckPointer()) return;

				if (tempObj != null)
				{
					Destroy(tempObj);
					tempObj = null;
				}
				
				arPlaneManager.enabled = true;
				planeList.Clear();
				UIManager.instance.Coroutine = null;
				UIManager.instance.IsCreatedBuilding = false;
			}
		}
	}

	private float ClampAngle(float value, float angle)
	{
		value = value % 360.0f; // 언더, 오버플로 방지
		angle = Mathf.Abs(angle % 180.0f); // clamp min max 뒤집히는 것 방지 및 각도 범위 오류 방지

		return Mathf.Clamp(value, -angle, angle);
	}

	private void Zoom(GameObject obj)
	{
		var touchZero = Input.GetTouch(0);
		var touchOne = Input.GetTouch(1);

		if (touchZero.phase == TouchPhase.Ended || touchZero.phase == TouchPhase.Canceled ||
			touchOne.phase == TouchPhase.Ended || touchOne.phase == TouchPhase.Canceled)
		{
			return;
		}

		if (touchZero.phase == TouchPhase.Began || touchOne.phase == TouchPhase.Began)
		{
			initialDistance = Vector2.Distance(touchZero.position, touchOne.position);
			initialScale = obj.transform.localScale;
		}
		else // touch moved
		{
			var currentDistance = Vector2.Distance(touchZero.position, touchOne.position);

			if (Mathf.Approximately(initialDistance, 0)) return;

			var factor = currentDistance / initialDistance;

			obj.transform.localScale = initialScale * factor;
		}
	}

	private void DragBuildingOnPlane(GameObject tempObj)
	{
		Touch touchZero = Input.GetTouch(0);
		Touch touchOne = Input.GetTouch(1);
		Touch touchTwo = Input.GetTouch(2);

		if (touchZero.phase == TouchPhase.Ended || touchZero.phase == TouchPhase.Canceled ||
			touchOne.phase == TouchPhase.Ended || touchOne.phase == TouchPhase.Canceled ||
			touchTwo.phase == TouchPhase.Ended || touchTwo.phase == TouchPhase.Canceled)
		{
			return;
		}

		if (touchZero.phase == TouchPhase.Began || touchOne.phase == TouchPhase.Began || touchTwo.phase == TouchPhase.Began)
		{
			avgPrePos = ((touchZero.position + touchOne.position + touchTwo.position) / 3) -
						((touchZero.deltaPosition + touchOne.deltaPosition + touchTwo.deltaPosition) / 3);

			if (raycastMgr.Raycast(avgPrePos, hits, TrackableType.PlaneWithinPolygon))
			{
				deltaPosVec3 = hits[0].pose.position;
			}
		}
		else // touch moved
		{
			avgNowPos = ((touchZero.position + touchOne.position + touchTwo.position) / 3) -
						((touchZero.deltaPosition + touchOne.deltaPosition + touchTwo.deltaPosition) / 3);

			if (raycastMgr.Raycast(avgNowPos, hits, TrackableType.PlaneWithinPolygon))
			{
				deltaPosVec3_2 = hits[0].pose.position;
			}

			movePos = deltaPosVec3 - deltaPosVec3_2;
			movePos = new Vector3(-movePos.x, 0, -movePos.z);

			tempObj.transform.Translate(movePos * Time.deltaTime * Speed, Space.World);

			deltaPosVec3 = deltaPosVec3_2;
		}
	}

	private bool uiCheckPointer()
	{
		PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
		eventDataCurrentPosition.position = new Vector2(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y);

		List<RaycastResult> results = new List<RaycastResult>();

		EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

		return results.Count > 0;
	}
}
