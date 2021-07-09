using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
	private float min_X = -4f, max_X = 4f;

	[SerializeField] private GameObject[] asteroid_Prefabs = null; // stage1
	[SerializeField] private GameObject enemyPrefab = null; // stage2

	private float timer = 0.5f;

	public List<GameObject> listStage1 = new List<GameObject>();
	public List<GameObject> listStage2 = new List<GameObject>();

	[SerializeField] private GameObject boss = null;

	public GameObject spawnStage(int stateNum)
	{
		float pos_X = Random.Range(min_X, max_X);
		Vector3 temp = transform.position;
		temp.z = -1;
		temp.x = pos_X;
		GameObject obj = null;

		if (stateNum == (int)GameManager.State.STAGE1)
		{
			obj = Instantiate(asteroid_Prefabs[Random.Range(0, asteroid_Prefabs.Length)],
					temp, Quaternion.identity);
		}
		else if (stateNum == (int)GameManager.State.STAGE2)
		{
			obj = Instantiate(enemyPrefab, temp, Quaternion.Euler(0f, 0f, 180f));
		}

		return obj;
	}

	public IEnumerator SpawnEnemies(int stateNum)
	{
		while (true)
		{
			if (stateNum == 0)
			{
				var obj = ObjPoolManager.Instance.GetEnemyStage1();
				listStage1.Add(obj);
			}

			if (stateNum == 1)
			{
				var obj2 = ObjPoolManager.Instance.GetEnemyStage2();
				listStage2.Add(obj2);
			}

			if (stateNum == 2)
			{
				ShowBoss();
				break;
			}

			yield return new WaitForSeconds(timer);
		}
	}

	public void ShowBoss()
	{
		boss.SetActive(true);
	}
}
