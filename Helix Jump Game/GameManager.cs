using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Advertisements; //ad넣을떄

public class GameManager : MonoBehaviour
{
    public int best;
    public int score;
    public int currentStage = 0;

    public static GameManager singleton;

    void Awake()
    {
        // Advertisement.Initialize("아이디"); // ad넣을때
        if (singleton == null)
            singleton = this;
        else if (singleton != this)
            Destroy(gameObject);

        best = PlayerPrefs.GetInt("Highscore"); // 최고점수 불러옴
    }

    public void NextLevel() // 다음 stage 실행
    {
        currentStage++;
        FindObjectOfType<BallController>().ResetBall();
        FindObjectOfType<HelixController>().LoadStage(currentStage);
    }

    public void RestartLevel() // 현재 stage 재실행
    {
        //Advertisement.Show(); // ad넣을떄

        singleton.score = 0;
        FindObjectOfType<BallController>().ResetBall();

        // 이 코드 안쓰면 같은 스테이지 재시작해도 데스파트랑 통로가 똑같음
        FindObjectOfType<HelixController>().LoadStage(currentStage);
    }

    public void AddScore(int scoreToAdd)
    {
        score += scoreToAdd;

        if (score > best)
        {
            best = score;
            PlayerPrefs.SetInt("Highscore", score); // key, value를 이용하여 저장
        }
    }
}
