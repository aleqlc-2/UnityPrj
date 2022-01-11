using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Progress : MonoBehaviour
{
    public RectTransform extraBallInner;

    private GameController gameController;

    private float currentWidth, addWidth, totalWidth;

    void Awake()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
    }

    void Start()
    {
        extraBallInner.sizeDelta = new Vector2(31, 117); // Width, Height 초기값 설정
        currentWidth = 31; // ExtraBallInner 객체의 하이어라키창에 설정된 초기값
        totalWidth = 600; // ExtraBallInner 객체의 Width값 600주면 게이지 꽉참
    }

    void Update()
    {
        // 게이지 꽉차면 다음번 발사때 볼 1개 더 쏠 수 있도록
        if (currentWidth >= totalWidth)
        {
            gameController.ballsCount++; // 볼 1개 늘리고
            gameController.ballsCountText.text = gameController.ballsCount.ToString();
            currentWidth = 31; // 게이지 초기화
        }

        // currentWidth 초기값이 31이므로 게임시작되면 일단 36까지 게이지 차고 시작.
        if (currentWidth >= addWidth)
        {
            addWidth += 5;
            extraBallInner.sizeDelta = new Vector2(addWidth, 117); // 실제로 게이지를 채움
        }
        // 게이지 36까지 차면 else 구문들어왔다가 addWidth가 31이 되서 다시 if문으로 감. 블록깨져서 currentWidth가 커져도 +-5 하며 계속 반복
        else
            addWidth = currentWidth;
    }

    // 블록 1개 깨질때마다 채워져야할 게이지 최대값 늘려서 설정하는 메서드
    // addRandom은 게이지 랜덤으로 차도록. 게이지를 빨리 600까지 채우면 update문에서 다음번 발사때 볼을 1개 더 쏠 수 있도록
    public void IncreaseCurrentWidth()
    {
        // currentWidth % 576f는 currentWidth를 누적하기 위함
        // 문제는 currentWidth가 576일때 이 구문 들어오면 게이지가 오히려 줄어들어버릴텐데
        int addRandom = Random.Range(80, 120);
        currentWidth = currentWidth % 576f + addRandom + 31;
    }
}
