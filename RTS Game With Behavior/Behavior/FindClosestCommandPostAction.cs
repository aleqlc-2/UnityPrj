using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using GameDevTV.RTS.Units;
using System.Collections.Generic;

namespace GameDevTV.RTS.Behavior
{
	[Serializable, GeneratePropertyBag]
	[NodeDescription(name: "Find Closest Command Post", story: "[Unit] finds nearest [CommandPost] .", category: "Action/Units", id: "3ab6a6d737a8bf89da30c65ce2847b10")]
	public partial class FindClosestCommandPostAction : Action
	{
		[SerializeReference] public BlackboardVariable<GameObject> Unit;
		[SerializeReference] public BlackboardVariable<GameObject> CommandPost;
		[SerializeReference] public BlackboardVariable<float> SearchRadius = new(10);
		[SerializeReference] public BlackboardVariable<BuildingSO> CommandPostBuilding;

		protected override Status OnStart()
		{
			Collider[] colliders = Physics.OverlapSphere(Unit.Value.transform.position, SearchRadius, LayerMask.GetMask("Buildings"));

			List<BaseBuilding> nearbyCommandPosts = new();
			foreach (Collider collider in colliders)
			{
				if (collider.TryGetComponent(out BaseBuilding building) && building.UnitSO.Equals(CommandPostBuilding.Value))
				{
					nearbyCommandPosts.Add(building);
				}
			}

			if (nearbyCommandPosts.Count == 0)
			{
				return Status.Failure;
			}

			CommandPost.Value = nearbyCommandPosts[0].gameObject;

			return Status.Success;
		}
	}
}