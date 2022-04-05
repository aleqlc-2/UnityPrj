using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackDamage : MonoBehaviour
{
    public LayerMask layerMask;

    public float radius = 1f;
    public float damage = 1f;

    void Update()
    {
        // 적의 공격 감지, layerMask는 Player할당
        Collider[] hit = Physics.OverlapSphere(transform.position, radius, layerMask);

        if (hit.Length > 0)
        {
            hit[0].GetComponent<HealthScript>().ApplyDamage(damage);

            gameObject.SetActive(false);
        }
    }
}
