using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class WalkInsideCircle : MonoBehaviour
{
    public float circleRadius = 4f;

    NavMeshAgent agent;

    IEnumerator walk;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        walk = WalkCycle();
        StartCoroutine(walk);
    }

    IEnumerator WalkCycle()
    {
        while(true)
        {
            Vector3 dest = GetDestination();
            agent.destination = dest;

            yield return new WaitForSeconds(3f);
        }
    }

    Vector3 GetDestination()
    {
        Vector2 randomXZ = Random.insideUnitCircle * circleRadius;

        Vector3 destination = new Vector3();
        destination.x = randomXZ.x;
        destination.y = transform.position.y; // y축 이동은 하지 않도록
        destination.z = randomXZ.y;

        return destination;
    }

    void OnDisable()
    {
        StopCoroutine(walk);
    }
}
