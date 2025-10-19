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

	// runner.spawn�� ȣ��� �� ȣ��
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
			// ���ο� ������Ʈ�� ����� ��ųʸ��� �ִ´�
			networkObject = CreateObjectInstance(prefab);
		}

		return networkObject;
	}

	private NetworkObject CreateObjectInstance(NetworkObject prefab)
	{
		var obj = Instantiate(prefab);

		if (prefabsThatHadBeenInstantiated.TryGetValue(prefab, out var instanceData)) // ��ųʸ��� �ִ� ����Ʈ���
		{
			instanceData.Add(obj);
		}
		else // ����Ʈ ���� ����
		{
			var list = new List<NetworkObject> { obj }; // list�� obj�� add�ؼ� ����
			prefabsThatHadBeenInstantiated.Add(prefab, list); // ���θ��� list�� ��ųʸ��� �ִ´�
		}

		return obj;
	}

	// runner.despawn�� ȣ��� �� ȣ��
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
				// obj�� �����͸� �����Ѵ�
				foreach (var networkObject in item.Value.Where(networkObject => networkObject == obj))
				{
					item.Value.Remove(networkObject);
					break;
				}
			}
		}
	}
}
