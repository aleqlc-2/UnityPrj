using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField]
    private GameObject startPlatform, endPlatform, platformPrefab;
    [SerializeField]
    private int amountToSpawn = 100;
    [SerializeField]
    private GameObject playerPrefab;

    private float blockWidth = 0.5f, blockHeight = 0.2f;
    private int beginAmount = 0;
    private Vector3 lastPos;
    private List<GameObject> spawnedPlatforms = new List<GameObject>();

    void Awake()
    {
        InstantiateLevel();
    }

    void InstantiateLevel()
    {
        for (int i = beginAmount; i < amountToSpawn; i++)
        {
            GameObject newPlatform;

            if (i == 0)
            {
                newPlatform = Instantiate(startPlatform);
            }
            else if (i == amountToSpawn - 1)
            {
                newPlatform = Instantiate(endPlatform);
                newPlatform.tag = "EndPlatform";
            }
            else
            {
                newPlatform = Instantiate(platformPrefab);
            }

            newPlatform.transform.parent = transform;

            spawnedPlatforms.Add(newPlatform);

            if (i == 0)
            {
                lastPos = newPlatform.transform.position;

                //instantiate the player
                Vector3 temp = lastPos;
                temp.y += 0.1f;
                Instantiate(playerPrefab, temp, Quaternion.identity);

                continue; // 2번째 블록부터 아래 코드 적용
            }

            int left = UnityEngine.Random.Range(0, 2);

            if (left == 0)
            {
                newPlatform.transform.position = 
                    new Vector3(lastPos.x - blockWidth, lastPos.y + blockHeight, lastPos.z);
            }
            else
            {
                newPlatform.transform.position =
                    new Vector3(lastPos.x, lastPos.y + blockHeight, lastPos.z + blockWidth);
            }

            lastPos = newPlatform.transform.position;

            // 게임 시작시 상자들 정렬되는 fancy animation
            if (i < 25)
            {
                float endPos = newPlatform.transform.position.y; // 원래 y위치 저장

                // 상자를 blockHeight * 3f 만큼 내려놨다가
                newPlatform.transform.position =
                    new Vector3(newPlatform.transform.position.x,
                                newPlatform.transform.position.y - blockHeight * 3f,
                                newPlatform.transform.position.z);

                // 딜레이 주면서 원위치로
                newPlatform.transform.DOLocalMoveY(endPos, 0.3f).SetDelay(i * 0.1f); // using
            }
        }
    }
}
