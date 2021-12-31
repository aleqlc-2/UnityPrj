using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyControlAnotherWay : MonoBehaviour
{
    public Transform[] walkPoints;
    private Transform playerTarget;

    private int walk_Index = 0;

    private Animator anim;

    private NavMeshAgent navAgent;

    private float walk_Distance = 8f;
    private float attack_Distance = 2f;
    private float currentAttackTime;
    private float waitAttackTime = 1f;

    private Vector3 nextDestination;

    private EnemyHealth enemyHealth;

    void Awake()
    {
        playerTarget = GameObject.FindGameObjectWithTag("Player").transform;
        anim = GetComponent<Animator>();
        navAgent = GetComponent<NavMeshAgent>();
        enemyHealth = GetComponent<EnemyHealth>();
    }

    void Update()
    {
        if (enemyHealth.health > 0) // 적이 살아있을때만 실행되도록
            MoveAndAttack();
        else
        {
            anim.SetBool("Death", true);
            navAgent.enabled = false;

            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Death") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f
                && !anim.IsInTransition(0))
            {
                Destroy(gameObject, 2f);
            }
        }
    }

    private void MoveAndAttack()
    {
        float distance = Vector3.Distance(transform.position, playerTarget.position);

        if (distance > walk_Distance) // 플레이어가 추적 범위 밖이면
        {
            // 일단 게임시작시 목적지가 없으므로 true로 한번 실행되고 나서
            // 첫번째 목적지에 0.5f 거리 이내로 들어가면 다시 true
            if (navAgent.remainingDistance <= 0.5f)
            {
                navAgent.isStopped = false;
                anim.SetBool("Walk", true);
                anim.SetBool("Run", false);
                anim.SetInteger("Atk", 0);

                // 새로운 목적지 설정
                nextDestination = walkPoints[walk_Index].position;
                navAgent.SetDestination(nextDestination);

                if (walk_Index == walkPoints.Length - 1)
                    walk_Index = 0;
                else
                    walk_Index++;
            }
        }
        else // 플레이어가 추적 범위 내로 들어오면
        {
            if (distance > attack_Distance) // 아직 공격 범위가 아니라면
            {
                navAgent.isStopped = false;

                anim.SetBool("Walk", false);
                anim.SetBool("Run", true);
                anim.SetInteger("Atk", 0);

                navAgent.SetDestination(playerTarget.position);
            }
            else // 공격범위 내로 들어오면
            {
                navAgent.isStopped = true;

                anim.SetBool("Run", false);

                Vector3 targetPosition = new Vector3(playerTarget.position.x, transform.position.y, playerTarget.position.z);

                transform.rotation = Quaternion.Slerp(transform.rotation,
                                                      Quaternion.LookRotation(targetPosition - transform.position),
                                                      Time.deltaTime * 5f);

                if (currentAttackTime >= waitAttackTime)
                {
                    int atkRange = Random.Range(1, 3);
                    anim.SetInteger("Atk", atkRange);
                    currentAttackTime = 0f;
                }
                else
                {
                    anim.SetInteger("Atk", 0);
                    currentAttackTime += Time.deltaTime;
                }
            }
        }
    }
}
