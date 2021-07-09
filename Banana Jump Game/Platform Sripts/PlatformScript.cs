using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 플랫폼 프리펩에 부착된 스크립트
public class PlatformScript : MonoBehaviour
{
    [SerializeField]
    private GameObject one_Banana, bananas;

    [SerializeField]
    private Transform spawn_Point;

    void Start()
    {
        GameObject newBanana = null;

        if (Random.Range(0, 10) > 3)
        {
            newBanana = Instantiate(one_Banana, spawn_Point.position, Quaternion.identity);
        }
        else
        {
            newBanana = Instantiate(bananas, spawn_Point.position, Quaternion.identity);
        }

        newBanana.transform.parent = transform;
    }
}
