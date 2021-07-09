using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Enemy : MonoBehaviour
{
    public Ball ballPrefab;

    void OnTriggerEnter()
    {
        Instantiate(ballPrefab.gameObject, transform.position, Quaternion.identity);
        CreateRandomCube();
        Destroy(this.gameObject);
    }

    void CreateRandomCube()
    {
        Vector3 pos = new Vector3(Random.Range(-4f, 4f), Random.Range(1f, 9f), Random.Range(-4f, 4f));
        Instantiate(this.gameObject, pos, Quaternion.identity);
    }
}
