using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerScript : MonoBehaviour
{
    public float spawnDelay = 0.85f;

    public GameObject carPrefab;

    private float nextTimeToSpawn;

    void Update()
    {
        // Time.deltaTime, Time.fixedDeltaTime 쓰면 한번만 스폰됨.
        if (nextTimeToSpawn <= Time.time)
        {
            SpawnCar();
            nextTimeToSpawn = Time.time + spawnDelay;
        }
    }

    void SpawnCar()
    {
        Instantiate(carPrefab, transform.position, transform.rotation);
    }
}
