using Unity.VisualScripting;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public GameObject targetObject;
    private Vector3 targetPosition;

	public float zoomSpeed = 100f;
	public float minDistance = 1f;
	public float maxDistance = 10f;

	public float cameraRotateSpeed = 1000f;
	public float minVerticalAngle = -70f;
	public float maxVerticalAngle = 70f;

	private RaycastHit hit;
	public LayerMask obstacleLayer;
	public bool nearObstacle = false;
	public float defaultDistance = 3f;

	private bool firstLateUpdate = true;

	private void Start()
	{
		// targetPosition = targetObject.transform.position; // targetObject가 플레이어의 자식개체지만 localPosition이 아닌 position이라 적용됨
	}

	private void LateUpdate()
	{
		if (targetObject != null)
		{
			if (firstLateUpdate)
			{
				targetPosition = targetObject.transform.position;
				firstLateUpdate = false;
			}

			if (!nearObstacle)
			{
				transform.position += targetObject.transform.position - targetPosition;
			}

			// 카메라가 플레이어 따라가도록
			targetPosition = targetObject.transform.position;
			transform.LookAt(targetPosition);

			// 마우스스크롤 줌인,줌아웃
			float distance = Vector3.Distance(transform.position, targetPosition);
			float scroll = Input.GetAxis("Mouse ScrollWheel");
			if (scroll != 0.0f)
			{
				if (scroll > 0 && distance > minDistance) // 앞으로 마우스 스크롤하면
				{
					transform.position += transform.forward * zoomSpeed * Time.deltaTime; // 카메라 줌인
				}
				else if (scroll < 0 && distance < maxDistance) // 뒤로 마우스 스크롤하면
				{
					transform.position -= transform.forward * zoomSpeed * Time.deltaTime; // 카메라 줌아웃
				}
			}

			// 마우스클릭한채로 카메라회전
			float mouseInputX = Input.GetAxis("Mouse X");
			float mouseInputY = Input.GetAxis("Mouse Y");
			if (Input.GetMouseButton(0))
			{
				transform.RotateAround(targetPosition, Vector3.up, mouseInputX * Time.deltaTime * cameraRotateSpeed); // 플레이어를 중심으로 좌우공전
				transform.RotateAround(targetPosition, transform.right, mouseInputY * Time.deltaTime * -cameraRotateSpeed); // 플레이어를 중심으로 상하공전

				float angleToHorizontalPlane = Vector3.Angle(transform.forward, Vector3.up) - 90f;
				if (angleToHorizontalPlane + mouseInputY * Time.deltaTime * -10f < minVerticalAngle)
				{
					mouseInputY = (minVerticalAngle - angleToHorizontalPlane) / (-10f * Time.deltaTime);
				}
				else if (angleToHorizontalPlane + mouseInputY * Time.deltaTime * -10f > maxVerticalAngle)
				{
					mouseInputY = (maxVerticalAngle - angleToHorizontalPlane) / (-10f * Time.deltaTime);
				}

				// 상하회전 clamp
				transform.RotateAround(targetPosition, transform.right, mouseInputY * Time.deltaTime * -10f);
			}

			// 장애물로 플레이어안보이면 플레이어 가까이로 카메라 이동
			if (Physics.Linecast(targetPosition, transform.position, out hit, obstacleLayer))
			{
				transform.position = hit.point;
				nearObstacle = true;
			}

			Debug.DrawLine(targetPosition, transform.position, Color.red, 0f, false);

			if (distance > defaultDistance)
			{
				nearObstacle = false;
			}
		}
	}
}
