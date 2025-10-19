using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
	private enum Mode
	{
		LookAt,
		LookAtInverted,
		CameraForward,
		CameraForwardInverted,
	}
	[SerializeField] private Mode mode; // enum 에디터에서 고를수있게

	private void LateUpdate() // Late
	{
		switch (mode)
		{
			case Mode.LookAt:
				transform.LookAt(Camera.main.transform);
				break;
			case Mode.LookAtInverted:
				Vector3 dirFromCamera = transform.position - Camera.main.transform.position;
				transform.LookAt(transform.position + dirFromCamera);
				break;
			case Mode.CameraForward: // 사용중
				transform.forward = Camera.main.transform.forward;
				break;
			case Mode.CameraForwardInverted:
				transform.forward = -Camera.main.transform.forward;
				break;
		}
	}
}
