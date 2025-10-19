using UnityEngine;

public class MovementController : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    private Vector3 velocity = Vector3.zero;
    private Vector3 rotation = Vector3.zero;
	private Rigidbody rb;
	[SerializeField] private float lookSensitivity = 6f;
	[SerializeField] private GameObject fpsCamera;
	private float CameraUpAndDownRotation = 0f;
	private float CurrentCameraUpAndDownRotation = 0f;

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
	}

	private void FixedUpdate()
	{
		// Move
		if (velocity != Vector3.zero)
		{
			rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
		}

		// Rotate
		rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation));
	
		// Camera
		if (fpsCamera != null)
		{
			CurrentCameraUpAndDownRotation -= CameraUpAndDownRotation;
			CurrentCameraUpAndDownRotation = Mathf.Clamp(CurrentCameraUpAndDownRotation, -85, 85);
			fpsCamera.transform.localEulerAngles = new Vector3(CurrentCameraUpAndDownRotation, 0, 0);
		}
	}

	private void Update()
	{
		// Move
		float _xMovement = Input.GetAxis("Horizontal");
		float _zMovement = Input.GetAxis("Vertical");
		Vector3 _movementHorizontal = transform.right * _xMovement;
		Vector3 _movementVertical = transform.forward * _zMovement;
		Vector3 _movementVelocity = (_movementHorizontal + _movementVertical).normalized * speed;
		Move(_movementVelocity);
		
		// Rotate
		float _yRotation = Input.GetAxis("Mouse X");
		Vector3 _rotationVector = new Vector3(0, _yRotation, 0) * lookSensitivity;
		Rotate(_rotationVector);

		// Camera
		float _cameraUpDownRotation = Input.GetAxis("Mouse Y") * lookSensitivity;
		RotationCamera(_cameraUpDownRotation);
	}

	private void Move(Vector3 movementVelocity)
	{
		velocity = movementVelocity;
	}

	private void Rotate(Vector3 rotationVector)
	{
		rotation = rotationVector;
	}

	private void RotationCamera(float cameraUpDownRotation)
	{
		CameraUpAndDownRotation = cameraUpDownRotation;
	}
}
