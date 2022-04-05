using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState
{
    PATROL,
    CHASE,
    ATTACK
}

public class EnemyController : MonoBehaviour
{
    private CharacterAnimation enemyAnim;

    private NavMeshAgent navAgent;

    private EnemyState enemyState;

    private float patrol_Radius = 30f;
    private float patrol_Timer = 10f;
    private float timer_Count;
    public float move_Speed = 3.5f;
    public float run_Speed = 5f;

    private Transform player_Target;
    public float chase_Distance = 7f;
    public float attack_Distance = 1f;
    public float chase_Player_After_Attack_Distance = 1f;
    private float wait_Before_Attack_Time = 3f;
    private float attack_Timer;

    private bool enemyDied;

    void Awake()
    {
        navAgent = GetComponent<NavMeshAgent>();
        enemyAnim = GetComponent<CharacterAnimation>();
    }

    void Start()
    {
        timer_Count = patrol_Timer;
        enemyState = EnemyState.PATROL;
        
        player_Target = GameObject.FindGameObjectWithTag(TagManager.PLYAER_TAG).transform;
        attack_Timer = wait_Before_Attack_Time;
    }

    void Update()
    {
        if (enemyDied) return;

        if (enemyState == EnemyState.PATROL) Patrol();

        // 공격받던 플레이어가 도망가는 경우
        if (enemyState != EnemyState.CHASE && enemyState != EnemyState.ATTACK)
        {
            if (Vector3.Distance(transform.position, player_Target.position) <= chase_Distance)
            {
                enemyState = EnemyState.CHASE; // 다시 쫓음
                enemyAnim.StopAnimation(); // 적의 공격 애니메이션 중지
            }
        }

        if (enemyState == EnemyState.CHASE) ChasePlayer();
        if (enemyState == EnemyState.ATTACK) AttackPlayer();
    }

    private void Patrol()
    {
        timer_Count += Time.deltaTime;
        navAgent.speed = move_Speed;

        if (timer_Count > patrol_Timer)
        {
            SetNewRandomDestination();
            timer_Count = 0f;
        }

        if (navAgent.remainingDistance <= 0.5f)
        {
            navAgent.velocity = Vector3.zero;
        }

        if (navAgent.velocity.sqrMagnitude == 0) enemyAnim.Walk(false);
        else enemyAnim.Walk(true);
    }

    private void SetNewRandomDestination()
    {
        Vector3 newDestination = RandomNavSphere(transform.position, patrol_Radius, -1);
        navAgent.SetDestination(newDestination);
    }

    // enemy를 중심으로 원을 그려 랜덤 포지션을 잡음
    private Vector3 RandomNavSphere(Vector3 originPos, float dist, int layerMask)
    {
        Vector3 randDir = Random.insideUnitSphere * dist;
        randDir += originPos;

        NavMeshHit navHit;
        NavMesh.SamplePosition(randDir, out navHit, dist, layerMask);

        return navHit.position;
    }

    private void ChasePlayer()
    {
        navAgent.SetDestination(player_Target.position);
        navAgent.speed = run_Speed;

        if (navAgent.velocity.sqrMagnitude == 0) enemyAnim.Run(false);
        else enemyAnim.Run(true);

        if (Vector3.Distance(transform.position, player_Target.position) <= attack_Distance)
        {
            enemyState = EnemyState.ATTACK;
        }
        else if (Vector3.Distance(transform.position, player_Target.position) > chase_Distance)
        {
            timer_Count = patrol_Timer;
            enemyState = EnemyState.PATROL;
            enemyAnim.Run(false);
        }
    }

    private void AttackPlayer()
    {
        navAgent.velocity = Vector3.zero;
        navAgent.isStopped = true;

        enemyAnim.Run(false);
        enemyAnim.Walk(false);

        attack_Timer += Time.deltaTime;

        if (attack_Timer > wait_Before_Attack_Time)
        {
            enemyAnim.NormalAttack_1();
            attack_Timer = 0f;
        }

        if (Vector3.Distance(transform.position, player_Target.position) > attack_Distance + chase_Player_After_Attack_Distance)
        {
            navAgent.isStopped = false;
            enemyState = EnemyState.CHASE;
        }
    }

    private void DeactivateScript()
    {
        GameplayController.instance.EnemyDied();
        enemyDied = true;
        StartCoroutine(DeactivateEnemyGameObject());
    }

    private IEnumerator DeactivateEnemyGameObject()
    {
        yield return new WaitForSeconds(2f);
        gameObject.SetActive(false);
    }
}
