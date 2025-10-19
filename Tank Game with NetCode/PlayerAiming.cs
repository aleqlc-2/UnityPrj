using Unity.Netcode;
using UnityEngine;

public class PlayerAiming : NetworkBehaviour
{
    [SerializeField] private InputReader inputReader;
    [SerializeField] private Transform turretTransform;

	private void LateUpdate()
	{
		if (!IsOwner) return; // ȣ��Ʈ�� Ŭ��� ���� �Ʒ��ڵ� ����

		Vector2 aimScreenPosition = inputReader.AimPosition;
		Vector2 aimWorldPosition = Camera.main.ScreenToWorldPoint(aimScreenPosition);

		// �������� �ѱ��� ��Ȯ�� y���� ���ϰ������Ƿ� transform.up���� �״�� ������ ����
		turretTransform.up = new Vector2(aimWorldPosition.x - turretTransform.position.x, aimWorldPosition.y - turretTransform.position.y);
	}
}
