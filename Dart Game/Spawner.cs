using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject pinPrefab;

    void Update()
    {
        if (Input.GetButtonDown("Fire1")) // ctrl, 마우스좌클릭
        {
            spawnPin();
        }
    }

    void spawnPin()
    {
        Instantiate(pinPrefab, transform.position, transform.rotation);
    }
}
