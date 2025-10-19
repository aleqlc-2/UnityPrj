using Fusion;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectPoolingManager : MonoBehaviour, INetworkObjectProvider
{
	private Dictionary<INetworkPrefabSource, List<NetworkObject>> prefabsThatHadBeenInstantiated = new();

	private void Start()
	{
		if (GlobalManagers.Instance != null)
		{
			GlobalManagers.Instance.ObjectPoolingManager = this;
		}
	}

	// runner.spawn이 호출될 때 호출
	// in 키워드는 매개변수로 넘어온 값을 함수내부에서 사용만할수 있고 값을 바꿀수 없게하므로 메모리가 적게든다
	public NetworkObjectAcquireResult AcquirePrefabInstance(NetworkRunner runner, in NetworkPrefabAcquireContext context, out NetworkObject result)
	{
		NetworkObject networkObject = null;
		NetworkPrefabId prefabID = context.PrefabId;
		INetworkPrefabSource prefabSource = NetworkProjectConfig.Global.PrefabTable.GetSource(prefabID);
		prefabsThatHadBeenInstantiated.TryGetValue(prefabSource, out var networkObjects);

		bool foundMatch = false;
		if (networkObjects?.Count > 0)
		{
			foreach (var item in networkObjects)
			{
				if (item != null && item.gameObject.activeSelf == false)
				{
					networkObject = item;
					foundMatch = true;
					break;
				}
			}
		}

		if (foundMatch == false)
		{
			// 새로운 오브젝트를 만들고 딕셔너리에 넣는다
			networkObject = CreateObjectInstance(prefabSource);
		}

		result = networkObject;
		return NetworkObjectAcquireResult.Success;
	}

	private NetworkObject CreateObjectInstance(INetworkPrefabSource prefab)
	{
		var obj = Instantiate(prefab.WaitForResult(), Vector3.zero, Quaternion.identity);

		if (prefabsThatHadBeenInstantiated.TryGetValue(prefab, out var instanceData)) // 딕셔너리에 있는 리스트사용
		{
			instanceData.Add(obj);
		}
		else // 리스트 새로 생성
		{
			var list = new List<NetworkObject> { obj }; // list에 obj를 add해서 생성
			prefabsThatHadBeenInstantiated.Add(prefab, list); // 새로만든 list를 딕셔너리에 넣는다
		}

		return obj;
	}

	// runner.despawn이 호출될 때 호출
	public void ReleaseInstance(NetworkRunner runner, in NetworkObjectReleaseContext context)
	{
		context.Object.gameObject.SetActive(false);
	}

	public void RemoveNetworkOjbectFromDic(NetworkObject obj)
	{
		if (prefabsThatHadBeenInstantiated.Count > 0)
		{
			foreach (var item in prefabsThatHadBeenInstantiated)
			{
				// obj와 같은것만 제거한다
				foreach (var networkObject in item.Value.Where(networkObject => networkObject == obj))
				{
					item.Value.Remove(networkObject);
					break;
				}
			}
		}
	}
}
