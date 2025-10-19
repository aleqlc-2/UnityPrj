using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    private static List<SpawnPoint> spawnPoints = new List<SpawnPoint>();

	private void OnEnable()
	{
		spawnPoints.Add(this);
	}

	public static Vector3 GetRandomSpawnPos()
    {
        if (spawnPoints.Count == 0) return Vector3.zero;

        return spawnPoints[Random.Range(0, spawnPoints.Count)].transform.position;
    }

	private void OnDisable()
	{
		spawnPoints.Remove(this);
	}

	// 에디터에서 이 스크립트가 부착된 오브젝트 선택할때만 기즈모그림
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(transform.position, 1);
	}
}
