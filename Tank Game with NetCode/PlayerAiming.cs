using Unity.Netcode;
using UnityEngine;

public class PlayerAiming : NetworkBehaviour
{
    [SerializeField] private InputReader inputReader;
    [SerializeField] private Transform turretTransform;

	private void LateUpdate()
	{
		if (!IsOwner) return; // 호스트건 클라건 각자 아래코드 실행

		Vector2 aimScreenPosition = inputReader.AimPosition;
		Vector2 aimWorldPosition = Camera.main.ScreenToWorldPoint(aimScreenPosition);

		// 프리펩의 총구가 정확히 y축을 향하고있으므로 transform.up에다 그대로 방향을 대입
		turretTransform.up = new Vector2(aimWorldPosition.x - turretTransform.position.x, aimWorldPosition.y - turretTransform.position.y);
	}
}
