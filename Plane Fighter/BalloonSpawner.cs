using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonSpawner : MonoBehaviour
{
    private GameObject player;

    private bool continueSpawning;

    private WaitForSeconds waitForSeconds = new WaitForSeconds(10f);

    void Start()
    {
        player = GameObject.FindWithTag("Player");
    }
    
    IEnumerator SpawnBalloon()
    {
        while (continueSpawning)
        {
            GameObject balloon = Spawner.instance.GetPooledBalloon();
            balloon.SetActive(true);

            if (player.transform.position.y > 120)
            {
                balloon.transform.position = new Vector3(player.transform.position.x,
                                                         -18,
                                                         player.transform.position.z + 200);
            }
            else
            {
                balloon.transform.position = new Vector3(player.transform.position.x,
                                                         -18,
                                                         player.transform.position.z + 150);
            }

            yield return waitForSeconds;
        }
    }

    public void StartSpawningBalloons()
    {
        continueSpawning = true;
        StartCoroutine(SpawnBalloon());
    }

    public void StopSpawningBalloons()
    {
        continueSpawning = false;
        StopCoroutine(SpawnBalloon());
    }
}
