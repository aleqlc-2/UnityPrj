using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

[UpdateInGroup(typeof(LateSimulationSystemGroup))]
[UpdateBefore(typeof(ResetEventsSystem))]
partial struct SelectedVisualSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
		foreach (RefRO<Selected> selected in SystemAPI.Query<RefRO<Selected>>().WithPresent<Selected>())
		{
			if (selected.ValueRO.onDeselected) // 선택취소됐을때
			{
				RefRW<LocalTransform> visualLocalTransform = SystemAPI.GetComponentRW<LocalTransform>(selected.ValueRO.visualEntity);
				visualLocalTransform.ValueRW.Scale = 0f;
			}

			if (selected.ValueRO.onSelected) // 선택됐을때
			{
				RefRW<LocalTransform> visualLocalTransform = SystemAPI.GetComponentRW<LocalTransform>(selected.ValueRO.visualEntity);
				visualLocalTransform.ValueRW.Scale = selected.ValueRO.showScale;
			}
		}
	}
}
