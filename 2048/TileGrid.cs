using UnityEngine;

public class TileGrid : MonoBehaviour
{
    public TileRow[] rows {  get; private set; }
    public TileCell[] cells {  get; private set; }
    public int size => cells.Length; // ��� ���� ����(16��)
    public int height => rows.Length; // ���� ����(4��)
	public int width => size / height; // 16/4 = 4

	private void Awake()
	{
		rows = GetComponentsInChildren<TileRow>();
        cells = GetComponentsInChildren<TileCell>();
	}

	private void Start()
	{
		for (int y = 0; y < rows.Length; y++) // 1�� 2�� 3�� 4��
		{
			for (int x = 0; x < rows[y].cells.Length; x++) // 1�� 2�� 3�� 4���� cell
			{
				rows[y].cells[x].coordinates = new Vector2Int(x, y); // ������ cell�� coordinates�� (0,0) (0,1) (0,2)... �Ҵ�
			}
		}
	}

	public TileCell GetAdjacentCell(TileCell cell, Vector2Int direction)
	{
		Vector2Int coordinates = cell.coordinates;
		coordinates.x += direction.x;
		coordinates.y -= direction.y;

		return GetCell(coordinates);
	}

	public TileCell GetCell(Vector2Int coordinates)
	{
		return GetCell(coordinates.x, coordinates.y);
	}

	public TileCell GetCell(int x, int y)
	{
		if (x >= 0 && x < width && y >= 0 && y < height) // x,y ��� 0,1,2,3 �߿� �ִٸ�
		{
			return rows[y].cells[x]; // �ش� cell ��ȯ
		}
		else
		{
			return null;
		}
	}

	// occupied���°� �ƴ� cell �������� ã��
	public TileCell GetRandomEmptyCell()
	{
		int index = Random.Range(0, cells.Length);
		int startingIndex = index;

		while (cells[index].occupied)
		{
			index++;
			if (index >= cells.Length)
			{
				index = 0;
			}

			// ��� cell�� occupied ���¶�� null�� �����Ͽ� ���ѷ����� �������ʵ���
			if (index == startingIndex)
			{
				return null;
			}
		}

		return cells[index];
	}
}
