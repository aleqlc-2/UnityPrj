using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
partial struct GoInGameClientSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        EntityQueryBuilder entityQueryBuilder = new EntityQueryBuilder(Allocator.Temp).WithAll<NetworkId>().WithNone<NetworkStreamInGame>();
        state.RequireForUpdate(state.GetEntityQuery(entityQueryBuilder));
        entityQueryBuilder.Dispose();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);
        foreach ((RefRO<NetworkId> networkId, Entity entity)
             in SystemAPI.Query<RefRO<NetworkId>>().WithNone<NetworkStreamInGame>().WithEntityAccess())
        {
            entityCommandBuffer.AddComponent<NetworkStreamInGame>(entity);
            UnityEngine.Debug.Log("Connected! " + entity + " :: " + networkId.ValueRO.Value);

            Entity rpcEntity = entityCommandBuffer.CreateEntity();
            entityCommandBuffer.AddComponent<GoInGameRequestRpc>(rpcEntity);
            entityCommandBuffer.AddComponent<SendRpcCommandRequest>(rpcEntity);

            Entity onConnectedEventEntity = entityCommandBuffer.CreateEntity();
            entityCommandBuffer.AddComponent(onConnectedEventEntity, new OnConnectedEvent
            {
                connectionId = networkId.ValueRO.Value,
            });
        }

        entityCommandBuffer.Playback(state.EntityManager);
    }
}
