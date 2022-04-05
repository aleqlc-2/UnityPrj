using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayController : MonoBehaviour
{
    public static GameplayController instance;

    public GameObject[] heroes;
    public GameObject player_Spawn_FX;

    public Transform player_Spawn_Point;

    private EnemySpawner enemySpawner;

    [HideInInspector] public int enemy_Count;

    private bool second_Wave;

    private BossSpawnController bossSpawnController;

    void Awake()
    {
        MakeSingleton();

        enemySpawner = GetComponent<EnemySpawner>();

        bossSpawnController = GameObject.Find("Boss Spawn Controller").GetComponent<BossSpawnController>();
    }

    void Start()
    {
        SpawnPlayer();
    }

    private void MakeSingleton()
    {
        if (instance == null) instance = this;
    }

    public void SpawnPlayer()
    {
        StartCoroutine(SpawnPlayerAfterDelay());
    }

    private IEnumerator SpawnPlayerAfterDelay()
    {
        player_Spawn_FX.SetActive(true);

        yield return new WaitForSeconds(0.2f);

        // Quaternion.Euler(0f, 180f, 0f)는 생성된 캐릭터 프리펩의 Rotation 초기값이 얼마이건간에
        // (0f, 180f, 0f) 일 때 바라보는 방향이 되도록 Rotation값을 변화시킴
        Instantiate(heroes[GameManager.instance.selected_Hero_Index], player_Spawn_Point.position, Quaternion.Euler(0f, 180f, 0f));
    }

    public void EnemyDied()
    {
        enemy_Count--;

        if (enemy_Count <= 0) bossSpawnController.StartBossSpawn();

        //if (enemy_Count == 0)
        //{
        //    if (!second_Wave)
        //    {
        //        second_Wave = true;
        //        StartCoroutine(SpawnSecondWaveOfEnemies());
        //    }
        //    else
        //    {

        //    }
        //}
    }

    private IEnumerator SpawnSecondWaveOfEnemies()
    {
        yield return new WaitForSeconds(3f);

        SpawnEnemy(3);
    }

    public void SpawnEnemy(int enemyCount)
    {
        enemy_Count = enemyCount;
        enemySpawner.SpawnEnemy(enemyCount);
    }
}
