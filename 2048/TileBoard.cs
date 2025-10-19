using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBoard : MonoBehaviour
{
	public GameManager gameManager;

    public Tile tilePrefab; // 에디터에서 할당
	private TileGrid grid;
    private List<Tile> tiles;
	public TileState[] tileStates; // 에디터에서 할당
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
				MoveTiles(Vector2Int.up, 0, 1, 1, 1); // Vector2Int.up는 (0,1)
			}
			else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
			{
				MoveTiles(Vector2Int.down, 0, 1, grid.height - 2, -1); // Vector2Int.down는 (0,-1)
			}
			else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
			{
				MoveTiles(Vector2Int.left, 1, 1, 0, 1); // Vector2Int.left는 (-1,0)
			}
			else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
			{
				MoveTiles(Vector2Int.right, grid.width - 2, -1, 0, 1); // Vector2Int.right는 (1,0)
			}
		}
	}

	private void MoveTiles(Vector2Int direction, int startX, int incrementX, int startY, int incrementY)
	{
		bool changed = false;

		for (int x = startX; x >= 0 && x < grid.width; x += incrementX) // 열
		{
			for (int y = startY; y >= 0 && y < grid.height; y += incrementY) // 행
			{
				TileCell cell = grid.GetCell(x, y);
				if (cell.occupied) // 해당 cell에 tile이 있다면
				{
					// 누른방향으로 tile을 이동시킨다. 하나의 tile이라도 이동했으면 true여야하므로 or연산
					changed |= MoveTile(cell.tile, direction);
				}
			}
		}

		// Animate는 한번만되므로 true가 하나라도 있으면 마지막에 한번만 호출한다
		if (changed)
		{
			StartCoroutine(WaitForChanges());
		}
	}

	private bool MoveTile(Tile tile, Vector2Int direction)
	{
		TileCell newCell = null;
		TileCell adjacent = grid.GetAdjacentCell(tile.cell, direction); // 누른 방향쪽에 있는 인접한 cell을 가져온다

		// 가장자리에 가서 grid.GetAdjacentCell(adjacent, direction)를 해서 null이 될때까지 반복해서 tile을 누른방향쪽으로 가장자리까지 이동시킨다.
		while (adjacent != null)
		{
			if (adjacent.occupied) // 인접한 cell에 이미 tile이 있으면 중지
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

		if (newCell != null) // 가장자리를 벗어난게 아니면
		{
			tile.MoveTo(newCell); // 이동
			return true;
		}

		return false;
	}

	// Animate될동안 input 안받도록
	private IEnumerator WaitForChanges()
	{
		waiting = true;
		yield return new WaitForSeconds(0.1f);
		waiting = false;

		// 합병이 끝나면 모든타일 locked를 false로 하여 다음번엔 merge가 가능하도록한다.
		foreach (var tile in tiles)
		{
			tile.locked = false;
		}

		if (tiles.Count != grid.size) // 생성된 타일의 개수가 16개가 아니라면
		{
			CreateTile(); // 합친 후 새로운 타일 랜덤생성
		}

		if (CheckForGameOver())
		{
			gameManager.GameOver();
		}
	}

	public void CreateTile()
	{
		Tile tile = Instantiate(tilePrefab, grid.transform); // 스크립트를 Instantiate하는데 오브젝트가 만들어짐
		tile.SetState(tileStates[0], 2);
		tile.Spawn(grid.GetRandomEmptyCell());
		tiles.Add(tile);
	}

	// a는 제거되고 b가 새로운
	private bool CanMerge(Tile a,  Tile b)
	{
		// 둘의 숫자가 같아야하고 b가 locked상태라면 합칠 수 없다.
		return a.number == b.number && !b.locked;
	}

	// a는 제거되고 b가 새로운
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
		// 빈 cell이 있으면 게임오버아님
		if (tiles.Count != grid.size)
		{
			return false;
		}

		// 빈 cell이 없더라도 합병가능한 cell이 있으면 게임오버아님
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

		// 빈 cell이 없고 합병가능한 cell도 없으면 게임오버
		return true;
	}
}
