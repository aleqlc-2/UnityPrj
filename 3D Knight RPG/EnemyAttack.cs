using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public float damageAmount = 10f;
    private float damageDistance = 2f;

    private Transform playerTarget;

    private Animator anim;

    private bool finishedAttack = true;

    private PlayerHealth playerHealth;

    void Awake()
    {
        playerTarget = GameObject.FindGameObjectWithTag("Player").transform;
        anim = GetComponent<Animator>();

        playerHealth = playerTarget.GetComponent<PlayerHealth>();
    }

    void Update()
    {
        if (finishedAttack)
        {
            if (playerTarget) // 플레이어가 살아있을때만 실행되도록
                DealDamage(CheckIfAttacking());
        }
        else
        {
            if (!anim.IsInTransition(0) && anim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                finishedAttack = true;
            }
        }
    }

    private void DealDamage(bool isAttacking)
    {
        if (isAttacking)
        {
            if (Vector3.Distance(transform.position, playerTarget.position) <= damageDistance)
            {
                playerHealth.TakeDamage(damageAmount);
            }
        }
    }

    // 현재 공격중인지 체크
    private bool CheckIfAttacking()
    {
        bool isAttacking = false;

        // 다른 애니메이션으로 전환하지 않았고 && 현재 애니메이션이 공격애니메이션이고
        if (!anim.IsInTransition(0) &&
            (anim.GetCurrentAnimatorStateInfo(0).IsName("Atk1") || anim.GetCurrentAnimatorStateInfo(0).IsName("Atk2")))
        {
            // 공격 애니메이션이 50% 이상 진행되었다면
            if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5f)
            {
                isAttacking = true; // 공격중인것으로 판단
                finishedAttack = false; // 공격이 끝나지 않았음
            }
        }

        return isAttacking;
    }
}
