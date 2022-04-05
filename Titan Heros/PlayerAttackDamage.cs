using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackDamage : MonoBehaviour
{
    public LayerMask layerMask;

    public float radius = 1f;
    public float damage = 1f;
    public float delayTime = 1f;

    public bool deal_Multiple_Damage;
    public bool disable_Script;
    public bool detectCollisionAfterDelay;
    private bool canDetectCollision = true;

    void Start()
    {
        if (detectCollisionAfterDelay)
        {
            canDetectCollision = false;
            StartCoroutine(CollisionAfterDelay());
        }
    }

    void Update()
    {
        if (canDetectCollision) DetectCollision();
    }

    private void DetectCollision()
    {
        Collider[] hit = Physics.OverlapSphere(transform.position, radius, layerMask);

        if (hit.Length > 0)
        {
            if (deal_Multiple_Damage) // 범위공격
            {
                for (int i = 0; i < hit.Length; i++)
                {
                    hit[i].GetComponent<HealthScript>().ApplyDamage(damage);
                }
            }
            else // 단일대상공격
            {
                hit[0].GetComponent<HealthScript>().ApplyDamage(damage);
            }

            if (disable_Script) this.enabled = false; // 스크립트만 비활성화
            else gameObject.SetActive(false);
        }
    }

    private IEnumerator CollisionAfterDelay()
    {
        yield return new WaitForSeconds(delayTime);
        canDetectCollision = true;
    }
}
