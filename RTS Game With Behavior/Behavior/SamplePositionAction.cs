using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

namespace GameDevTV.RTS.Behavior
{
	[Serializable, GeneratePropertyBag]
	[NodeDescription(name: "Sample Position", story: "Set [TargetLocation] to the closest point on the NavMesh to [Target] .", category: "Action/Navigation", id: "13b98225169762defbf2b06ee09519d2")]
	public partial class SamplePositionAction : Action
	{
		[SerializeReference] public BlackboardVariable<Vector3> TargetLocation;
		[SerializeReference] public BlackboardVariable<GameObject> Target;
		[SerializeReference] public BlackboardVariable<float> Radius = new(5);

		protected override Status OnStart()
		{
			if (Target.Value == null || !Target.Value.TryGetComponent(out NavMeshAgent agent)) return Status.Failure;

			NavMeshQueryFilter queryFilter = new();
			queryFilter.agentTypeID = agent.agentTypeID;
			queryFilter.areaMask = agent.areaMask;

			if (NavMesh.SamplePosition(Target.Value.transform.position, out NavMeshHit hit, Radius, NavMesh.AllAreas))
			{
				TargetLocation.Value = hit.position;
				return Status.Success;
			}

			return Status.Failure;
		}
	}
}