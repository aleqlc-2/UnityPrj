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

	// �����Ϳ��� �Ҵ�
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
			currentRow.tiles[columnIndex].Setletter('\0'); // \0�� null
			currentRow.tiles[columnIndex].SetState(emptyState); // �ش� tile�� ���ڰ� �� ���� ����
		}
        else if (columnIndex >= currentRow.tiles.Length) // �� �࿡ �ټ����� ��������
        {
			if (Input.GetKeyDown(KeyCode.Return)) // ���� or ����Ʈ+����
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
					currentRow.tiles[columnIndex].SetState(occupiedState); // �ش� tile�� ���ڰ� ���� ����
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

		// ��Ȯ�� correctState�� ã�� for���� wrongSpotStateã�� for������ �����д�
		for (int i = 0; i < row.tiles.Length; i++)
		{
			Tile tile = row.tiles[i];

			if (tile.letter == word[i]) // ��ġ�� ���� ��� ��ġ
			{
				tile.SetState(correctState);
				remaining = remaining.Remove(i, 1); // ������ڴ� ����, string.Remove
				remaining = remaining.Insert(i, " "); // ������ ��ġ�� �������(�������� ���⶧ ��Ȯ�� i ��ġ�� ����� ����)
			}
			else if (!word.Contains(tile.letter)) // �ش� ���� ������������
			{
				tile.SetState(incorrectState);
			}
		}

		for (int i = 0; i < row.tiles.Length; i++)
		{
			Tile tile = row.tiles[i];

			if (tile.state != correctState && tile.state != incorrectState)
			{
				if (remaining.Contains(tile.letter)) // ��ġ�� Ʋ���� ���ڸ� �������
				{
					tile.SetState(wrongSpotState);
					int index = remaining.IndexOf(tile.letter); // ��ġ�� Ʋ���Ƿ� �ε��� �ٽ�ã�Ƽ�, IndexOf ���ʿ������� ã�����
					remaining = remaining.Remove(index, 1); // ������ڴ� ����
					remaining = remaining.Insert(index, " "); // ������ ��ġ�� �������
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
			enabled = false; // ��ũ��Ʈ�� ��Ȱ��ȭ -> OnDisable() ȣ��Ǿ� tryAgainButton, newWordButton Ȱ��ȭ
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
