using UnityEngine;

public class ResetStaticDataManager : MonoBehaviour
{
	// GameScene���� static �̺�Ʈ���� Invoke����Ʈ�� �����Ǿ������Ƿ� ���θ޴����� �ʱ�ȭ
	private void Awake()
	{
		CuttingCounter.ResetStaticData();
		BaseCounter.ResetStaticData();
		TrashCounter.ResetStaticData();
		Player.ResetStaticData();
	}
}
