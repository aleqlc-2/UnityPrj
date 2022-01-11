using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Enemy enemyPrefab;

    private Enemy spawnedEnemy;

    private Vector3 spawnPos;

    [SerializeField] private float minSpawnPos = -30f, maxSpawnPos = 30f;
    [SerializeField] private float minYpos = 1f, maxYpos = 5f;
    [SerializeField] private float minSpawnTime = 3f, maxSpawnTime = 6f;
    private float spawnTimer;

    private Transform playerTarget;

    void Awake()
    {
        playerTarget = GameObject.FindWithTag(TagManager.PLAYER_TAG).transform;
    }

    void Update()
    {
        CheckToSpawnEnemy();
    }

    private void CheckToSpawnEnemy()
    {
        if (Time.time > spawnTimer)
        {
            spawnTimer = Time.time + Random.Range(minSpawnTime, maxSpawnTime);
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        spawnPos = new Vector3(Random.Range(minSpawnPos, maxSpawnPos),
                               Random.Range(minYpos, maxYpos),
                               Random.Range(minSpawnPos, maxSpawnPos));

        spawnedEnemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        spawnedEnemy.SetTarget(playerTarget);
    }
}
