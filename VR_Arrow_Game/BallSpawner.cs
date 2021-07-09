using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallSpawner : MonoBehaviour
{
    public GameObject ballPrefab;
    public Transform[] spawnPoints;

    public float minDelay = 1f;
    public float maxDelay = 3f;

    void Start()
    {
        StartCoroutine(ballSpawn());
    }

    IEnumerator ballSpawn()
    {
        while(true)
        {
            float delay = Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds(delay);

            int spawnIndex = Random.Range(0, spawnPoints.Length);
            Transform spawnPoint = spawnPoints[spawnIndex];

            GameObject spawnedBall = Instantiate(ballPrefab, spawnPoint.position, spawnPoint.rotation);
            Destroy(spawnedBall, 10f);
        }
    }
}
