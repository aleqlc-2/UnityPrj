using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager singleton;
    public bool GameStarted { get; private set; }
    public bool GameEnded { get; private set; }

    [SerializeField] private float slowMotionFactor = .1f; // 게임오버시 슬로우모션
    [SerializeField] private Transform startTransform;
    [SerializeField] private Transform goalTransform;
    [SerializeField] private BallController ball;

    public float EntireDistance { get; private set; } // 도착선 - 시작선
    public float DistanceLeft { get; private set; } // 도착선 - ball의 위치

    private void Awake()
    {
        StartGame();
        if (singleton == null)
        {
            singleton = this;
        }
        else if (singleton != this)
        {
            Destroy(gameObject);
        }

        // default time
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f; // Interval
    }

    private void Start()
    {
        EntireDistance = goalTransform.position.z - startTransform.position.z; // float
    }

    public void StartGame()
    {
        GameStarted = true;
    }

    public void EndGame(bool win)
    {
        GameEnded = true;

        if (!win) // 초록색 공을 건드렸다면
        {
            // Restart the game
            Invoke("RestartGame", 2 * slowMotionFactor); // 재시작은 2초뒤에, 2초간 슬로우모션
            Time.timeScale = slowMotionFactor;
            Time.fixedDeltaTime = Time.fixedDeltaTime * Time.timeScale; // 이 코드 없으면 슬로우모션이 뚝뚝 끊김
        }
        else
        {
            Invoke("RestartGame", 2f); // 게임을 클리어한 경우에도 재시작
        }
    }

    public void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0); // sceneBuildIndex가 0인 scene을 Load
    }

    void Update()
    {
        // float 반환
        DistanceLeft = Vector3.Distance(ball.transform.position, goalTransform.position);

        if (DistanceLeft > EntireDistance)
            DistanceLeft = EntireDistance;

        if (ball.transform.position.z > goalTransform.transform.position.z)
            DistanceLeft = 0;
    }
}
