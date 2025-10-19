using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;
using GameDevTV.RTS.Utilities;

namespace GameDevTV.RTS.Behavior
{
	[Serializable, GeneratePropertyBag]
	[NodeDescription(name: "Move to Target GameObject", story: "[Agent] moves to [TargetGameObject] .", category: "Action/Navigation", id: "3a3a3cce8f201223f700a0a64ae29b4e")]
	public partial class MoveToTargetGameObjectAction : Action
	{
		[SerializeReference] public BlackboardVariable<GameObject> Agent;
		[SerializeReference] public BlackboardVariable<GameObject> TargetGameObject;

		private NavMeshAgent agent;
		private Animator animator;

		protected override Status OnStart()
		{
			if (!Agent.Value.TryGetComponent(out agent) || TargetGameObject.Value == null)
			{
				return Status.Failure;
			}

			Agent.Value.TryGetComponent(out animator);

			Vector3 targetPosition = GetTargetPosition();

			if (Vector3.Distance(agent.transform.position, targetPosition) <= agent.stoppingDistance)
			{
				return Status.Success;
			}

			agent.SetDestination(targetPosition);
			return Status.Running;
		}

		protected override Status OnUpdate()
		{
			if (animator != null)
			{
				animator.SetFloat(AnimationConstants.SPEED, agent.velocity.magnitude);
			}

			if (agent.remainingDistance <= agent.stoppingDistance)
			{
				return Status.Success;
			}
			
			return Status.Running;
		}

		protected override void OnEnd()
		{
			if (animator != null)
			{
				animator.SetFloat(AnimationConstants.SPEED, 0);
			}
		}

		private Vector3 GetTargetPosition()
		{
			Vector3 targetPosition;
			if (TargetGameObject.Value.TryGetComponent(out Collider collider))
			{
				targetPosition = collider.ClosestPoint(agent.transform.position);
			}
			else
			{
				targetPosition = TargetGameObject.Value.transform.position;
			}

			return targetPosition;
		}
	}
}
