using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    private ShotCountText shotCountText;

    public Text ballsCountText;

    public List<GameObject> levels;

    public GameObject[] block;

    private GameObject level1;
    private GameObject level2;
    
    private Vector2 level1Pos;
    private Vector2 level2Pos;

    public int shotCount;
    public int ballsCount;
    public int score;

    private GameObject ballsContainer;
    public GameObject gameOver;

    private bool firstShot;

    void Awake()
    {
        shotCountText = GameObject.Find("ShotCountText").GetComponent<ShotCountText>();
        ballsCountText = GameObject.Find("BallCountText").GetComponent<Text>();
        ballsContainer = GameObject.Find("BallsContainer");
    }

    void Start()
    {
        PlayerPrefs.DeleteKey("Level");

        ballsCount = PlayerPrefs.GetInt("BallsCount", 5);
        ballsCountText.text = ballsCount.ToString();

        Physics2D.gravity = new Vector2(0, -17); // ?

        SpawnLevel();

        GameObject.Find("Cannon").GetComponent<Animator>().SetBool("MoveIn", true);
    }

    void Update()
    {
        if (ballsContainer.transform.childCount == 0 && shotCount == 4) // 게임오버
        {
            gameOver.SetActive(true);
            GameObject.Find("Cannon").GetComponent<Animator>().SetBool("MoveIn", false);
        }

        if (shotCount > 2) firstShot = false;
        else firstShot = true;

        CheckBlocks();
    }

    // 단계 클리어했는지 검사
    private void CheckBlocks()
    {
        block = GameObject.FindGameObjectsWithTag("Block");

        if (block.Length < 1)
        {
            RemoveBalls();
            PlayerPrefs.SetInt("Level", PlayerPrefs.GetInt("Level") + 1);
            SpawnLevel();

            // 게이지 채워서 늘린 ball의 개수를 저장하였다가 게임재시작시 불러오기위함.
            // 이 코드 적용하려면 PlayerPrefs.DeleteKey("Level"); 코드 지워야함
            if (ballsCount >= PlayerPrefs.GetInt("BallsCount", 5))
                PlayerPrefs.SetInt("BallsCount", ballsCount);

            if (firstShot) score += 5; // 첫번째 발사에 단계 클리어하면 5점
            else score += 3; // 두,세번째 발사에 클리어하면 3점
        }
    }

    // 단계 클리어하면 발사된 ball 모두 제거
    private void RemoveBalls()
    {
        GameObject[] balls = GameObject.FindGameObjectsWithTag("Ball");

        for (int i = 0; i < balls.Length; i++)
        {
            Destroy(balls[i]);
        }
    }

    private void SpawnLevel()
    {
        if (PlayerPrefs.GetInt("Level", 0) == 0) SpawnNewLevel(0, 17, 3, 5);
        if (PlayerPrefs.GetInt("Level") == 1) SpawnNewLevel(1, 18, 3, 5);
        if (PlayerPrefs.GetInt("Level") == 2) SpawnNewLevel(2, 19, 3, 6);
        if (PlayerPrefs.GetInt("Level") == 3) SpawnNewLevel(5, 20, 4, 7);
        if (PlayerPrefs.GetInt("Level") == 4) SpawnNewLevel(12, 28, 5, 8);
        if (PlayerPrefs.GetInt("Level") == 5) SpawnNewLevel(14, 29, 7, 10);
        if (PlayerPrefs.GetInt("Level") == 6) SpawnNewLevel(15, 30, 6, 12);
        if (PlayerPrefs.GetInt("Level") == 7) SpawnNewLevel(16, 31, 9, 15);
    }

    // 다음단계 세팅
    private void SpawnNewLevel(int numberLevel1, int numberLevel2, int min, int max)
    {
        // 1단계는 RotateCameraToFront 호출되지 않도록
        if (shotCount > 1)
            Camera.main.GetComponent<CameraTransition>().RotateCameraToFront();

        shotCount = 1;

        level1Pos = new Vector2(3.5f, 1f);
        level2Pos = new Vector2(3.5f, -3.4f);

        level1 = levels[numberLevel1];
        level2 = levels[numberLevel2];

        Instantiate(level1, level1Pos, Quaternion.identity);
        Instantiate(level2, level2Pos, Quaternion.identity);

        SetBlocksCount(min, max);
    }

    // 각각의 블록이 몇대 맞아야 파괴될 것인지 결정
    private void SetBlocksCount(int min, int max)
    {
        block = GameObject.FindGameObjectsWithTag("Block");

        for (int i = 0; i < block.Length; i++)
        {
            int count = Random.Range(min, max);
            block[i].GetComponent<Block>().SetStartingCount(count);
        }
    }

    public void CheckShotCount()
    {
        if (shotCount == 1)
        {
            shotCountText.SetTopText("SHOT");
            shotCountText.SetBottomText("1/3");
            shotCountText.Flash();
        }

        if (shotCount == 2)
        {
            shotCountText.SetTopText("SHOT");
            shotCountText.SetBottomText("2/3");
            shotCountText.Flash();
        }

        if (shotCount == 3)
        {
            shotCountText.SetTopText("FINAL");
            shotCountText.SetBottomText("SHOT");
            shotCountText.Flash();
        }
    }
}
