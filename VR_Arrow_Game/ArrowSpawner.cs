using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowSpawner : MonoBehaviour
{
    public GameObject ArrowPrefab;
    private GameObject arrow;

    void Update()
    {
        ArrowSpawn();
    }

    public void ArrowSpawn()
    {
        if (arrow == null)
        {
            arrow = Instantiate(ArrowPrefab);
            arrow.transform.position = this.gameObject.transform.position;
        }
    }
}
