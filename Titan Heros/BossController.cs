using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum BossState
{
    GO_TO_PLAYER,
    MOVE_AWAY_FROM_PLAYER,
    SEARCH_FOR_PLAYER,
    ATTACK
}

public class BossController : MonoBehaviour
{
    private CharacterAnimation bossAnim;

    private NavMeshAgent navAgent;

    private BossState boss_State;

    public float move_Speed = 3.5f;

    private Transform player_Target;
    public float attack_Distance = 1f;
    public float chase_Player_After_Attack_Distance = 1f;

    private float wait_Before_Attack_Time = 2f;
    private float attack_Timer;
    private bool first_Attack;

    public float retreat_Distance_Radius = 8f;
    private Vector3 randomPos;

    void Awake()
    {
        bossAnim = GetComponent<CharacterAnimation>();
        navAgent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        player_Target = GameObject.FindGameObjectWithTag(TagManager.PLYAER_TAG).transform;
        boss_State = BossState.SEARCH_FOR_PLAYER;
    }

    void Update()
    {
        if (boss_State == BossState.SEARCH_FOR_PLAYER)
        {
            if (Vector3.Distance(transform.position, player_Target.position) > attack_Distance)
                boss_State = BossState.GO_TO_PLAYER;
            else
                boss_State = BossState.ATTACK;
        }

        if (boss_State == BossState.GO_TO_PLAYER) GoTowardsPlayer();
        if (boss_State == BossState.ATTACK) AttackPlayer();
        if (boss_State == BossState.MOVE_AWAY_FROM_PLAYER) MoveAwayFromPlayer();
    }

    private void GoTowardsPlayer()
    {
        navAgent.isStopped = false;
        navAgent.SetDestination(player_Target.position);

        if (navAgent.velocity.sqrMagnitude == 0) bossAnim.Walk(false);
        else bossAnim.Walk(true);

        if (Vector3.Distance(transform.position, player_Target.position) <= attack_Distance)
        {
            navAgent.velocity = Vector3.zero;
            navAgent.isStopped = true;

            bossAnim.Walk(false);
            boss_State = BossState.ATTACK;

            attack_Timer = wait_Before_Attack_Time / 2f; // 도착후 첫공격은 1초만에 하고 그다음 공격부터 2초간격으로 하도록
        }
    }

    private void AttackPlayer()
    {
        attack_Timer += Time.deltaTime;

        if (attack_Timer > wait_Before_Attack_Time)
        {
            attack_Timer = 0f;

            if (!first_Attack)
            {
                bossAnim.NormalAttack_1();
                first_Attack = true;
            }
            else
            {
                if (Random.Range(0, 5) >= 1)
                {
                    if (Random.Range(0, 3) > 1) bossAnim.NormalAttack_1();
                    else bossAnim.SpecialAttack_1();
                }
                else
                {
                    randomPos = transform.position - (transform.forward * retreat_Distance_Radius);
                    boss_State = BossState.MOVE_AWAY_FROM_PLAYER;
                    first_Attack = false;
                }
            }
        }

        if (Vector3.Distance(transform.position, player_Target.position) > attack_Distance + chase_Player_After_Attack_Distance)
        {
            navAgent.isStopped = false;
            boss_State = BossState.GO_TO_PLAYER;
        }
    }

    private void MoveAwayFromPlayer()
    {
        navAgent.SetDestination(randomPos);
        navAgent.isStopped = false;

        if (navAgent.velocity.sqrMagnitude == 0) bossAnim.Walk(false);
        else bossAnim.Walk(true);

        if (navAgent.remainingDistance <= 0.2f)
        {
            navAgent.velocity = Vector3.zero;
            navAgent.isStopped = true;

            StartCoroutine(WaitThenSearchForPlayer());
        }
    }

    private IEnumerator WaitThenSearchForPlayer()
    {
        yield return new WaitForSeconds(Random.Range(1f, 2f));
        boss_State = BossState.SEARCH_FOR_PLAYER;
    }

    // 보스 죽을때 애니메이션의 이벤트에 등록되어있는 메서드
    private void DeactivateScript()
    {
        EndGameManager.instance.GameOver(true);

        navAgent.isStopped = true;
        enabled = false;
    }
}
