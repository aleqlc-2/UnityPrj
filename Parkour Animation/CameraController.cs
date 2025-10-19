using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform followTarget;
	[SerializeField] private float rotationSpeed = 2f;
    [SerializeField] private float distance = 5f;

	private float rotationX;
	private float rotationY;
	private float minVerticalAngle = -45f;
	private float maxVerticalAngle = 45f;

	[SerializeField] private Vector2 framingOffset;

	public Quaternion PlanarRotation => Quaternion.Euler(0, rotationY, 0);

	private void Start()
	{
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
	}

	private void Update()
	{
		rotationX += Input.GetAxis("Mouse Y") * -rotationSpeed; // 마우스 상하
		rotationX = Mathf.Clamp(rotationX, minVerticalAngle, maxVerticalAngle);
		rotationY += Input.GetAxis("Mouse X") * rotationSpeed; // 마우스 좌우

		var targetRotation = Quaternion.Euler(rotationX, rotationY, 0f);
		var focusPosition = followTarget.position + new Vector3(framingOffset.x, framingOffset.y);

		transform.position = focusPosition - targetRotation * new Vector3(0f, 0f, distance);
		transform.rotation = targetRotation;
	}
}
