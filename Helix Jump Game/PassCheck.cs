using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassCheck : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        GameManager.singleton.AddScore(2); // 원판 지날때는 2점 추가

        // 오브젝트.perfectPass++; 은 또 뭐냐..
        FindObjectOfType<BallController>().perfectPass++; // 통과할때마다 perfectPass +1
    }
}
