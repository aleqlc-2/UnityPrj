using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{
    private Vector3 randomPos;

    private GameObject target;
    private NavMeshAgent agent;

    public AudioSource groanSFX;

    private bool isWalking;
    private bool isRunning;

    private Animator anim;

    private void Start()
    {
        randomPos = transform.position;
        target = GameObject.FindGameObjectWithTag("Player"); // G
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        WalkToRandomSpot();
    }

    private void Update()
    {
        if (MapManager.instance.zombiesCanMove) // 플레이어가 win하면 움직이지 못하게 하기위해
        {
            if (Vector3.Distance(transform.position, target.transform.position) <= 5)
            {
                // 플레이어와의 거리가 5 이하이면 추적
                ChasePlayer();
            }
            else if (isRunning) // 추적중인데 플레이어와의 거리가 5가 초과되면
            {
                // 추적을 멈추고 걸어다님
                WalkToRandomSpot();
            }

            if (Vector3.Distance(transform.position, target.transform.position) <= 1)
            {
                anim.SetTrigger("attack");
            }
        }
    }

    private void ChasePlayer()
    {
        agent.SetDestination(target.transform.position);

        if (!isRunning)
        {
            groanSFX.Play();
            isRunning = true;
            isWalking = false;
            agent.speed = 2;
            anim.SetBool("isRunning", isRunning);
            anim.SetBool("isWalking", isWalking);
        }
    }

    private void WalkToRandomSpot()
    {
        agent.speed = 0.75f;
        randomPos = MapManager.instance.GetRandomPos();

        agent.SetDestination(randomPos);

        isRunning = false;
        isWalking = true;

        anim.SetBool("isRunning", isRunning);
        anim.SetBool("isWalking", isWalking);
    }
}
