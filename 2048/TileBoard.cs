using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBoard : MonoBehaviour
{
	public GameManager gameManager;

    public Tile tilePrefab; // �����Ϳ��� �Ҵ�
	private TileGrid grid;
    private List<Tile> tiles;
	public TileState[] tileStates; // �����Ϳ��� �Ҵ�
	private bool waiting;

	private void Awake()
	{
		grid = GetComponentInChildren<TileGrid>();
		tiles = new List<Tile>(16);
	}

	private void Update()
	{
		if (!waiting)
		{
			if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
			{
				MoveTiles(Vector2Int.up, 0, 1, 1, 1); // Vector2Int.up�� (0,1)
			}
			else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
			{
				MoveTiles(Vector2Int.down, 0, 1, grid.height - 2, -1); // Vector2Int.down�� (0,-1)
			}
			else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
			{
				MoveTiles(Vector2Int.left, 1, 1, 0, 1); // Vector2Int.left�� (-1,0)
			}
			else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
			{
				MoveTiles(Vector2Int.right, grid.width - 2, -1, 0, 1); // Vector2Int.right�� (1,0)
			}
		}
	}

	private void MoveTiles(Vector2Int direction, int startX, int incrementX, int startY, int incrementY)
	{
		bool changed = false;

		for (int x = startX; x >= 0 && x < grid.width; x += incrementX) // ��
		{
			for (int y = startY; y >= 0 && y < grid.height; y += incrementY) // ��
			{
				TileCell cell = grid.GetCell(x, y);
				if (cell.occupied) // �ش� cell�� tile�� �ִٸ�
				{
					// ������������ tile�� �̵���Ų��. �ϳ��� tile�̶� �̵������� true�����ϹǷ� or����
					changed |= MoveTile(cell.tile, direction);
				}
			}
		}

		// Animate�� �ѹ����ǹǷ� true�� �ϳ��� ������ �������� �ѹ��� ȣ���Ѵ�
		if (changed)
		{
			StartCoroutine(WaitForChanges());
		}
	}

	private bool MoveTile(Tile tile, Vector2Int direction)
	{
		TileCell newCell = null;
		TileCell adjacent = grid.GetAdjacentCell(tile.cell, direction); // ���� �����ʿ� �ִ� ������ cell�� �����´�

		// �����ڸ��� ���� grid.GetAdjacentCell(adjacent, direction)�� �ؼ� null�� �ɶ����� �ݺ��ؼ� tile�� �������������� �����ڸ����� �̵���Ų��.
		while (adjacent != null)
		{
			if (adjacent.occupied) // ������ cell�� �̹� tile�� ������ ����
			{
				if (CanMerge(tile, adjacent.tile))
				{
					Merge(tile, adjacent.tile);
					return true;
				}

				break;
			}

			newCell = adjacent;
			adjacent = grid.GetAdjacentCell(adjacent, direction);
		}

		if (newCell != null) // �����ڸ��� ����� �ƴϸ�
		{
			tile.MoveTo(newCell); // �̵�
			return true;
		}

		return false;
	}

	// Animate�ɵ��� input �ȹ޵���
	private IEnumerator WaitForChanges()
	{
		waiting = true;
		yield return new WaitForSeconds(0.1f);
		waiting = false;

		// �պ��� ������ ���Ÿ�� locked�� false�� �Ͽ� �������� merge�� �����ϵ����Ѵ�.
		foreach (var tile in tiles)
		{
			tile.locked = false;
		}

		if (tiles.Count != grid.size) // ������ Ÿ���� ������ 16���� �ƴ϶��
		{
			CreateTile(); // ��ģ �� ���ο� Ÿ�� ��������
		}

		if (CheckForGameOver())
		{
			gameManager.GameOver();
		}
	}

	public void CreateTile()
	{
		Tile tile = Instantiate(tilePrefab, grid.transform); // ��ũ��Ʈ�� Instantiate�ϴµ� ������Ʈ�� �������
		tile.SetState(tileStates[0], 2);
		tile.Spawn(grid.GetRandomEmptyCell());
		tiles.Add(tile);
	}

	// a�� ���ŵǰ� b�� ���ο�
	private bool CanMerge(Tile a,  Tile b)
	{
		// ���� ���ڰ� ���ƾ��ϰ� b�� locked���¶�� ��ĥ �� ����.
		return a.number == b.number && !b.locked;
	}

	// a�� ���ŵǰ� b�� ���ο�
	private void Merge(Tile a, Tile b)
	{
		tiles.Remove(a);
		a.Merge(b.cell);

		int index = Mathf.Clamp(IndexOf(b.state) + 1, 0, tileStates.Length - 1);
		int number = b.number * 2;
		b.SetState(tileStates[index], number);

		gameManager.IncreaseScore(number);
	}

	private int IndexOf(TileState state)
	{
		for (int i = 0; i < tileStates.Length; i++)
		{
			if (state == tileStates[i])
			{
				return i;
			}
		}

		return -1;
	}

	public void ClearBoard()
	{
		foreach (var cell in grid.cells)
		{
			cell.tile = null;
		}

		foreach (var tile in tiles)
		{
			Destroy(tile.gameObject);
		}

		tiles.Clear();
	}

	private bool CheckForGameOver()
	{
		// �� cell�� ������ ���ӿ����ƴ�
		if (tiles.Count != grid.size)
		{
			return false;
		}

		// �� cell�� ������ �պ������� cell�� ������ ���ӿ����ƴ�
		foreach (var tile in tiles)
		{
			TileCell up = grid.GetAdjacentCell(tile.cell, Vector2Int.up);
			TileCell down = grid.GetAdjacentCell(tile.cell, Vector2Int.down);
			TileCell left = grid.GetAdjacentCell(tile.cell, Vector2Int.left);
			TileCell right = grid.GetAdjacentCell(tile.cell, Vector2Int.right);

			if (up != null && CanMerge(tile, up.tile)) return false;
			if (down != null && CanMerge(tile, down.tile)) return false;
			if (left != null && CanMerge(tile, left.tile)) return false;
			if (right != null && CanMerge(tile, right.tile)) return false;
		}

		// �� cell�� ���� �պ������� cell�� ������ ���ӿ���
		return true;
	}
}
