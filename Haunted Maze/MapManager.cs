using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public Texture2D[] maps;
    private Texture2D selectedMap;

    public GameObject wallPrefab;
    public GameObject gemPrefab;
    public GameObject zombiePrefab;

    private List<Vector3> openPositions = new List<Vector3>();

    private Color wallColor = Color.black;

    public static MapManager instance;

    private int gemsRemaining;

    public bool zombiesCanMove = true;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        GenerateNewMap();
        GenerateGems();
        GenerateZombies();
    }

    public void GenerateNewMap()
    {
        openPositions.Clear();

        // 랜덤으로 map을 선택
        selectedMap = maps[Random.Range(0, maps.Length)];

        // 선택된 map의 모든 픽셀을 인자로 던져 GenerateTile 메서드 실행
        for (int x = 0; x < selectedMap.width; x++)
        {
            for (int y = 0; y < selectedMap.height; y++)
            {
                GenerateTile(x, y);
            }
        }
    }

    private void GenerateTile(int x, int y)
    {
        Color pixelColor = selectedMap.GetPixel(x, y);

        if (pixelColor.a == 0) // 투명도가 0이면(black도트가 아니면)
        {
            // 벽을 생성하지 않고
            // 벽을 생성하지 않은 위치를 저장해 두었다가 zombie와 gem 생성
            openPositions.Add(new Vector3(x, 0, y));
            return;
        }

        // black도트만 벽을 그림
        // 생성된 벽은 이 스크립트가 부착된 오브젝트의 자식개체로.
        if (pixelColor == wallColor)
            Instantiate(wallPrefab, new Vector3(x, 0, y), Quaternion.identity, transform);
    }

    private void GenerateZombies()
    {
        for (int i = 0; i < 7; i++)
        {
            int index = Random.Range(0, openPositions.Count);
            Instantiate(zombiePrefab, openPositions[index], Quaternion.identity);
            openPositions.RemoveAt(index); // 같은 위치에 겹쳐서 스폰되지 않도록 위치를 삭제
        }
    }

    private void GenerateGems()
    {
        for (int i = 0; i < 5; i++)
        {
            int index = Random.Range(0, openPositions.Count);
            Instantiate(gemPrefab, openPositions[index], Quaternion.identity);
            openPositions.RemoveAt(index); // 같은 위치에 겹쳐서 스폰되지 않도록 위치를 삭제
        }

        gemsRemaining = 5;
    }

    public Vector3 GetRandomPos()
    {
        return openPositions[Random.Range(0, openPositions.Count)];
    }

    public void GemPickedUp()
    {
        gemsRemaining--;

        if (gemsRemaining == 0)
        {
            zombiesCanMove = false;
            UIManager.instance.ShowGameOver(true); // win이므로 true던지며 호출
        }
    }
}
