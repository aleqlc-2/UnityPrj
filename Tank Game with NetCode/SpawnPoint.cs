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

	// �����Ϳ��� �� ��ũ��Ʈ�� ������ ������Ʈ �����Ҷ��� �����׸�
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(transform.position, 1);
	}
}
