using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] private GameObject mineField;

    [HideInInspector] public int rows;
    [HideInInspector] public int columns;
    private int easyLevelMinesCount = 14, mediumLevelMinesCount = 20, hardLevelMinesCount = 30;
    [HideInInspector] public int minesCount;

    [HideInInspector] public float cameraX, cameraY;

    public enum Level { EASY, MEDIUM, HARD }
    [HideInInspector] public Level level;

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

            // 게임도중 다른 scene을 load할 수도 있으므로 GameManager가 다른 scene에서도 동작하도록
            DontDestroyOnLoad(gameObject);
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        rows = 9;
        columns = 11;
        cameraX = 4f;
        cameraY = 7f;
        minesCount = 0;
        level = Level.MEDIUM;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode level)
    {
        if (scene.name == "Gameplay")
        {
            MatrixGrid.mineFields = new MineField[rows, columns];
            Camera.main.transform.position = new Vector3(cameraX, cameraY, Camera.main.transform.position.z);

            for (int x = 0; x < rows; x++)
            {
                for (int y = 0; y < columns; y++)
                {
                    GameObject mine = Instantiate(mineField, new Vector3(x, y, 0), Quaternion.identity) as GameObject;
                    mine.name = x + "-" + y;
                    MatrixGrid.mineFields[x, y] = mine.GetComponent<MineField>();
                }
            }
        }
        else
        {
            minesCount = 0;
        }
    }

    public bool CanBeMine()
    {
        switch (level)
        {
            case Level.EASY:
                if (minesCount < easyLevelMinesCount) return true;
                else return false;
                //break;

            case Level.MEDIUM:
                if (minesCount < mediumLevelMinesCount) return true;
                else return false;
                //break;

            case Level.HARD:
                if (minesCount < hardLevelMinesCount) return true;
                else return false;
                //break;

            default:
                return false;
                //break;
        }
    }

    public void IncrementMinesInGame()
    {
        minesCount++;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
