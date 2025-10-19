using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;


namespace GameDevTV.RTS.Behavior
{
	[Serializable, GeneratePropertyBag]
	[NodeDescription(name: "Set NavMeshAgent Enabled", story: "[Self] sets NavMeshAgent active status to [Active] .", category: "Action/Navigation", id: "da5a846d8a809096f938a7de7a198d58")]
	public partial class SetNavMeshAgentEnabledAction : Action
	{
		[SerializeReference] public BlackboardVariable<GameObject> Self;
		[SerializeReference] public BlackboardVariable<bool> Active;

		protected override Status OnStart()
		{
			if (Self.Value == null || !Self.Value.TryGetComponent(out NavMeshAgent agent)) return Status.Failure;

			agent.enabled = Active;

			return Status.Success;
		}
	}
}