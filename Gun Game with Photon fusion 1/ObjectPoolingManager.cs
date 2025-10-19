using Fusion;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectPoolingManager : MonoBehaviour, INetworkObjectPool
{
	private Dictionary<NetworkObject, List<NetworkObject>> prefabsThatHadBeenInstantiated = new();

	private void Start()
	{
		if (GlobalManagers.Instance != null)
		{
			GlobalManagers.Instance.ObjectPoolingManager = this;
		}
	}

	// runner.spawn이 호출될 때 호출
	public NetworkObject AcquireInstance(NetworkRunner runner, NetworkPrefabInfo info)
	{
		NetworkObject networkObject = null;
		NetworkProjectConfig.Global.PrefabTable.TryGetPrefab(info.Prefab, out var prefab);
		prefabsThatHadBeenInstantiated.TryGetValue(prefab, out var networkObjects);

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
			networkObject = CreateObjectInstance(prefab);
		}

		return networkObject;
	}

	private NetworkObject CreateObjectInstance(NetworkObject prefab)
	{
		var obj = Instantiate(prefab);

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
	public void ReleaseInstance(NetworkRunner runner, NetworkObject instance, bool isSceneObject)
	{
		instance.gameObject.SetActive(false);
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
