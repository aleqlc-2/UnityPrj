using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public static Spawner instance;

    private List<GameObject> islandPool = new List<GameObject>();
    private List<GameObject> enemyPlanePool = new List<GameObject>();
    private List<GameObject> cloudsPool = new List<GameObject>(); // 구름
    private List<GameObject> pickupPool = new List<GameObject>();
    private List<GameObject> balloonPool = new List<GameObject>();

    void Awake()
    {
        if (instance == null) instance = this;

        Object[] list = Resources.LoadAll("Islands");
        for (int i = 0; i < list.Length; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                GameObject obj = (GameObject)list[i];
                obj.SetActive(false);
                islandPool.Add(Instantiate(obj));
            }
        }

        list = Resources.LoadAll("Clouds");
        for (int i = 0; i < list.Length; i++)
        {
            for (int j = 0; j < 15; j++)
            {
                GameObject obj = (GameObject)list[i];
                obj.SetActive(false);
                cloudsPool.Add(Instantiate(obj));
            }
        }

        list = Resources.LoadAll("EnemyAir");
        for (int i = 0; i < list.Length; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                GameObject obj = (GameObject)list[i];
                obj.SetActive(false);
                enemyPlanePool.Add(Instantiate(obj));
            }
        }

        list = Resources.LoadAll("Balloons");
        for (int i = 0; i < list.Length; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                GameObject obj = (GameObject)list[i];
                obj.SetActive(false);
                balloonPool.Add(Instantiate(obj));
            }
        }

        list = Resources.LoadAll("PickUps");
        for (int i = 0; i < list.Length; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                GameObject obj = (GameObject)list[i];
                obj.SetActive(false);
                pickupPool.Add(Instantiate(obj));
            }
        }
    }

    public GameObject GetPooledIsland(string islandName)
    {
        GameObject pooledIsland = islandPool.Find(i => i.name == islandName && !i.activeInHierarchy);

        return pooledIsland;
    }

    // 활성화되지않은 구름 랜덤으로 골라오기
    public GameObject GetRandomPooledCloud()
    {
        int cloudChoice = Random.Range(0, cloudsPool.Count);

        while (cloudsPool[cloudChoice].activeInHierarchy)
        {
            cloudChoice = Random.Range(0, cloudsPool.Count);
        }

        return cloudsPool[cloudChoice];
    }

    public GameObject GetPooledEnemyPlane(string enemyPlaneName)
    {
        GameObject plane = enemyPlanePool.Find(p => p.name == enemyPlaneName && !p.activeInHierarchy);

        return plane;
    }

    public List<GameObject> GetPooledEnemyPlanePool()
    {
        return enemyPlanePool;
    }

    public GameObject GetPooledBalloon()
    {
        GameObject balloon = balloonPool.Find(b => !b.activeInHierarchy);

        return balloon;
    }

    public GameObject GetPooledPickup(string pickupName)
    {
        GameObject pickup = pickupPool.Find(p => p.name == pickupName && !p.activeInHierarchy);

        return pickup;
    }
}
