using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private Level _level; // Level클래스가 ScriptableObject클래스를 상속받고 있으므로 에디터에서 SO할당가능
	[SerializeField] private BGCell _bgCellPrefab; // 백그라운드 셀
    [SerializeField] private Block _blockPrefab; // 백그라운드 셀 위에 놓여질 block
    [SerializeField] private float _blockSpawnSize; // 0.5
    [SerializeField] private float _blockHighLightSize; // 0.8
    [SerializeField] private float _blockPutSize; // 1

    private BGCell[, ] bgCellGrid;
    private bool hasGameFinished;
    private Block currentBlock;
    private Vector2 currentPos, previousPos;
    private List<Block> gridBlocks;

	private void Awake()
	{
		Instance = this;
        hasGameFinished = false;
        gridBlocks = new List<Block>();
        SpawnGrid();
        SpawnBlocks();
	}

    // BGCell and BGCellGrid(BGCell들 넣은 2차원배열)
    private void SpawnGrid()
    {
        bgCellGrid = new BGCell[_level.Rows, _level.Columns]; // [3,3]
        
        for (int i = 0; i < _level.Rows; i++)
        {
            for (int j = 0; j < _level.Columns; j++)
            {
                BGCell bgcell = Instantiate(_bgCellPrefab); // cell 생성
                bgcell.transform.position = new Vector3(j + 0.5f, i + 0.5f, 0f); // cell의 위치
                bgcell.Init(_level.Data[i * _level.Columns + j]); // cell의 sprite가 _blockedSprite인지 _emptySprite인지 결정
				bgCellGrid[i, j] = bgcell; // 생성된 cell을 2차원 배열에 넣는다
            }
        }
    }

    private void SpawnBlocks()
    {
        // startPos
        Vector3 startPos = Vector3.zero;
        startPos.x = 0.25f + (_level.Columns - _level.BlockColumns * _blockSpawnSize) * 0.5f; // 0.25 + (3 - 3 * 0.5) * 0.5 = 1
        startPos.y = -_level.BlockRows * _blockSpawnSize + 0.25f - 1f; // -3 * 0.5 + 0.25 - 1 = -2.25

        // 부모 Block 3개
        for (int i = 0; i < _level.Blocks.Count; i++) // for 3회
        {
            Block block = Instantiate(_blockPrefab);
            Vector2Int blockPos = _level.Blocks[i].StartPos; // (1,1) (1,0) (1,2)
            Vector3 blockSpawnPos = startPos + new Vector3(blockPos.y, blockPos.x, 0) * _blockSpawnSize; // (1,-2.25)+(1,1)*0.5=(1.5,-1.75)  (1,-2.25)+(0,1)*0.5=(1,-1.75)  (1,-2.25)+(2,1)*0.5=(2,-1.75)
            block.transform.position = blockSpawnPos;
            block.Init(_level.Blocks[i].BlockPositions, blockSpawnPos, _level.Blocks[i].Id); // 부모블럭 1개당 자식블럭3개 (0,0)(1,0)(-1,0) Id 3 1 2
        }

		// Camera orthographicSize
		float maxColumns = Mathf.Max(_level.Columns, _level.BlockColumns * _blockSpawnSize); // max(3, 3*0.5) = 3
        float maxRows = _level.Rows + 2f + _level.BlockRows * _blockSpawnSize; // 3 + 2 + 3*0.5 = 6.5
        Camera.main.orthographicSize = Mathf.Max(maxColumns, maxRows) * 0.65f; // max(3, 6.5) * 0.65 = 4.225

		// Camera position
		Vector3 camPos = Camera.main.transform.position;
        camPos.x = _level.Columns * 0.5f; // 3 * 0.5 = 1.5
        camPos.y = (_level.Rows + 0.5f + startPos.y) * 0.5f; // (3 + 0.5 - 2.25) * 0.5 = 0.625
        Camera.main.transform.position = camPos;
    }

	private void Update()
	{
        if (hasGameFinished) return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y); // Z는 자른다

        if (Input.GetMouseButtonDown(0)) // 첫클릭
        {
			Debug.Log("DOWN : " + mousePos2D);
			RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
            if (!hit) return;

            currentBlock = hit.collider.transform.parent.GetComponent<Block>();
            if (currentBlock == null) return;

            currentPos = mousePos2D;
            previousPos = mousePos2D;

            currentBlock.ElevateSprites(); // sortingOrder 1로
			currentBlock.transform.localScale = Vector3.one * _blockHighLightSize; // 블럭크기 크게 하이라이트
            if (gridBlocks.Contains(currentBlock))
            {
                gridBlocks.Remove(currentBlock);
            }
			UpdateFilled();
			ResetHighLight();
            UpdateHighLight();
        }
        else if (Input.GetMouseButton(0) && currentBlock != null) // 누른채로 드래그할때
        {
			Debug.Log("DRAG : " + mousePos2D);
			currentPos = mousePos2D;
            currentBlock.UpdatePos(currentPos - previousPos); // 블럭이 마우스 따라오게끔
            previousPos = currentPos;
            ResetHighLight();
            UpdateHighLight();
        }
        else if (Input.GetMouseButtonUp(0) && currentBlock != null) // 뗄 때
        {
            Debug.Log("UP : " + mousePos2D);
            currentBlock.ElevateSprites(true); // sortingOrder 0으로(BGCell들의 sortingOrder는 -10으로 에디터할당)

			if (IsCorrectMove())
            {
                currentBlock.UpdateCorrectMove(); // 블럭이 Cell에 정확히 위치되도록
                currentBlock.transform.localScale = Vector3.one * _blockPutSize;
                gridBlocks.Add(currentBlock); // 놓아진 블럭을 지역변수 List에 넣는다
            }
            else if (mousePos2D.y < 0) // 마우스가 BGCell 젤 아래 블럭 밑으로 내려가면
            {
                currentBlock.UpdateStartMove(); // 드래그하던 블럭 startPos로 블럭 원위치
				currentBlock.transform.localScale = Vector3.one * _blockSpawnSize;
            }
			else // IsCorrectMove()가 false이고 mousePos2D.y > 0일때
			{
                currentBlock.UpdateIncorrectMove(); // 드래그하던 블럭 startPos로 블럭 원위치
				if (currentBlock.CurrentPos.y > 0)
                {
                    gridBlocks.Add(currentBlock);
                    currentBlock.transform.localScale = Vector3.one * _blockPutSize;
                }
				else // currentBlock.CurrentPos.y < 0
				{
                    currentBlock.transform.localScale = Vector3.one * _blockSpawnSize;
                }
            }

            currentBlock = null;
            ResetHighLight();
            UpdateFilled();
            CheckWin();
		}
	}

    private void ResetHighLight()
    {
        for (int i = 0; i < _level.Rows; i++)
        {
            for (int j = 0; j < _level.Columns; j++)
            {
                if (!bgCellGrid[i, j].IsBlocked) // IsBlocked가 false인 BGCell만
				{
                    bgCellGrid[i, j].ResetHighLight(); // ResetHighLight 함수 호출
				}
            }
        }
    }

    private void UpdateFilled()
    {
		for (int i = 0; i < _level.Rows; i++)
		{
			for (int j = 0; j < _level.Columns; j++)
			{
				if (!bgCellGrid[i, j].IsBlocked) // IsBlocked가 false인 BGCell만
				{
					bgCellGrid[i, j].IsFilled = false; // IsFilled를 false로
				}
			}
		}

        foreach (var block in gridBlocks)
        {
            foreach (var pos in block.BlockPositions())
            {
                if (IsValidPos(pos))
                {
                    bgCellGrid[pos.x, pos.y].IsFilled = true;
                }
            }
        }
	}

	// 드래그하고있는 블럭의 자식블럭3개가 모두 Correct이면 초록색, 하나라도 아니면 빨강색으로 BGCell이 하이라이트되도록
	private void UpdateHighLight()
    {
        bool isCorrect = IsCorrectMove();
        foreach (var pos in currentBlock.BlockPositions()) // 드래그하고있는 블럭들의 현재위치 가져와서
		{
            if (IsValidPos(pos)) // 백그라운드 셀 범위내의 위치이면
            {
				// Cell을 correctColor로 하이라이트
                // pos가 Vector2Int이므로 소수점 자동 절삭되어 해당 배열에 있는 BGCell이 highlight된다
				bgCellGrid[pos.x, pos.y].UpdateHighlight(isCorrect);
            }
        }
    }

    private bool IsCorrectMove()
    {
		// 한 개의 블럭이라도 유효하지않은 위치거나 해당 위치의 Cell이 채워져있으면 false반환
		foreach (var pos in currentBlock.BlockPositions()) // 드래그하고있는 블럭들의 현재위치 가져와서
        {
            if (!IsValidPos(pos) || bgCellGrid[pos.x, pos.y].IsFilled)
                return false;
        }

		// 세 개의 블럭 모두 위의 두 경우 아니면 true반환
		return true;
    }

    // 백그라운드 셀들이 0~3미만 포지션에 있으므로 이 함수가 true라는것은 드래그하는 블럭이 백그라운드 셀 위에 있다는것임
    private bool IsValidPos(Vector2Int pos)
    {
        return pos.x >= 0 && pos.y >= 0 && pos.x < _level.Rows && pos.y < _level.Columns;
    }

    private void CheckWin()
    {
        // 하나라도 안채워진 Cell이 있으면 lose
        for (int i = 0; i < _level.Rows; i++)
        {
            for (int j = 0; j < _level.Columns; j++)
            {
                if (!bgCellGrid[i, j].IsFilled)
                {
                    return;
                }
            }
        }

        hasGameFinished = true;
        StartCoroutine(GameWin()); // Cell을 전부채우면 win
    }

    private IEnumerator GameWin()
    {
        yield return new WaitForSeconds(2f);
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}
