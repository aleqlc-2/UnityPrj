using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private Level _level; // LevelŬ������ ScriptableObjectŬ������ ��ӹް� �����Ƿ� �����Ϳ��� SO�Ҵ簡��
	[SerializeField] private BGCell _bgCellPrefab; // ��׶��� ��
    [SerializeField] private Block _blockPrefab; // ��׶��� �� ���� ������ block
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

    // BGCell and BGCellGrid(BGCell�� ���� 2�����迭)
    private void SpawnGrid()
    {
        bgCellGrid = new BGCell[_level.Rows, _level.Columns]; // [3,3]
        
        for (int i = 0; i < _level.Rows; i++)
        {
            for (int j = 0; j < _level.Columns; j++)
            {
                BGCell bgcell = Instantiate(_bgCellPrefab); // cell ����
                bgcell.transform.position = new Vector3(j + 0.5f, i + 0.5f, 0f); // cell�� ��ġ
                bgcell.Init(_level.Data[i * _level.Columns + j]); // cell�� sprite�� _blockedSprite���� _emptySprite���� ����
				bgCellGrid[i, j] = bgcell; // ������ cell�� 2���� �迭�� �ִ´�
            }
        }
    }

    private void SpawnBlocks()
    {
        // startPos
        Vector3 startPos = Vector3.zero;
        startPos.x = 0.25f + (_level.Columns - _level.BlockColumns * _blockSpawnSize) * 0.5f; // 0.25 + (3 - 3 * 0.5) * 0.5 = 1
        startPos.y = -_level.BlockRows * _blockSpawnSize + 0.25f - 1f; // -3 * 0.5 + 0.25 - 1 = -2.25

        // �θ� Block 3��
        for (int i = 0; i < _level.Blocks.Count; i++) // for 3ȸ
        {
            Block block = Instantiate(_blockPrefab);
            Vector2Int blockPos = _level.Blocks[i].StartPos; // (1,1) (1,0) (1,2)
            Vector3 blockSpawnPos = startPos + new Vector3(blockPos.y, blockPos.x, 0) * _blockSpawnSize; // (1,-2.25)+(1,1)*0.5=(1.5,-1.75)  (1,-2.25)+(0,1)*0.5=(1,-1.75)  (1,-2.25)+(2,1)*0.5=(2,-1.75)
            block.transform.position = blockSpawnPos;
            block.Init(_level.Blocks[i].BlockPositions, blockSpawnPos, _level.Blocks[i].Id); // �θ�� 1���� �ڽĺ�3�� (0,0)(1,0)(-1,0) Id 3 1 2
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
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y); // Z�� �ڸ���

        if (Input.GetMouseButtonDown(0)) // ùŬ��
        {
			Debug.Log("DOWN : " + mousePos2D);
			RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
            if (!hit) return;

            currentBlock = hit.collider.transform.parent.GetComponent<Block>();
            if (currentBlock == null) return;

            currentPos = mousePos2D;
            previousPos = mousePos2D;

            currentBlock.ElevateSprites(); // sortingOrder 1��
			currentBlock.transform.localScale = Vector3.one * _blockHighLightSize; // ��ũ�� ũ�� ���̶���Ʈ
            if (gridBlocks.Contains(currentBlock))
            {
                gridBlocks.Remove(currentBlock);
            }
			UpdateFilled();
			ResetHighLight();
            UpdateHighLight();
        }
        else if (Input.GetMouseButton(0) && currentBlock != null) // ����ä�� �巡���Ҷ�
        {
			Debug.Log("DRAG : " + mousePos2D);
			currentPos = mousePos2D;
            currentBlock.UpdatePos(currentPos - previousPos); // ���� ���콺 ������Բ�
            previousPos = currentPos;
            ResetHighLight();
            UpdateHighLight();
        }
        else if (Input.GetMouseButtonUp(0) && currentBlock != null) // �� ��
        {
            Debug.Log("UP : " + mousePos2D);
            currentBlock.ElevateSprites(true); // sortingOrder 0����(BGCell���� sortingOrder�� -10���� �������Ҵ�)

			if (IsCorrectMove())
            {
                currentBlock.UpdateCorrectMove(); // ���� Cell�� ��Ȯ�� ��ġ�ǵ���
                currentBlock.transform.localScale = Vector3.one * _blockPutSize;
                gridBlocks.Add(currentBlock); // ������ ���� �������� List�� �ִ´�
            }
            else if (mousePos2D.y < 0) // ���콺�� BGCell �� �Ʒ� �� ������ ��������
            {
                currentBlock.UpdateStartMove(); // �巡���ϴ� �� startPos�� �� ����ġ
				currentBlock.transform.localScale = Vector3.one * _blockSpawnSize;
            }
			else // IsCorrectMove()�� false�̰� mousePos2D.y > 0�϶�
			{
                currentBlock.UpdateIncorrectMove(); // �巡���ϴ� �� startPos�� �� ����ġ
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
                if (!bgCellGrid[i, j].IsBlocked) // IsBlocked�� false�� BGCell��
				{
                    bgCellGrid[i, j].ResetHighLight(); // ResetHighLight �Լ� ȣ��
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
				if (!bgCellGrid[i, j].IsBlocked) // IsBlocked�� false�� BGCell��
				{
					bgCellGrid[i, j].IsFilled = false; // IsFilled�� false��
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

	// �巡���ϰ��ִ� ���� �ڽĺ�3���� ��� Correct�̸� �ʷϻ�, �ϳ��� �ƴϸ� ���������� BGCell�� ���̶���Ʈ�ǵ���
	private void UpdateHighLight()
    {
        bool isCorrect = IsCorrectMove();
        foreach (var pos in currentBlock.BlockPositions()) // �巡���ϰ��ִ� ������ ������ġ �����ͼ�
		{
            if (IsValidPos(pos)) // ��׶��� �� �������� ��ġ�̸�
            {
				// Cell�� correctColor�� ���̶���Ʈ
                // pos�� Vector2Int�̹Ƿ� �Ҽ��� �ڵ� ����Ǿ� �ش� �迭�� �ִ� BGCell�� highlight�ȴ�
				bgCellGrid[pos.x, pos.y].UpdateHighlight(isCorrect);
            }
        }
    }

    private bool IsCorrectMove()
    {
		// �� ���� ���̶� ��ȿ�������� ��ġ�ų� �ش� ��ġ�� Cell�� ä���������� false��ȯ
		foreach (var pos in currentBlock.BlockPositions()) // �巡���ϰ��ִ� ������ ������ġ �����ͼ�
        {
            if (!IsValidPos(pos) || bgCellGrid[pos.x, pos.y].IsFilled)
                return false;
        }

		// �� ���� �� ��� ���� �� ��� �ƴϸ� true��ȯ
		return true;
    }

    // ��׶��� ������ 0~3�̸� �����ǿ� �����Ƿ� �� �Լ��� true��°��� �巡���ϴ� ���� ��׶��� �� ���� �ִٴ°���
    private bool IsValidPos(Vector2Int pos)
    {
        return pos.x >= 0 && pos.y >= 0 && pos.x < _level.Rows && pos.y < _level.Columns;
    }

    private void CheckWin()
    {
        // �ϳ��� ��ä���� Cell�� ������ lose
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
        StartCoroutine(GameWin()); // Cell�� ����ä��� win
    }

    private IEnumerator GameWin()
    {
        yield return new WaitForSeconds(2f);
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}
