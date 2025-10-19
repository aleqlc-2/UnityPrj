using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
partial struct ResetTargetSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // Debug.Log("ResetTargetSystem " + Time.frameCount);

        foreach (RefRW<Target> target in SystemAPI.Query<RefRW<Target>>())
        {
            //Debug.Log(target.ValueRO.targetEntity + " " +
            //          SystemAPI.Exists(target.ValueRO.targetEntity) + " " +
            //          SystemAPI.HasComponent<LocalTransform>(target.ValueRO.targetEntity));

            if (target.ValueRW.targetEntity != Entity.Null)
            {
				if (!SystemAPI.Exists(target.ValueRO.targetEntity) || !SystemAPI.HasComponent<LocalTransform>(target.ValueRO.targetEntity))
				{
					target.ValueRW.targetEntity = Entity.Null;
				}
			}
        }
    }
}
