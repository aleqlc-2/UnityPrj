using UnityEngine;

public class SpawnOnDestroy : MonoBehaviour // client�Ѿ� �����鿡 ���� ��ũ��Ʈ
{
    [SerializeField] private GameObject prefab; // ��ƼŬ

	// �Ѿ� �������
	private void OnDestroy()
	{
		if (!gameObject.scene.isLoaded) return; // �Ѿ˿�����Ʈ�� �����ִ� scene�� �ε�����ʾ����� ����

		Instantiate(prefab, transform.position, Quaternion.identity); // ��ƼŬ ����
	}
}
