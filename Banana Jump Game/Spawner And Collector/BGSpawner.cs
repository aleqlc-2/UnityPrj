using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGSpawner : MonoBehaviour
{
    //새로운 배경 생성
    private GameObject[] bgs;
    private float height;
    private float highest_Y_Pos;

    void Awake()
    {
        bgs = GameObject.FindGameObjectsWithTag("BG"); // BG라는 태그를 가진 객체 전부 배열로 넣음.
    }

    void Start()
    {
        // 배경 한개의 height를 구함.
        height = bgs[0].GetComponent<BoxCollider2D>().bounds.size.y;
        
        // for문을 이용하여 모든 배경 중 가장 위쪽 배경의 y 포지션을 구함.
        highest_Y_Pos = bgs[0].transform.position.y;
        for (int i = 0; i < bgs.Length; i++)
        {
            if (bgs[i].transform.position.y > highest_Y_Pos)
                highest_Y_Pos = bgs[i].transform.position.y;
        }
    }

    void OnTriggerEnter2D(Collider2D target)
    {
        // BGSpawner가 카메라 하위계층이고 위에 달려있으므로
        // 가장 위쪽배경의 y를 넘어가는순간 새로운 배경 생성
        if (target.tag == "BG")
        {
            if (target.transform.position.y >= highest_Y_Pos)
            {
                Vector3 temp = target.transform.position; // 가장 위쪽 배경의 위치
                for (int i = 0; i < bgs.Length; i++)
                {
                    if (!bgs[i].activeInHierarchy) // Collector가 지운게 있다면
                    {
                        temp.y += height;

                        // 한장씩 생성
                        bgs[i].transform.position = temp;
                        bgs[i].gameObject.SetActive(true);

                        highest_Y_Pos = temp.y;
                    }
                }
            }
        }
    }
}
