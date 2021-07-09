using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPointControl : MonoBehaviour
{
    Transform[] points;
    float moveSpeed = 50f; // 1초에 50유닛을 이동하도록 하기위해
    int nextIndex = 1;

    void Start()
    {
        GameObject wayPointGroup = GameObject.Find("WayPointGroup");

        // 부모개체까지 배열에 들어감, 0번째 인덱스가 부모개체임
        points = wayPointGroup.GetComponentsInChildren<Transform>();
        Debug.Log(points.Length); // 부모개체1개 + 자식개체10개 = 총 11개
        foreach(Transform t in points) 
        {
            Debug.Log(t.gameObject.name);
        }
    }

    void Update()
    {
        // 다음 이동지점 - 플레이어의 현재위치
        Vector3 direction = points[nextIndex].position - transform.position;

        // 1초에 50유닛만큼의 속도로 해당 방향으로 이동
        transform.Translate(direction.normalized * Time.deltaTime * moveSpeed);
    }

    // wayPoint에는 콜라이더만 추가, player는 리지드바디와 콜라이더 추가
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "POINT")
        {
            nextIndex++;
            Debug.Log(nextIndex);
            // Point (9)에 닿으면 nextIndex 11이 되서 points.Length - 1 = 10 보다 커져서
            // nextIndex = 1이 되서 Point로 이동
            if (nextIndex > points.Length - 1)
                nextIndex = 1;
        }
    }
}