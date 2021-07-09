using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBombs : MonoBehaviour
{
    public Transform CameraRig;
    public GameObject[] balls;
    public float radius = 2f;

    void Start()
    {
        StartCoroutine(SpawnBombsOverTime());
    }

    IEnumerator SpawnBombsOverTime()
    {
        while(true)
        {
            GameObject ball = Instantiate(balls[Random.Range(0, balls.Length)]);
            float angle = Random.Range(0f, 360f);
            float radius = Random.Range(1f, 1.25f);
            ball.transform.position = CameraRig.position +
                new Vector3(Mathf.Sin(angle) * radius, Random.Range(1.25f, 1.75f), Mathf.Cos(angle) * radius);
            yield return new WaitForSeconds(Random.Range(1f, 3f));
        }
    }
}
