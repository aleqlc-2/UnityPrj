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
		// targetPosition = targetObject.transform.position; // targetObject�� �÷��̾��� �ڽİ�ü���� localPosition�� �ƴ� position�̶� �����
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

			// ī�޶� �÷��̾� ���󰡵���
			targetPosition = targetObject.transform.position;
			transform.LookAt(targetPosition);

			// ���콺��ũ�� ����,�ܾƿ�
			float distance = Vector3.Distance(transform.position, targetPosition);
			float scroll = Input.GetAxis("Mouse ScrollWheel");
			if (scroll != 0.0f)
			{
				if (scroll > 0 && distance > minDistance) // ������ ���콺 ��ũ���ϸ�
				{
					transform.position += transform.forward * zoomSpeed * Time.deltaTime; // ī�޶� ����
				}
				else if (scroll < 0 && distance < maxDistance) // �ڷ� ���콺 ��ũ���ϸ�
				{
					transform.position -= transform.forward * zoomSpeed * Time.deltaTime; // ī�޶� �ܾƿ�
				}
			}

			// ���콺Ŭ����ä�� ī�޶�ȸ��
			float mouseInputX = Input.GetAxis("Mouse X");
			float mouseInputY = Input.GetAxis("Mouse Y");
			if (Input.GetMouseButton(0))
			{
				transform.RotateAround(targetPosition, Vector3.up, mouseInputX * Time.deltaTime * cameraRotateSpeed); // �÷��̾ �߽����� �¿����
				transform.RotateAround(targetPosition, transform.right, mouseInputY * Time.deltaTime * -cameraRotateSpeed); // �÷��̾ �߽����� ���ϰ���

				float angleToHorizontalPlane = Vector3.Angle(transform.forward, Vector3.up) - 90f;
				if (angleToHorizontalPlane + mouseInputY * Time.deltaTime * -10f < minVerticalAngle)
				{
					mouseInputY = (minVerticalAngle - angleToHorizontalPlane) / (-10f * Time.deltaTime);
				}
				else if (angleToHorizontalPlane + mouseInputY * Time.deltaTime * -10f > maxVerticalAngle)
				{
					mouseInputY = (maxVerticalAngle - angleToHorizontalPlane) / (-10f * Time.deltaTime);
				}

				// ����ȸ�� clamp
				transform.RotateAround(targetPosition, transform.right, mouseInputY * Time.deltaTime * -10f);
			}

			// ��ֹ��� �÷��̾�Ⱥ��̸� �÷��̾� �����̷� ī�޶� �̵�
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
