using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudSpawner : MonoBehaviour
{
    private GameObject player;

    public int leftSide_SpawnLimit_LX = -3200;
    public int leftSide_SpawnLimit_RX = 0;

    public int rightSide_SpawnLimit_LX = 200;
    public int rightSide_SpawnLimit_RX = 4200;

    public float cloudLiftHeightPlus = 50f;

    void Start()
    {
        player = GameObject.FindWithTag("Player");

        SpawnCloudBlock(-10000);
        SpawnCloudBlock(-9000);
        SpawnCloudBlock(-8000);
        SpawnCloudBlock(-7000);
        SpawnCloudBlock(-6000);
        SpawnCloudBlock(-5000);
        SpawnCloudBlock(-4000);
        SpawnCloudBlock(-3000);
        SpawnCloudBlock(-2000);
        SpawnCloudBlock(-1000);
    }

    void Update()
    {
        // z축으로 1000만큼 이동했을때 구름생성
        if ((int)player.transform.position.z > 0 && (int)player.transform.position.z % 1000 == 0)
        {
            SpawnCloudBlock(player.transform.position.z);
        }
    }

    private void SpawnCloudBlock(float playerPosZ)
    {
        float zSpawnPos = playerPosZ + 8200;

        List<GameObject> clouds = new List<GameObject>();

        for (int i = 0; i < 4; i++)
        {
            GameObject newCloud = Spawner.instance.GetRandomPooledCloud();
            clouds.Add(newCloud);
            newCloud.SetActive(true);
        }

        // 왼쪽 배경에 구름 생성
        for (int i = 0; i < clouds.Count / 2; i++)
        {
            clouds[i].transform.position = new Vector3(Random.Range(leftSide_SpawnLimit_LX, leftSide_SpawnLimit_RX),
                                                       clouds[i].transform.position.y + cloudLiftHeightPlus,
                                                       Random.Range(zSpawnPos, zSpawnPos + 1000));
        }

        // 오른쪽 배경에 구름 생성
        for (int i = 0; i < clouds.Count / 2; i++)
        {
            clouds[i].transform.position = new Vector3(Random.Range(rightSide_SpawnLimit_LX, rightSide_SpawnLimit_RX),
                                                       clouds[i].transform.position.y + cloudLiftHeightPlus,
                                                       Random.Range(zSpawnPos, zSpawnPos + 1000));
        }
    }
}
