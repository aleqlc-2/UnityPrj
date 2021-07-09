using System.Collections.Generic;
using UnityEngine;

public class ObjPoolManager : MonoBehaviour
{
	//[Serializable]
	//public class TestA
	//{
	//	public PoolType pool;
	//	[Range(0,10)]		public int count;
	//	public GameObject go;
	//}

	//[SerializeField] private TestA[] aa;

	//// 딕셔너리로 만들것
	//public enum PoolType { a, b, c }
	//Dictionary<PoolType, GameObject> dicNameAndGO = new Dictionary<PoolType, GameObject>();
	//Dictionary<PoolType, List<GameObject>> dicNameAndGO = new Dictionary<PoolType, List<GameObject>>();

	public static ObjPoolManager Instance = null;

	[SerializeField] private EnemySpawner enemySpawner = null;
	[SerializeField] private BossHealth bossHealth = null;

	[SerializeField] private GameObject poolingObjectPrefab = null;
	[SerializeField] private GameObject playerBullet = null;
	[SerializeField] private GameObject rotateBullet = null;

	Queue<GameObject> poolingObjectQueue = new Queue<GameObject>();
	Queue<GameObject> poolingBullet = new Queue<GameObject>();
	Queue<GameObject> poolingRotateBullet = new Queue<GameObject>();
	Queue<GameObject> enemiesStage1 = new Queue<GameObject>();
	Queue<GameObject> enemiesStage2 = new Queue<GameObject>();

	private int initCount = 16;
	public int InitCount
	{
		get { return initCount; }
		set { initCount = value; }
	}

	private void Awake()
	{
		if (Instance == null)
			Instance = this;

		Initialize(initCount);
	}

	private void Initialize(int initCount)
	{
		for (int i = 0; i < initCount + 100; i++)
		{
			poolingBullet.Enqueue(CreatePlayerBullet()); // 플레이어가 쏘는 bullet
					}

		for (int i = 0; i < initCount; i++)
		{
			enemiesStage1.Enqueue(CreateEnemyStage((int)GameManager.State.STAGE1)); // stage1의 enemy
		}

		for (int i = 0; i < initCount; i++)
		{
			enemiesStage2.Enqueue(CreateEnemyStage((int)GameManager.State.STAGE2)); // stage2의 enemy
		}

		for (int i = 0; i < initCount; i++)
		{
			poolingObjectQueue.Enqueue(CreateStraightBullet()); // 보스가 일직선으로 쏘는 bullet
		}

		for (int i = 0; i < initCount + 15; i++)
		{
			poolingRotateBullet.Enqueue(CreateRotateBullet()); // 보스가 회전하며 쏘는 bullet
		}
	}

	private GameObject CreatePlayerBullet()
	{
		Debug.Log($"[PoolManager] CreatePlayerBullet :");
		var newObj = Instantiate(playerBullet);
		newObj.gameObject.SetActive(false);
		newObj.transform.SetParent(transform);
		return newObj;
	}

	private GameObject CreateEnemyStage(int stateNum)
	{
		var newObj = enemySpawner.spawnStage(stateNum);
		newObj.gameObject.SetActive(false);
		newObj.transform.SetParent(transform);
		return newObj;
	}

	private GameObject CreateStraightBullet()
	{
		var newObj = Instantiate(poolingObjectPrefab);
		newObj.gameObject.SetActive(false);
		newObj.transform.SetParent(transform);
		return newObj;
	}

	private GameObject CreateRotateBullet()
	{
		Debug.Log($"[PoolManager] CreatePlayerBullet :");
		var newObj = Instantiate(rotateBullet);
		newObj.gameObject.SetActive(false);
		newObj.transform.SetParent(transform);
		return newObj;
	}

	public GameObject GetPlayerBullet()
	{
		Debug.Log($"[ObjPoolManager] : Count : {Instance.poolingBullet.Count}");
		if (Instance.poolingBullet.Count > 0)
		{
			var obj = Instance.poolingBullet.Dequeue();
			obj.transform.SetParent(null);
			obj.gameObject.SetActive(true);
			return obj;
		}
		else
		{
			var newObj = Instance.CreatePlayerBullet();
			newObj.gameObject.SetActive(true);
			newObj.transform.SetParent(null);
			return newObj;
		}
	}

	public GameObject GetEnemyStage1()
	{
		if (Instance.enemiesStage1.Count > 0)
		{
			var obj = Instance.enemiesStage1.Dequeue();
			obj.transform.SetParent(null);
			obj.gameObject.SetActive(true);
			return obj;
		}
		else
		{
			var newObj = Instance.CreateEnemyStage((int)GameManager.State.STAGE1);
			newObj.gameObject.SetActive(true);
			newObj.transform.SetParent(null);
			return newObj;
		}
	}

	public GameObject GetEnemyStage2()
	{
		if (Instance.enemiesStage2.Count > 0)
		{
			var obj = Instance.enemiesStage2.Dequeue();
			obj.transform.SetParent(null);
			obj.gameObject.SetActive(true);
			return obj;
		}
		else
		{
			var newObj = Instance.CreateEnemyStage((int)GameManager.State.STAGE2);
			newObj.gameObject.SetActive(true);
			newObj.transform.SetParent(null);
			return newObj;
		}
	}

	public GameObject GetStrightBullet()
	{
		if (Instance.poolingObjectQueue.Count > 0)
		{
			var obj = Instance.poolingObjectQueue.Dequeue();
			obj.transform.SetParent(null);
			obj.gameObject.SetActive(true);
			return obj;
		}
		else
		{
			var newObj = Instance.CreateStraightBullet();
			newObj.gameObject.SetActive(true);
			newObj.transform.SetParent(null);
			return newObj;
		}
	}

	public GameObject GetRotateBullet()
	{
		Debug.Log($"[ObjPoolManager] : Count : {Instance.poolingRotateBullet.Count}");
		if (Instance.poolingRotateBullet.Count > 0)
		{
			var obj = Instance.poolingRotateBullet.Dequeue();
			obj.transform.SetParent(null);
			obj.gameObject.SetActive(true);
			return obj;
		}
		else
		{
			var newObj = Instance.CreateRotateBullet();
			newObj.gameObject.SetActive(true);
			newObj.transform.SetParent(null);
			return newObj;
		}
	}

	public void ReturnPlayerBullet(GameObject obj)
	{
		if (obj.gameObject.activeSelf)
		{
			obj.gameObject.SetActive(false);
		}
		obj.transform.SetParent(Instance.transform);
		Instance.poolingBullet.Enqueue(obj);
	}

	public void ReturnEnemyStage1(GameObject obj)
	{
		obj.gameObject.SetActive(false);
		obj.transform.SetParent(Instance.transform);
		Instance.enemiesStage1.Enqueue(obj);
	}

	public void ReturnEnemyStage2(GameObject obj)
	{
		obj.gameObject.SetActive(false);
		obj.transform.SetParent(Instance.transform);
		Instance.enemiesStage2.Enqueue(obj);
	}

	public void ReturnStraightBullet(List<GameObject> objList)
	{
		foreach (GameObject obj in objList)
		{
			obj.gameObject.SetActive(false);
			obj.transform.SetParent(Instance.transform);
			Instance.poolingObjectQueue.Enqueue(obj);
		}
	}

	public void ReturnRotateBullet(List<GameObject> objList)
	{
		foreach (GameObject obj in objList)
		{
			if (obj.gameObject.activeSelf)
			{
				obj.gameObject.SetActive(false);
			}

			obj.transform.SetParent(Instance.transform);
			Instance.poolingRotateBullet.Enqueue(obj);
		}
	}

	public void ReturnEnemiesByStage(int stateNum) // 스테이지 종료에 의한 enemy 리턴
	{
		if (stateNum == 0)
		{
			foreach (GameObject obj in enemySpawner.listStage1)
			{
				ReturnEnemyStage1(obj);
			}

			GameManager.Instance.Lives = 3;
		}
		else if (stateNum == 1)
		{
			foreach (GameObject obj2 in enemySpawner.listStage2)
			{
				ReturnEnemyStage2(obj2);
			}

			GameManager.Instance.Lives = 3;
		}
	}

	public void ReturnEnemiesByBomb(GameManager.State state) // 필살기에 의한 enemy 리턴
	{
		if (state == GameManager.State.STAGE1)
		{
			foreach (var obj in enemySpawner.listStage1)
			{
				ReturnEnemyStage1(obj);
			}
		}
		else if (state == GameManager.State.STAGE2)
		{
			foreach (var obj in enemySpawner.listStage2)
			{
				ReturnEnemyStage2(obj);
			}
		}
		else if (state == GameManager.State.BOSS)
		{
			bossHealth.Bosshealth -= 100;
		}
	}

	private void OnDestroy()
	{
		if (Instance != null)
			Instance = null;
	}
}
