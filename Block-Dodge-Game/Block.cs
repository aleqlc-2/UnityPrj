using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    void Start()
    {
        // 단계가 거듭될수록 블록이 점점 빨라짐
        GetComponent<Rigidbody2D>().gravityScale += Time.timeSinceLevelLoad / 20f;
    }

    void Update()
    {
        if (transform.position.y < -2f)
        {
            Destroy(gameObject);
        }
    }
}
