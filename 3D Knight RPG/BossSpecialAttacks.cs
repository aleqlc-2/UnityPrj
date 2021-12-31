using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpecialAttacks : MonoBehaviour
{
    public GameObject bossFire;
    public GameObject bossMagic;

    private Transform playerTarget;

    void Start()
    {
        playerTarget = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // 애니메이션 이벤트에 등록된 메서드들
    private void BossFireTornado()
    {
        Instantiate(bossFire, playerTarget.position, Quaternion.Euler(0f, Random.Range(0f, 360f), 0f));
    }

    private void BossSpell()
    {
        Vector3 temp = playerTarget.position;
        temp.y += 1.5f;
        Instantiate(bossMagic, temp, Quaternion.identity);
    }
}
