using Unity.Burst;
using Unity.Entities;
using Unity.Collections;

[UpdateInGroup(typeof(LateSimulationSystemGroup))]
partial struct HealthDeadTestSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer entityCommandBuffer =
            SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
        
        foreach ((RefRO<Health> health, Entity entity)
             in SystemAPI.Query<RefRO<Health>>().WithEntityAccess())
        {
            if (health.ValueRO.healthAmount <= 0)
            {
				// EntityCommandBuffer로 파괴후 ResetTargetSystem에서 Entity.Null을 할당해줘야 에러발생안함
				entityCommandBuffer.DestroyEntity(entity);
            }
        }
    }
}
