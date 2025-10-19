using Unity.Cinemachine;
using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    [SerializeField] private CinemachineImpulseSource impulseSource;
    [SerializeField] private CinemachineConfiner2D cinemachineConfiner2D;

	private void Start()
	{
		// 카메라 이동 바운더리 제한
		cinemachineConfiner2D.BoundingShape2D = GlobalManagers.Instance.GameManager.CameraBounds;
	}

	public void ShakeCamera(Vector3 shakeAmount)
    {
        impulseSource.GenerateImpulse(shakeAmount);
    }
}
