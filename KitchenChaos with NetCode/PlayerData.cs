using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public struct PlayerData : IEquatable<PlayerData>, INetworkSerializable // struct
{	
    public ulong clientId;
	public int colorId;

	// NetworkList<PlayerData>에 들어가는 PlayerData는 non-nullable형식이어야함(string은 nullable이라 안됨)
	public FixedString64Bytes playerName;
	public FixedString64Bytes playerId;

	public bool Equals(PlayerData other)
	{
		return clientId == other.clientId && colorId == other.colorId && playerName == other.playerName && playerId == other.playerId;
	}

	public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
	{
		serializer.SerializeValue(ref clientId);
		serializer.SerializeValue(ref colorId);
		serializer.SerializeValue(ref playerName);
		serializer.SerializeValue(ref playerId);
	}
}
