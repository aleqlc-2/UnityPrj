using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

namespace GameDevTV.RTS.Behavior
{
	[Serializable, GeneratePropertyBag]
	[NodeDescription(name: "Set Agent Avoidance", story: "Set [Agent] avoidance quality to [AvoidanceQuality] .", category: "Action/Navigation", id: "91dd28e9b83f2a4a190e126fe5a66ef4")]
	public partial class SetAgentAvoidanceAction : Action
	{
		[SerializeReference] public BlackboardVariable<GameObject> Agent;
		[SerializeReference] public BlackboardVariable<int> AvoidanceQuality;

		protected override Status OnStart()
		{
			if (!Agent.Value.TryGetComponent(out NavMeshAgent agent) || AvoidanceQuality > 4 || AvoidanceQuality < 0)
			{
				return Status.Failure;
			}

			agent.obstacleAvoidanceType = (ObstacleAvoidanceType)AvoidanceQuality.Value;

			return Status.Success;
		}
	}
}
