using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlannedAgent : MonoBehaviour
{
    public static PlannedAgent instance;

    public Vector3[] points; // 인스펙터창에서 할당했음
    private int counter = 0;

    private NavMeshAgent agent;

    [HideInInspector]
    public GameObject followTarget;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (instance == null)
            instance = this;
        else
            Destroy(instance);
    }

    void Update()
    {
        // 목적지에 도착했는지 체크
        if (!agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    NewDestination(); // 새로운 목적지 설정
                }
            }
        }
    }

    private void NewDestination()
    {
        Vector3 newDest;

        if (followTarget != null) // player를 발각하면 추적
            newDest = followTarget.transform.position;
        else
        {
            counter = (counter + 1) % points.Length;
            newDest = points[counter];
        }
            
        NavMeshHit hit;
        bool hasDestination = NavMesh.SamplePosition(newDest, out hit, 100f, 1);
        if (hasDestination)
        {
            agent.SetDestination(hit.position);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "MainCamera")
            followTarget = other.gameObject;
    }
}
