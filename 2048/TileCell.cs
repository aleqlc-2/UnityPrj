using UnityEngine;

public class TileCell : MonoBehaviour
{
    public Vector2Int coordinates { get; set; } // �ܺο����б� �ܺο���set ��ΰ���
    public Tile tile { get; set; }
    public bool empty => tile == null; // �� ��ũ��Ʈ�� ������Ƽ �� tile�� null�̸� true, null�� �ƴϸ� false��ȯ
	public bool occupied => tile != null; // �� ��ũ��Ʈ�� ������Ƽ �� tile�� null�� �ƴϸ� true, null�̸� false��ȯ
}
