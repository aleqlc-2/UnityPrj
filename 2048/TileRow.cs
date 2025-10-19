using UnityEngine;

public class TileRow : MonoBehaviour
{
    public TileCell[] cells { get; private set; } // �ܺο��� �бⰡ��, ���ο����� set

	private void Awake()
	{
		cells = GetComponentsInChildren<TileCell>(); // �� ��ũ��Ʈ�� ������ ������Ʈ�� �ڽİ�ü�� ��ο��Լ� TileCell��ũ��Ʈ�� ����
	}
}
