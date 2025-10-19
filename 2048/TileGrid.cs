using UnityEngine;

public class TileGrid : MonoBehaviour
{
    public TileRow[] rows {  get; private set; }
    public TileCell[] cells {  get; private set; }
    public int size => cells.Length; // 모든 셀의 개수(16개)
    public int height => rows.Length; // 행의 개수(4줄)
	public int width => size / height; // 16/4 = 4

	private void Awake()
	{
		rows = GetComponentsInChildren<TileRow>();
        cells = GetComponentsInChildren<TileCell>();
	}

	private void Start()
	{
		for (int y = 0; y < rows.Length; y++) // 1행 2행 3행 4행
		{
			for (int x = 0; x < rows[y].cells.Length; x++) // 1열 2열 3열 4열의 cell
			{
				rows[y].cells[x].coordinates = new Vector2Int(x, y); // 각각의 cell의 coordinates에 (0,0) (0,1) (0,2)... 할당
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
		if (x >= 0 && x < width && y >= 0 && y < height) // x,y 모두 0,1,2,3 중에 있다면
		{
			return rows[y].cells[x]; // 해당 cell 반환
		}
		else
		{
			return null;
		}
	}

	// occupied상태가 아닌 cell 랜덤으로 찾기
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

			// 모든 cell이 occupied 상태라면 null을 리턴하여 무한루프에 빠지지않도록
			if (index == startingIndex)
			{
				return null;
			}
		}

		return cells[index];
	}
}
