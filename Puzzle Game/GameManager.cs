using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private GameState gameState;

    // 배열 전체 인스턴스화(배열크기결정). 배열요소가 각각 class이므로 각자 객체화해줘야함.
    private PuzzlePiece[,] Matrix = new PuzzlePiece[GameVariables.MaxRows, GameVariables.MaxColumns];

    private int puzzleIndex;
    private GameObject[] puzzlePieces;
    private Sprite[] puzzleImages;

    private PuzzlePiece PieceToAnimate;
    private Vector3 screenPositionToAnimate;
    private int toAnimateRow, toAnimateColumn;
    private float animSpeed = 10f;

    void Awake()
    {
        MakeSingleton();
    }

    private void MakeSingleton()
    {
        if (instance != null)
            Destroy(gameObject);
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += SceneLoaded;
    }

    void SceneLoaded(Scene scene, LoadSceneMode sceneMode)
    {
        if (SceneManager.GetActiveScene().name == "Gameplay")
        {
            if (puzzleIndex > 0)
            {
                LoadPuzzle();
                GameStarted();
            }
        }
    }

    void Start()
    {
        puzzleIndex = -1;
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name == "Gameplay")
        {
            switch (gameState)
            {
                case GameState.Playing:
                    CheckInput();
                    break;

                case GameState.Animating:
                    AnimateMovement(PieceToAnimate, Time.deltaTime);
                    CheckIfAnimationEnded();
                    break;

                case GameState.End:
                    
                    break;
            }
        }
    }

    private void LoadPuzzle()
    {
        puzzleImages = Resources.LoadAll<Sprite>("Sprites/BG " + puzzleIndex);

        puzzlePieces = GameObject.Find("Puzzle Holder").GetComponent<PuzzleHolder>().puzzlePieces;

        for (int i = 0; i < puzzlePieces.Length; i++)
        {
            puzzlePieces[i].GetComponent<SpriteRenderer>().sprite = puzzleImages[i];
        }
    }

    private void GameStarted()
    {
        // 조각중에 랜덤으로 하나뽑아서 비활성화
        int index = Random.Range(0, GameVariables.MaxSize);
        puzzlePieces[index].SetActive(false);

        for (int row = 0; row < GameVariables.MaxRows; row++)
        {
            for (int column = 0; column < GameVariables.MaxColumns; column++)
            {
                // GameVariables.MaxColumns 곱하는이유는 배열에 일렬로 들어가 있으므로
                if (puzzlePieces[row * GameVariables.MaxColumns + column].activeInHierarchy)
                {
                    Vector3 point = GetScreenCoordinatesFromViewport(row, column);
                    puzzlePieces[row * GameVariables.MaxColumns + column].transform.position = point;

                    // 배열요소 각각을 인스턴스화(배열요소 각각이 클래스이므로)
                    // 객체가 들어갔으므로 프로퍼티가 전부 null이라도 Matrix[row, column]는 null이 아님
                    Matrix[row, column] = new PuzzlePiece();
                    Matrix[row, column].obj = puzzlePieces[row * GameVariables.MaxColumns + column];
                    Matrix[row, column].OriginalRow = row;
                    Matrix[row, column].OriginalColumn = column;
                }
                else
                {
                    Matrix[row, column] = null; // 객체를 넣지 않고 null을 넣었으므로 null임
                }
            }
        }

        Shuffle();
        gameState = GameState.Playing;
    }

    private void Shuffle()
    {
        for (int row = 0; row < GameVariables.MaxRows; row++)
        {
            for (int column = 0; column < GameVariables.MaxColumns; column++)
            {
                if (Matrix[row, column] == null) continue;

                int random_row = Random.Range(0, GameVariables.MaxRows);
                int random_column = Random.Range(0, GameVariables.MaxColumns);

                Swap(row, column, random_row, random_column);
            }
        }
    }

    private void Swap(int row, int column, int random_row, int random_column)
    {
        PuzzlePiece temp = Matrix[row, column];
        Matrix[row, column] = Matrix[random_row, random_column];
        Matrix[random_row, random_column] = temp;

        if (Matrix[row, column] != null)
        {
            Matrix[row, column].obj.transform.position = GetScreenCoordinatesFromViewport(row, column);
            Matrix[row, column].CurrentRow = row;
            Matrix[row, column].CurrentColumn = column;
        }

        Matrix[random_row, random_column].obj.transform.position = GetScreenCoordinatesFromViewport(random_row, random_column);
        Matrix[random_row, random_column].CurrentRow = random_row;
        Matrix[random_row, random_column].CurrentColumn = random_column;
    }

    private Vector3 GetScreenCoordinatesFromViewport(int row, int column)
    {
        // 뷰포트는 좌하단 0,0 우상단 1,1 이므로 row, column에 따라 값을 맞춰줘야함
        // 현재 맞춰준 값은 column이 세로로 깔리도록 했음
        Vector3 point = Camera.main.ViewportToWorldPoint(new Vector3(0.225f * row, 1 - 0.235f * column, 0));
        point.z = 0;
        return point;
    }

    private void CheckInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            // hit.collider는 ray에 맞은 콜라이더.
            // 즉, 이렇게 input 짜려면 이미지에 전부 콜라이더 넣어야함
            if (hit.collider != null)
            {
                string[] parts = hit.collider.gameObject.name.Split('-');
                int rowPart = int.Parse(parts[1]);
                int columnPart = int.Parse(parts[2]);

                int rowFound = -1;
                int columnFound = -1;

                for (int row = 0; row < GameVariables.MaxRows; row++)
                {
                    if (rowFound != -1) break;

                    for (int column = 0; column < GameVariables.MaxColumns; column++)
                    {
                        if (columnFound != -1) break;
                        if (Matrix[row, column] == null) continue;

                        // 셔플해도 name은 안바꿨으므로 Original과 비교
                        if (Matrix[row, column].OriginalRow == rowPart && Matrix[row, column].OriginalColumn == columnPart)
                        {
                            rowFound = row; // 클릭한 이미지의 실제 행
                            columnFound = column; // 클릭한 이미지의 실제 열
                        }
                    }
                }

                bool pieceFound = false;

                if (rowFound > 0 && Matrix[rowFound - 1, columnFound] == null) // 왼쪽 비었으면
                {
                    pieceFound = true;
                    toAnimateRow = rowFound - 1;
                    toAnimateColumn = columnFound;
                }
                else if (columnFound > 0 && Matrix[rowFound, columnFound - 1] == null) // 위쪽 비었으면
                {
                    pieceFound = true;
                    toAnimateRow = rowFound;
                    toAnimateColumn = columnFound - 1;
                }
                else if (rowFound < GameVariables.MaxRows - 1 && Matrix[rowFound + 1, columnFound] == null) // 오른쪽 비었으면
                {
                    pieceFound = true;
                    toAnimateRow = rowFound + 1;
                    toAnimateColumn = columnFound;
                }
                else if (columnFound < GameVariables.MaxColumns - 1 && Matrix[rowFound, columnFound + 1] == null) // 아래쪽 비었으면
                {
                    pieceFound = true;
                    toAnimateRow = rowFound;
                    toAnimateColumn = columnFound + 1;
                }

                if (pieceFound)
                {
                    screenPositionToAnimate = GetScreenCoordinatesFromViewport(toAnimateRow, toAnimateColumn); // 이동할 위치
                    PieceToAnimate = Matrix[rowFound, columnFound]; // 이동시킬 이미지
                    gameState = GameState.Animating;
                }
            }
        }
    }

    private void AnimateMovement(PuzzlePiece toMove, float time)
    {
        toMove.obj.transform.position = Vector2.MoveTowards(toMove.obj.transform.position,
                                                            screenPositionToAnimate,
                                                            animSpeed * time);
    }

    private void CheckIfAnimationEnded()
    {
        if (Vector2.Distance(PieceToAnimate.obj.transform.position, screenPositionToAnimate) < 0.1f)
        {
            Swap(PieceToAnimate.CurrentRow, PieceToAnimate.CurrentColumn, toAnimateRow, toAnimateColumn);
            gameState = GameState.Playing;

            CheckForVictory();
        }
    }

    private void CheckForVictory()
    {
        // 하나라도 원래위치로 돌아가지 않았으면 게임안끝남
        for (int row = 0; row < GameVariables.MaxRows; row++)
        {
            for (int column = 0; column < GameVariables.MaxColumns; column++)
            {
                if (Matrix[row, column] == null) continue;
                if (Matrix[row, column].CurrentRow != Matrix[row, column].OriginalRow ||
                    Matrix[row, column].CurrentColumn != Matrix[row, column].OriginalColumn)
                {
                    return;
                }
            }
        }

        gameState = GameState.End;
    }

    public void SetPuzzleIndex(int puzzleIndex)
    {
        this.puzzleIndex = puzzleIndex;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= SceneLoaded;
    }
}
