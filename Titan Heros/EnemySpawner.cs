using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject[] enemies;
    public GameObject[] boss_Enemies;

    public Transform[] enemy_Spawn_Point;

    public GameObject boss_Spawn_FX;

    public void SpawnEnemy(int enemyCount)
    {
        for (int i = 0; i < enemyCount; i++)
        {
            Instantiate(enemies[Random.Range(0, enemies.Length)],
                        enemy_Spawn_Point[i].position,
                        Quaternion.identity);
        }
    }

    public void SpawnBoss(int bossIndex)
    {
        bossIndex = Random.Range(0, boss_Enemies.Length);
        boss_Spawn_FX.SetActive(true);
        Instantiate(boss_Enemies[bossIndex], enemy_Spawn_Point[1].position, Quaternion.identity);
    }
}
