using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    private static readonly KeyCode[] SUPPORTED_KEYS = new KeyCode[]
    {
        KeyCode.A, KeyCode.B, KeyCode.C, KeyCode.D, KeyCode.E, KeyCode.F, KeyCode.G, KeyCode.H, KeyCode.I,
        KeyCode.J, KeyCode.K, KeyCode.L, KeyCode.M, KeyCode.N, KeyCode.O, KeyCode.P, KeyCode.Q, KeyCode.R,
        KeyCode.S, KeyCode.T, KeyCode.U, KeyCode.V, KeyCode.W, KeyCode.X, KeyCode.Y, KeyCode.Z
    };

	// 에디터에서 할당
	[Header("States")]
	public Tile.State emptyState;
	public Tile.State occupiedState;
	public Tile.State correctState;
	public Tile.State wrongSpotState;
	public Tile.State incorrectState;

	[Header("UI")]
	public Button newWordButton;
	public Button tryAgainButton;

    private int rowIndex;
    private int columnIndex;
    private Row[] rows;

	private string[] validWords;
	private string[] solutions;
	private string word;

	private void Awake()
	{
		rows = GetComponentsInChildren<Row>();
	}

	private void OnEnable()
	{
		tryAgainButton.gameObject.SetActive(false);
		newWordButton.gameObject.SetActive(false);
	}

	private void Start()
	{
		LoadData();
		NewGame();
	}

	private void Update()
	{
		Row currentRow = rows[rowIndex];

		if (Input.GetKeyDown(KeyCode.Backspace))
		{
			columnIndex = Mathf.Max(columnIndex - 1, 0);
			currentRow.tiles[columnIndex].Setletter('\0'); // \0는 null
			currentRow.tiles[columnIndex].SetState(emptyState); // 해당 tile에 글자가 안 적힌 상태
		}
        else if (columnIndex >= currentRow.tiles.Length) // 한 행에 다섯글자 다적으면
        {
			if (Input.GetKeyDown(KeyCode.Return)) // 엔터 or 쉬프트+엔터
			{
				SubmitRow(currentRow);
			}
        }
        else
        {
			for (int i = 0; i < SUPPORTED_KEYS.Length; i++)
			{
				if (Input.GetKeyDown(SUPPORTED_KEYS[i]))
				{
					currentRow.tiles[columnIndex].Setletter((char)SUPPORTED_KEYS[i]);
					currentRow.tiles[columnIndex].SetState(occupiedState); // 해당 tile에 글자가 적힌 상태
					columnIndex++;
					break;
				}
			}
		}
	}

	private void LoadData()
	{
		TextAsset textFile = Resources.Load("official_wordle_all") as TextAsset;
		validWords = textFile.text.Split('\n');

		textFile = Resources.Load("official_wordle_common") as TextAsset;
		solutions = textFile.text.Split('\n');
	}

	private void SetRandomWord()
	{
		word = solutions[Random.Range(0, solutions.Length)];
		word = word.ToLower().Trim();
	}

	private void SubmitRow(Row row)
	{
		string remaining = word;

		// 정확한 correctState를 찾는 for문을 wrongSpotState찾는 for문보다 위에둔다
		for (int i = 0; i < row.tiles.Length; i++)
		{
			Tile tile = row.tiles[i];

			if (tile.letter == word[i]) // 위치와 글자 모두 일치
			{
				tile.SetState(correctState);
				remaining = remaining.Remove(i, 1); // 맞춘글자는 제거, string.Remove
				remaining = remaining.Insert(i, " "); // 제거한 위치에 공백삽입(다음번에 맞출때 정확한 i 위치로 지우기 위해)
			}
			else if (!word.Contains(tile.letter)) // 해당 글자 존재하지않음
			{
				tile.SetState(incorrectState);
			}
		}

		for (int i = 0; i < row.tiles.Length; i++)
		{
			Tile tile = row.tiles[i];

			if (tile.state != correctState && tile.state != incorrectState)
			{
				if (remaining.Contains(tile.letter)) // 위치는 틀리고 글자만 맞을경우
				{
					tile.SetState(wrongSpotState);
					int index = remaining.IndexOf(tile.letter); // 위치가 틀리므로 인덱스 다시찾아서, IndexOf 왼쪽에서부터 찾기시작
					remaining = remaining.Remove(index, 1); // 맞춘글자는 제거
					remaining = remaining.Insert(index, " "); // 제거한 위치에 공백삽입
				}
			}
		}

		if (HasWon(row))
		{
			enabled = false;
		}

		rowIndex++;
		columnIndex = 0;

		if (rowIndex >= rows.Length)
		{
			enabled = false; // 스크립트만 비활성화 -> OnDisable() 호출되어 tryAgainButton, newWordButton 활성화
		}
	}

	private bool IsValidWord(string word)
	{
		for (int i = 0; i < validWords.Length; i++)
		{
			if (validWords[i] == word)
			{
				return true;
			}
		}

		return false;
	}

	private bool HasWon(Row row)
	{
		for (int i = 0; i < row.tiles.Length; i++)
		{
			if (row.tiles[i].state != correctState)
				return false;
		}

		return true;
	}

	public void NewGame()
	{
		ClearBoard();
		SetRandomWord();
		enabled = true;
	}

	public void TryAgain()
	{
		ClearBoard();
		enabled = true;
	}

	private void ClearBoard()
	{
		for (int row = 0; row < rows.Length; row++)
		{
			for (int col = 0; col < rows[row].tiles.Length; col++)
			{
				rows[row].tiles[col].Setletter('\0');
				rows[row].tiles[col].SetState(emptyState);
			}
		}

		rowIndex = 0;
		columnIndex = 0;
	}

	private void OnDisable()
	{
		tryAgainButton.gameObject.SetActive(true);
		newWordButton.gameObject.SetActive(true);
	}
}
