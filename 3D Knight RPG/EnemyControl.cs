using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState
{
    IDLE,
    WALK,
    RUN,
    PAUSE,
    GOBACK,
    ATTACK,
    DEATH
}

public class EnemyControl : MonoBehaviour
{
    [HideInInspector] public EnemyState enemy_CurrentState = EnemyState.IDLE;
    private EnemyState enemy_LastState = EnemyState.IDLE;

    private Vector3 initialPosition;
    private Vector3 whereTo_Move = Vector3.zero;
    private Vector3 whereTo_Navigate;

    private float attack_Distance = 1.5f;
    private float alert_Attack_Distance = 8f;
    private float followDistance = 15f;
    private float enemyToPlayerDistance;
    private float move_Speed = 2f;
    private float walk_Speed = 1f;
    private float currentAttackTime;
    private float waitAttackTime = 1f;

    private bool finished_Animation = true;
    private bool finished_Movement = true;

    private Transform playerTarget;

    private CharacterController charController;

    private Animator anim;

    private NavMeshAgent navAgent;

    private EnemyHealth enemyHealth;

    void Awake()
    {
        playerTarget = GameObject.FindGameObjectWithTag("Player").transform;
        navAgent = GetComponent<NavMeshAgent>();
        charController = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        enemyHealth = GetComponent<EnemyHealth>();

        initialPosition = transform.position;
        whereTo_Navigate = transform.position;
    }

    void Update()
    {
        if (enemyHealth.health <= 0) enemy_CurrentState = EnemyState.DEATH;

        if (enemy_CurrentState != EnemyState.DEATH)
        {
            enemy_CurrentState = SetEnemyState(enemy_CurrentState, enemy_LastState, enemyToPlayerDistance);

            if (finished_Movement)
            {
                GetStateControl(enemy_CurrentState);
            }
            else
            {
                if (!anim.IsInTransition(0) && anim.GetCurrentAnimatorStateInfo(0).IsName("Idle")) // IsName
                    finished_Movement = true;
                else if ((anim.GetCurrentAnimatorStateInfo(0).IsTag("Atk1") || anim.GetCurrentAnimatorStateInfo(0).IsTag("Atk2")) // IsTag
                          && !anim.IsInTransition(0))
                {
                    anim.SetInteger("Atk", 0);
                }
            }
        }
        else
        {
            anim.SetBool("Death", true);
            charController.enabled = false;
            navAgent.enabled = false;

            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Death") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f
                && !anim.IsInTransition(0))
            {
                Destroy(gameObject, 2f);
            }
        }
    }

    private EnemyState SetEnemyState(EnemyState curState, EnemyState lastState, float enemyToPlayerDis)
    {
        enemyToPlayerDis = Vector3.Distance(transform.position, playerTarget.position);

        float initialDistance = Vector3.Distance(initialPosition, transform.position);

        // 적이 플레이어를 쫓아오다가 15f 거리이상이 되면 원래 자기자리로 돌아가도록
        if (initialDistance > followDistance)
        {
            lastState = curState;
            curState = EnemyState.GOBACK;
        }
        else if (enemyToPlayerDis <= attack_Distance)
        {
            lastState = curState;
            curState = EnemyState.ATTACK; // 공격
        }
        else if (enemyToPlayerDis >= alert_Attack_Distance &&
                (lastState == EnemyState.PAUSE || lastState == EnemyState.ATTACK))
        {
            lastState = curState;
            curState = EnemyState.PAUSE; // 공격중지
        }
        // 플레이어를 알아차렸지만 공격범위 밖인 상태
        else if (enemyToPlayerDis <= alert_Attack_Distance && enemyToPlayerDis > attack_Distance)
        {
            if (curState != EnemyState.GOBACK || lastState == EnemyState.WALK)
            {
                lastState = curState;
                curState = EnemyState.PAUSE;
            }
        }
        else if (enemyToPlayerDis > alert_Attack_Distance && lastState != EnemyState.GOBACK && lastState != EnemyState.PAUSE)
        {
            lastState = curState;
            curState = EnemyState.WALK; // 플레이어를 알아차리지 않은 상태에서 배회
        }

        return curState;
    }

    private void GetStateControl(EnemyState curState)
    {
        if (curState == EnemyState.RUN || curState == EnemyState.PAUSE)
        {
            if (curState != EnemyState.ATTACK)
            {
                Vector3 targetPosition = new Vector3(playerTarget.position.x, transform.position.y, playerTarget.position.z);

                if (Vector3.Distance(transform.position, targetPosition) >= 2.1f) // 가까이 붙으면 달려가는 애니메이션때문에 버벅이지 않도록
                {
                    anim.SetBool("Walk", false);
                    anim.SetBool("Run", true);

                    navAgent.SetDestination(targetPosition);
                }
            }
        }
        else if (curState == EnemyState.ATTACK)
        {
            anim.SetBool("Run", false);
            whereTo_Move.Set(0f, 0f, 0f);
            navAgent.SetDestination(transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation,
                                                  Quaternion.LookRotation(playerTarget.position - transform.position),
                                                  Time.deltaTime * 5f);

            if (currentAttackTime >= waitAttackTime)
            {
                int atkRange = Random.Range(1, 3);

                anim.SetInteger("Atk", atkRange); // 공격애니메이션 2개, 파라미터 각각 1과 2
                finished_Animation = false;
                currentAttackTime = 0f;
            }
            else
            {
                anim.SetInteger("Atk", 0); // 공격애니메이션 중지
                currentAttackTime += Time.deltaTime;
            }
        }
        else if (curState == EnemyState.GOBACK)
        {
            anim.SetBool("Run", true);
            Vector3 targetPosition = new Vector3(initialPosition.x, transform.position.y, initialPosition.z);
            navAgent.SetDestination(targetPosition);

            if (Vector3.Distance(targetPosition, initialPosition) <= 3.5f) // 항상 true 아닌가
            {
                enemy_LastState = curState;
                curState = EnemyState.WALK;
            }
        }
        else if (curState == EnemyState.WALK)
        {
            anim.SetBool("Run", false);
            anim.SetBool("Walk", true);

            // initialPosition를 중심으로 랜덤으로 배회하도록 하는 로직
            if (Vector3.Distance(transform.position, whereTo_Navigate) <= 2f)
            {
                whereTo_Navigate.x = Random.Range(initialPosition.x - 5f, initialPosition.x + 5f);
                whereTo_Navigate.z = Random.Range(initialPosition.z - 5f, initialPosition.z + 5f);
            }
            else
            {
                // anim.applyRootMotion 가 true이면 애니메이션이 위치와 회전을 제어하므로
                // navAgent.SetDestination를 해도 그쪽으로 이동을 하지 않고 제자리걸음 하므로 false줘야함
                anim.applyRootMotion = false;
                navAgent.SetDestination(whereTo_Navigate);
            }
        }
        else
        {
            anim.SetBool("Run", false);
            anim.SetBool("Walk", false);
            whereTo_Move.Set(0f, 0f, 0f);
            navAgent.isStopped = true;
        }
    }
}
