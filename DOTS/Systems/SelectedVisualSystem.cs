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
			if (selected.ValueRO.onDeselected) // ������ҵ�����
			{
				RefRW<LocalTransform> visualLocalTransform = SystemAPI.GetComponentRW<LocalTransform>(selected.ValueRO.visualEntity);
				visualLocalTransform.ValueRW.Scale = 0f;
			}

			if (selected.ValueRO.onSelected) // ���õ�����
			{
				RefRW<LocalTransform> visualLocalTransform = SystemAPI.GetComponentRW<LocalTransform>(selected.ValueRO.visualEntity);
				visualLocalTransform.ValueRW.Scale = selected.ValueRO.showScale;
			}
		}
	}
}
