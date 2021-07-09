using UnityEngine;
using System.Collections;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class RandomAgent : MonoBehaviour {

	private NavMeshAgent agent;

	void Start ()
	{
		agent = GetComponent<NavMeshAgent>();
	}
	
	void Update ()
	{
		if (!agent.pathPending)
		{
			if (agent.remainingDistance <= agent.stoppingDistance)
			{
				if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
				{
					NewDestination();
				}
			}
		}
	}

	private void NewDestination()
	{
		Vector3 newDest = Random.insideUnitSphere * 500f + new Vector3(139, 86f, -172f);
		NavMeshHit hit;
		bool hasDestination = UnityEngine.AI.NavMesh.SamplePosition(newDest, out hit, 100f, 1);
		if (hasDestination)
		{
			agent.SetDestination(hit.position);
		}
	}
}
