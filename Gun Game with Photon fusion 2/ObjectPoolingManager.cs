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

	// runner.spawn�� ȣ��� �� ȣ��
	// in Ű����� �Ű������� �Ѿ�� ���� �Լ����ο��� ��븸�Ҽ� �ְ� ���� �ٲܼ� �����ϹǷ� �޸𸮰� ���Ե��
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
			// ���ο� ������Ʈ�� ����� ��ųʸ��� �ִ´�
			networkObject = CreateObjectInstance(prefabSource);
		}

		result = networkObject;
		return NetworkObjectAcquireResult.Success;
	}

	private NetworkObject CreateObjectInstance(INetworkPrefabSource prefab)
	{
		var obj = Instantiate(prefab.WaitForResult(), Vector3.zero, Quaternion.identity);

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
