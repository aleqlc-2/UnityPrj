using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallHandler : MonoBehaviour
{
    public static Color oneColor;

    public static float rotationSpeed = 130f;
    public static float rotationTime = 3;

    public static int currentCircleNo;

    public GameObject ball;

    private float speed = 100f;

    private int ballsCount;
    private int circleNo;

    private Color[] ChangingColors;

    public SpriteRenderer spr;

    public Material splashMat;

    void Start()
    {
        ResetGame();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)) HitBall();
    }

    private void ResetGame()
    {
        ChangingColors = ColorScript.colorArray;
        oneColor = ChangingColors[0]; // 볼 색깔 변경
        spr.color = oneColor; // 물감의 색 변경
        splashMat.color = oneColor; // ?

        // as GameObject 안붙이면 에러뜸, (GameObject)Instantiate 이런식으로 명시적캐스트 해줘도됨
        GameObject gameObject2 = Instantiate(Resources.Load("round06" + Random.Range(1, 6))) as GameObject;
        gameObject2.transform.position = new Vector3(0, 20, 23);
        gameObject2.name = "Circle" + circleNo;

        ballsCount = LevelsHandlerScript.ballsCount;

        currentCircleNo = circleNo;

        LevelsHandlerScript.currentColor = oneColor;
        MakeHurdles();
    }

    public void HitBall()
    {
        if (ballsCount <= 1) base.Invoke("MakeNewCircle", 0.4f);

        ballsCount--;

        GameObject gameObject = Instantiate<GameObject>(ball, new Vector3(0, 0, -8), Quaternion.identity);
        gameObject.GetComponent<MeshRenderer>().material.color = oneColor;

        // AddForce는 딱 한번 힘을 주는것이므로 날아가다 떨어짐
        // 누르고있으면 날아가게하려면 Time.deltaTime 곱해야함
        gameObject.GetComponent<Rigidbody>().AddForce(Vector3.forward * speed, ForceMode.Impulse);
    }

    private void MakeNewCircle()
    {
        GameObject[] array = GameObject.FindGameObjectsWithTag("circle"); // 원판 전체
        GameObject gameObject = GameObject.Find("Circle" + this.circleNo);

        // 24번째 자식개체인 tube만 빼고 모든 자식개체조각을 비활성화
        for (int i = 0; i < 24; i++)
        {
            gameObject.transform.GetChild(i).gameObject.SetActive(false);
        }

        // 원판 전체의 색깔을 BallHandler.oneColor로 변경
        gameObject.transform.GetChild(24).gameObject.GetComponent<MeshRenderer>().material.color = BallHandler.oneColor;

        if (gameObject.GetComponent<iTween>()) gameObject.GetComponent<iTween>().enabled = false; // ?

        // 지금까지 생성된 모든 원판을
        foreach (GameObject target in array)
        {
            iTween.MoveBy(target, iTween.Hash(new object[]
            {
                "y", // y축을 기준으로
                -2.98f, // -2.98만큼 내려보냄
                "easetype",
                iTween.EaseType.spring, // 약간 튕기듯이 내려감
                "time",
                0.5
            }));
        }

        this.circleNo++;
        currentCircleNo = circleNo;

        GameObject gameObject2 = Instantiate(Resources.Load("round06" + Random.Range(1, 6))) as GameObject;
        gameObject2.transform.position = new Vector3(0, 20, 23);
        gameObject2.name = "Circle" + circleNo;

        ballsCount = LevelsHandlerScript.ballsCount;

        oneColor = ChangingColors[circleNo]; // 볼 색깔 변경
        spr.color = oneColor; // 물감의 색 변경
        splashMat.color = oneColor; // ?

        LevelsHandlerScript.currentColor = oneColor;
        MakeHurdles();
    }

    private void MakeHurdles()
    {
        if (circleNo == 1) FindObjectOfType<LevelsHandlerScript>().MakeHurdles1();
        if (circleNo == 2) FindObjectOfType<LevelsHandlerScript>().MakeHurdles2();
        if (circleNo == 3) FindObjectOfType<LevelsHandlerScript>().MakeHurdles3();
        if (circleNo == 4) FindObjectOfType<LevelsHandlerScript>().MakeHurdles4();
        if (circleNo == 5) FindObjectOfType<LevelsHandlerScript>().MakeHurdles5();
    }
}
