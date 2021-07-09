using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    public GameObject[] asteroidPrefabs;

    void Start()
    {
        StartCoroutine(SpawnAsteroids());
    }

    IEnumerator SpawnAsteroids()
    {
        while(true)
        {
            Instantiate(asteroidPrefabs[Random.Range(0, asteroidPrefabs.Length)],
                        Random.onUnitSphere * Random.Range(0.7f, 2f),
                        Quaternion.identity);

            yield return new WaitForSeconds(Random.Range(0.3f, 1f));
        }
    }
}
