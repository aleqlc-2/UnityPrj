using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 각각의 셀 프리펩에 부착된 스크립트
public class MineField : MonoBehaviour
{
    [HideInInspector] public bool isMine;

    private SpriteRenderer sr;

    [SerializeField] private Sprite[] images;
    [SerializeField] private Sprite mineImage;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        // Random.value는 0~1사이값 랜덤반환. 하한과 상한 모두포함
        isMine = Random.value < 0.15f; // 지뢰일 확률 15%

        if (isMine)
        {
            if (GameManager.instance.CanBeMine())
                GameManager.instance.IncrementMinesInGame();
            else
                isMine = false;
        }
    }

    public void ShowMine()
    {
        if (isMine) // 지뢰이면
        {
            sr.sprite = mineImage;
        }
    }

    public void ShowNearMinesCount(int nearMines)
    {
        sr.sprite = images[nearMines];
    }

    public bool IsClicked()
    {
        return sr.sprite.texture.name == "Hidden Element";
    }

    // 이 스크립트가 부착된 오브젝트의 콜라이더를 마우스로 클릭한 경우 호출됨
    void OnMouseDown()
    {
        if (isMine)
        {
            MatrixGrid.ShowAllMines();
        }
        else
        {
            string[] index = gameObject.name.Split('-');
            int x = int.Parse(index[0]);
            int y = int.Parse(index[1]);

            ShowNearMinesCount(MatrixGrid.NearMines(x, y));

            MatrixGrid.InvestigateMines(x, y, new bool[GameManager.instance.rows, GameManager.instance.columns]);

            if (MatrixGrid.IsGameFinished())
            {
                print("You Won!");
            }
        }
    }
}
